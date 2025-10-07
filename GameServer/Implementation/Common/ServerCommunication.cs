using GameServer.Models.Config;
using GameServer.Models.ServerCommunication;
using GameServer.Utils;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameServer.Models.PlayerData;
using GameServer.Models.ServerCommunication.Events;
using GameServer.Models.PlayerData.PlayerCreations;

namespace GameServer.Implementation.Common
{
    public static class ServerCommunication
    {
        private const int DefaultMessageCapacity = 4096;
        private const string MasterServer = "API";
        private static readonly List<ServerInfo> Servers = [];

        private static readonly int EncryptedHeaderSize = AesGcm.NonceByteSizes.MaxSize + AesGcm.TagByteSizes.MaxSize;
        private static readonly AesGcm Aes;
        private static readonly bool HasKey = !string.IsNullOrEmpty(ServerConfig.Instance.ServerCommunicationKey);

        static ServerCommunication()
        {
            if (HasKey)
            {
                Aes = new AesGcm(Encoding.UTF8.GetBytes(ServerConfig.Instance.ServerCommunicationKey), AesGcm.TagByteSizes.MaxSize);
            }
        }

        public static void NotifySessionCreated(Guid uuid, int playerConnectId, string username, int issuer, Platform platform)
        {
            DispatchEvent(GatewayEvents.PlayerSessionCreated, new PlayerSessionCreatedEvent
            {
                SessionUuid = uuid.ToString(),
                PlayerConnectId = playerConnectId,
                Username = username,
                Issuer = issuer,
                SessionPlatform = platform
            }).Wait();
        }
        
        public static void NotifySessionDestroyed(Guid uuid)
        {
            DispatchEvent(GatewayEvents.PlayerSessionDestroyed, new PlayerSessionDestroyedEvent
            {
                SessionUuid = uuid.ToString()
            }).Wait();
        }

        public static void NotifyHotSeatPlaylistReset()
        {
            DispatchEvent(GatewayEvents.HotSeatPlaylistReset, null).Wait();
        }

        public static async Task HandleConnection(WebSocket webSocket, Guid ServerID)
        {
            byte[] scratch = new byte[DefaultMessageCapacity];
            while (webSocket.State == WebSocketState.Open)
            {
                var buffer = new List<byte>();
                WebSocketReceiveResult result;
                try
                {
                    do
                    {
                        result = await webSocket.ReceiveAsync(scratch, CancellationToken.None);
                        buffer.AddRange(scratch.AsSpan(0, result.Count));
                    }
                    while (!result.EndOfMessage);
                }
                catch (Exception e)
                {
                    Log.Debug($"There was an error receiving message: {e}");
                    continue;
                }

                string payload;
                if (result.MessageType == WebSocketMessageType.Text && !HasKey)
                    payload = Encoding.UTF8.GetString(buffer.ToArray());
                else if (result.MessageType == WebSocketMessageType.Binary && HasKey)
                {
                    if (buffer.Count < EncryptedHeaderSize)
                    {
                        Log.Debug("Received message with invalid header!");
                        break;
                    }

                    try
                    {
                        payload = Decrypt(CollectionsMarshal.AsSpan(buffer));
                    }
                    catch (Exception e)
                    {
                        Log.Debug($"Failed to decrypt message: {e}");
                        break;
                    }
                }
                else break;

                try
                {
                    var message = JsonConvert.DeserializeObject<GatewayMessage>(payload);
                    if (message != null)
                    {
                        message.From = ServerID.ToString();
                        Database database = new();
                        ProcessMessage(database, webSocket, message);
                        database.Dispose();
                    }
                }
                catch (Exception e)
                {
                    Log.Debug($"Failed to process message: {e}");
                }
            }

            Servers.RemoveAll(match => match.ServerId == ServerID);
            if (webSocket is { State: WebSocketState.Aborted or WebSocketState.Closed or WebSocketState.CloseSent })
                return;
            
            try
            {
                await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
            catch (Exception e)
            {
                Log.Debug($"There was an error while closing connection: {e}");
            }
        }

        private static void ProcessMessage(Database database, WebSocket socket, GatewayMessage message)
        {
            var response = new GatewayMessage
            {
                From = MasterServer,
                To = message.From,
                Type = $"{message.Type}Error"
            };

            if (message.To == MasterServer)
            {
                ServerInfo server = Servers.FirstOrDefault(match => match.ServerId.ToString() == message.From);
                if (server == null && message.Type != GatewayEvents.ServerInfo)
                {
                    response.Content = $"Unknown sender {message.From}";
                    Send(socket, JsonConvert.SerializeObject(response)).Wait();
                    return;
                }
                
                switch (message.Type) 
                {
                    case GatewayEvents.ServerInfo:
                    {
                        if (ParseMessage(message, response, out ServerInfo info))
                        {
                            info.ServerId = Guid.Parse(message.From);
                            info.Socket = socket;
                            Servers.Add(info);
                            return;
                        }
                        
                        break;   
                    }

                    case GatewayEvents.EventStarted:
                    {
                        if (ParseMessage(message, response, out EventStartedEvent info))
                        {
                            PlayerCreationData creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == info.TrackId);
                            if (creation != null)
                            {
                                database.PlayerCreationRacesStarted.Add(new PlayerCreationRaceStarted
                                {
                                    PlayerCreationId = info.TrackId,
                                    StartedAt = TimeUtils.Now,
                                });
                                foreach (int playerId in info.PlayerIds)
                                {
                                    User user = database.Users.FirstOrDefault(match => match.UserId == playerId);
                                    if (user != null)
                                    {
                                        var character = database.PlayerCreations.FirstOrDefault(match => match.Type == PlayerCreationType.CHARACTER && match.PlayerCreationId == user.CharacterIdx);
                                        var kart = database.PlayerCreations.FirstOrDefault(match => match.Type == PlayerCreationType.KART && match.PlayerCreationId == user.KartIdx);
                                        if (character != null)
                                            database.PlayerCreationRacesStarted.Add(new PlayerCreationRaceStarted { PlayerCreationId = character.PlayerCreationId, StartedAt = TimeUtils.Now });
                                        if (kart != null)
                                            database.PlayerCreationRacesStarted.Add(new PlayerCreationRaceStarted { PlayerCreationId = kart.PlayerCreationId, StartedAt = TimeUtils.Now });
                                        database.OnlineRacesStarted.Add(new RaceStarted { PlayerId = user.UserId, StartedAt = TimeUtils.Now });
                                    }
                                }
                                database.SaveChanges();
                            }
                            return;
                        }

                        break;
                    }

                    case GatewayEvents.EventFinished:
                    {
                        if (ParseMessage(message, response, out EventFinishedEvent info))
                        {
                            PlayerCreationData creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == info.TrackId);
                            if (creation != null)
                            {
                                creation.RacesFinished++;
                                foreach (var player in info.Stats)
                                {
                                    User user = database.Users.FirstOrDefault(match => match.UserId == player.PlayerConnectId);
                                    if (user != null)
                                    {
                                        if (info.IsMNR)
                                        {
                                            var character = database.PlayerCreations.FirstOrDefault(match => match.Type == PlayerCreationType.CHARACTER && match.PlayerCreationId == user.CharacterIdx);
                                            var kart = database.PlayerCreations.FirstOrDefault(match => match.Type == PlayerCreationType.KART && match.PlayerCreationId == user.KartIdx);

                                            if (character != null)
                                            {
                                                character.RacesFinished++;
                                                if (player.Rank == 1)
                                                    character.RacesWon++;

                                                if (player.BestDrift > character.LongestDrift)
                                                    character.LongestDrift = player.BestDrift;
                                                if (player.BestHangTime > character.LongestHangTime)
                                                    character.LongestHangTime = player.BestHangTime;
                                                if (player.BestLapTime > character.BestLapTime)
                                                    character.BestLapTime = player.BestLapTime;
                                            }

                                            if (kart != null)
                                            {
                                                kart.RacesFinished++;
                                                if (player.Rank == 1)
                                                    kart.RacesWon++;

                                                if (player.BestDrift > kart.LongestDrift)
                                                    kart.LongestDrift = player.BestDrift;
                                                if (player.BestHangTime > kart.LongestHangTime)
                                                    kart.LongestHangTime = player.BestHangTime;
                                                if (player.BestLapTime > kart.BestLapTime)
                                                    kart.BestLapTime = player.BestLapTime;
                                            }
                                        }

                                        database.OnlineRacesFinished.Add(new RaceFinished { PlayerId = user.UserId, FinishedAt = TimeUtils.Now, IsWinner = player.Rank == 1 });
                                        
                                        if (player.Rank == 1)
                                        {
                                            creation.RacesWon++;
                                            user.WinStreak++;
                                            if (user.WinStreak > user.LongestWinStreak)
                                                user.LongestWinStreak = user.WinStreak;
                                        }
                                        else user.WinStreak = 0;
                                        
                                        if (player.BestDrift > user.LongestDrift)
                                            user.LongestDrift = player.BestDrift;
                                        if (player.BestHangTime > user.LongestHangTime)
                                            user.LongestHangTime = player.BestHangTime;

                                        if (player.BestDrift > creation.LongestDrift)
                                            creation.LongestDrift = player.BestDrift;
                                        if (player.BestHangTime > creation.LongestHangTime)
                                            creation.LongestHangTime = player.BestHangTime;
                                        if (player.BestLapTime > creation.BestLapTime)
                                            creation.BestLapTime = player.BestLapTime;

                                        Score score;

                                        if (!info.IsMNR)
                                        {
                                            score = database.Scores.FirstOrDefault(match => match.PlayerId == player.PlayerConnectId
                                                && match.SubKeyId == info.TrackId
                                                && match.SubGroupId == (int)info.GameType
                                                && match.Platform == creation.Platform
                                                && match.PlaygroupSize == player.PlaygroupSize
                                                && match.IsMNR == info.IsMNR);
                                        }
                                        else
                                        {
                                            score = database.Scores.FirstOrDefault(match => match.PlayerId == player.PlayerConnectId
                                                && match.SubKeyId == info.TrackId
                                                && match.SubGroupId == (int)info.GameType - 10
                                                && match.Platform == creation.Platform 
                                                && match.IsMNR == info.IsMNR);
                                        }

                                        if (!info.IsMNR)
                                        {
                                            var leaderboard = database.Scores.Where(match => match.SubKeyId == info.TrackId
                                                && match.SubGroupId == (int)info.GameType && match.PlaygroupSize == player.PlaygroupSize
                                                && match.Platform == creation.Platform);

                                            if (creation.ScoreboardMode == 1)
                                                leaderboard = leaderboard.OrderBy(s => s.FinishTime);
                                            else
                                                leaderboard = leaderboard.OrderByDescending(s => s.Points);

                                            if (leaderboard.Any())
                                            {
                                                var HighScore = leaderboard.FirstOrDefault();
                                                if (creation.ScoreboardMode == 1 && HighScore != null && HighScore.FinishTime > player.FinishTime)
                                                {
                                                    database.ActivityLog.Add(new ActivityEvent
                                                    {
                                                        AuthorId = user.UserId,
                                                        Type = ActivityType.player_event,
                                                        List = ActivityList.both,
                                                        Topic = "player_beat_finish_time",
                                                        Description = $"{player.FinishTime}",
                                                        PlayerId = 0,
                                                        PlayerCreationId = creation.PlayerCreationId,
                                                        CreatedAt = TimeUtils.Now,
                                                        AllusionId = creation.PlayerCreationId,
                                                        AllusionType = "PlayerCreation::Track"
                                                    });
                                                }
                                                if (creation.ScoreboardMode == 0 && HighScore != null && HighScore.Points < player.Points)
                                                {
                                                    database.ActivityLog.Add(new ActivityEvent
                                                    {
                                                        AuthorId = user.UserId,
                                                        Type = ActivityType.player_event,
                                                        List = ActivityList.both,
                                                        Topic = "player_beat_score",
                                                        Description = $"{player.Points}",
                                                        PlayerId = 0,
                                                        PlayerCreationId = creation.PlayerCreationId,
                                                        CreatedAt = TimeUtils.Now,
                                                        AllusionId = creation.PlayerCreationId,
                                                        AllusionType = "PlayerCreation::Track"
                                                    });
                                                }
                                            }
                                        }

                                        if (score != null)
                                        {
                                            if (score.FinishTime > player.FinishTime)
                                            {
                                                score.FinishTime = player.FinishTime;
                                                score.UpdatedAt = TimeUtils.Now;
                                                score.CharacterIdx = user.CharacterIdx;
                                                score.KartIdx = user.KartIdx;
                                            }
                                            if (score.Points < player.Points)
                                            {
                                                score.Points = player.Points;
                                                score.UpdatedAt = TimeUtils.Now;
                                            }
                                            if (score.BestLapTime > player.BestLapTime)
                                            {
                                                score.BestLapTime = player.BestLapTime;
                                                score.UpdatedAt = TimeUtils.Now;
                                                score.CharacterIdx = user.CharacterIdx;
                                                score.KartIdx = user.KartIdx;
                                            }
                                        }
                                        else
                                        {
                                            database.Scores.Add(new Score
                                            {
                                                CreatedAt = TimeUtils.Now,
                                                FinishTime = player.FinishTime,
                                                Platform = creation.Platform,
                                                PlayerId = player.PlayerConnectId,
                                                PlaygroupSize = player.PlaygroupSize,
                                                Points = player.Points,
                                                SubGroupId = info.IsMNR ? (int)info.GameType - 10 : (int)info.GameType,
                                                SubKeyId = info.TrackId,
                                                UpdatedAt = TimeUtils.Now,
                                                Latitude = 0,
                                                Longitude = 0,
                                                BestLapTime = player.BestLapTime,
                                                KartIdx = user.KartIdx,
                                                CharacterIdx = user.CharacterIdx,
                                                GhostCarDataMD5 = "",
                                                IsMNR = info.IsMNR,
                                                LocationTag = null
                                            });
                                        }
                                    }
                                }
                                database.SaveChanges();
                            }
                            return;
                        }

                        break;
                    }
                    
                    case GatewayEvents.UpdatePlayerCount:
                    {
                        if (ParseMessage(message, response, out UpdatePlayerCountEvent info))
                        {
                            server.PlayerCount = info.PlayerCount;
                            return;
                        }
                        
                        break;   
                    }
                    case GatewayEvents.PlayerQuit:
                    {
                        if (ParseMessage(message, response, out PlayerQuitEvent info))
                        {
                            User user = database.Users.FirstOrDefault(match => match.UserId == info.PlayerConnectId);
                            if (user != null)
                            {
                                if (info.Disconnected) user.OnlineDisconnected++;
                                else user.OnlineForfeit++;
                                database.SaveChanges();
                            }
                            
                            return;
                        }
                        
                        break;
                    }
                    case GatewayEvents.PlayerUpdated:
                    {
                        if (ParseMessage(message, response, out PlayerUpdatedEvent info))
                        {
                            User user = database.Users.FirstOrDefault(match => match.UserId == info.PlayerConnectId);
                            
                            // What should be done if Kart/Character Id is a local creation?
                            if (user != null)
                            {
                                user.CharacterIdx = info.CharacterId;
                                user.KartIdx = info.KartId;
                                database.SaveChanges();
                            }
                            
                            return;
                        }
                        
                        break;
                    }
                    default:
                    {
                        response.Content = $"Unknown message type {message.Type}";
                        break;
                    }
                }
                
                Send(socket, JsonConvert.SerializeObject(response)).Wait();
            }
            else if (message.To == "Broadcast")
            {
                foreach (var server in Servers)
                {
                    Send(server.Socket, JsonConvert.SerializeObject(message)).Wait();
                }
            }
            else if (Servers.FirstOrDefault(match => match.ServerId.ToString() == message.From) != null)
            {
                response.Content = $"Cannot find receiver {message.To}";
                var receiver = Servers.FirstOrDefault(match => match.ServerId == Guid.Parse(message.To));
                if (receiver != null)
                    Send(receiver.Socket, JsonConvert.SerializeObject(message)).Wait();
                else
                    Send(socket, JsonConvert.SerializeObject(response)).Wait();
            }
            else
            {
                response.Content = $"Unknown sender {message.From}";
                Send(socket, JsonConvert.SerializeObject(response)).Wait();
            }
        }

        private static bool ParseMessage<T>(GatewayMessage message, GatewayMessage response, out T evt)
        {
            evt = JsonConvert.DeserializeObject<T>(message.Content);
            if (evt != null) return true;
            response.Content = $"Cannot parse {message.Type}";
            return false;
        }

        private static async Task DispatchEvent(string type, object evt)
        {
            var message = new GatewayMessage
            {
                Type = type,
                From = MasterServer,
                To = "Broadcast",
                Content = evt != null ? JsonConvert.SerializeObject(evt) : ""
            };
            
            string payload = JsonConvert.SerializeObject(message);
            foreach (ServerInfo server in Servers)
                await Send(server.Socket, payload);
        }

        private static async Task Send(WebSocket webSocket, string message)
        {
            byte[] bytes;

            var type = WebSocketMessageType.Text;
            if (HasKey)
            {
                type = WebSocketMessageType.Binary;
                bytes = Encrypt(message);
            }
            else bytes = Encoding.UTF8.GetBytes(message);

            if (webSocket.State == WebSocketState.Open)
                await webSocket.SendAsync(bytes, type, true, CancellationToken.None);
        }

        public static ServerInfo GetServer(ServerType Type)
        {
            return Servers.Where(match => match.Type == Type).MinBy(s => s.PlayerCount);
        }

        public static ServerInfo GetServer(Guid ServerID)
        {
            return Servers.FirstOrDefault(match => match.ServerId == ServerID);
        }

        private static byte[] Encrypt(string message)
        {
            byte[] contents = Encoding.UTF8.GetBytes(message);
            byte[] payload = new byte[EncryptedHeaderSize + contents.Length];

            var nonce = payload.AsSpan(0, AesGcm.NonceByteSizes.MaxSize);
            var tag = payload.AsSpan(AesGcm.NonceByteSizes.MaxSize, AesGcm.TagByteSizes.MaxSize);
            var output = payload.AsSpan(EncryptedHeaderSize, contents.Length);
            
            RandomNumberGenerator.Fill(nonce);
            Aes.Encrypt(nonce, contents, output, tag);
            return payload;
        }

        private static string Decrypt(Span<byte> message)
        {
            ReadOnlySpan<byte> nonce = message[..AesGcm.NonceByteSizes.MaxSize];
            ReadOnlySpan<byte> tag = message.Slice(AesGcm.NonceByteSizes.MaxSize, AesGcm.TagByteSizes.MaxSize);
            ReadOnlySpan<byte> data = message.Slice(EncryptedHeaderSize, message.Length - EncryptedHeaderSize);

            byte[] output = new byte[data.Length];
            Aes.Decrypt(nonce, data, tag, output);
            return Encoding.UTF8.GetString(output);
        }

        public static async Task DisconnectAllServers(string statusDescription)
        {
            foreach (var server in Servers.ToList())
                await server.Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, statusDescription, CancellationToken.None);
        }
    }
}

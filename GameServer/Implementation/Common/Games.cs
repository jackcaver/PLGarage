using GameServer.Models;
using GameServer.Models.GameBrowser;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Implementation.Common
{
    public class Games
    {
        private static readonly List<GameData> GameList = [];
        private static int NextId = 0;

        public static string GetList(int page, int per_page, Filters filters)
        {
            if (page == 0) page = 1;

            var FilteredGameList = new List<GameData>(GameList);

            if (filters.game_state != null)
                FilteredGameList.RemoveAll(match => match.State != filters.game_state);
            if (filters.is_ranked != null)
                FilteredGameList.RemoveAll(match => match.IsRanked != filters.is_ranked);
            if (filters.game_type != null)
                FilteredGameList.RemoveAll(match => match.Type != filters.game_type);
            if (filters.platform != null)
                FilteredGameList.RemoveAll(match => match.Platform != filters.platform);
            if (filters.track_group != null)
                FilteredGameList.RemoveAll(match => match.TrackGroup != filters.track_group);
            if (filters.privacy != null)
                FilteredGameList.RemoveAll(match => match.Privacy != filters.privacy);
            if (!string.IsNullOrEmpty(filters.speed_class))
                FilteredGameList.RemoveAll(match => match.SpeedClass != filters.speed_class);
            if (filters.track != null)
                FilteredGameList.RemoveAll(match => match.Track != filters.track);
            if (filters.number_laps != null)
                FilteredGameList.RemoveAll(match => match.NumberLaps != filters.number_laps);

            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, GameList.Count);

            if (pageEnd > FilteredGameList.Count)
                pageEnd = FilteredGameList.Count;

            var gameList = new List<GameListGame>();

            for (int i = pageStart; i < pageEnd; i++)
            {
                var game = FilteredGameList[i];
                if (game != null)
                {
                    gameList.Add(new GameListGame
                    {
                        id = game.Id,
                        name = game.Name,
                        host_player_id = game.HostPlayerId,
                        host_player_ip_address = game.HostPlayerIP,
                        cur_players = game.Players.Count,
                        max_players = game.MaxPlayers,
                        min_players = game.MinPlayers,
                        game_type = game.Type.ToString(),
                        game_state_id = (int)game.State,
                        is_ranked = game.IsRanked,
                        track = game.Track,
                        lobby_channel_id = game.LobbyChannelId,
                        number_laps = game.NumberLaps,
                        speed_class = game.SpeedClass
                    });
                }
            }

            var resp = new Response<List<GameList>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new GameList {
                    page = page,
                    total = GameList.Count,
                    total_pages = totalPages,
                    Games = gameList
                } ]
            };
            return resp.Serialize();
        }

        public static string CreateGame(Database database, Guid SessionID, Game game, string HostIP)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (game == null || user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            GameList.Add(new GameData
            {
                HostPlayerId = user.UserId,
                HostPlayerIP = HostIP,
                Id = NextId,
                Name = game.name,
                Type = game.game_type,
                SpeedClass = game.speed_class,
                IsRanked = game.is_ranked,
                MaxPlayers = game.max_players,
                MinPlayers = game.min_players,
                LobbyChannelId = game.lobby_channel_id,
                NumberLaps = game.number_laps,
                State = GameState.PENDING,
                Platform = game.platform,
                Track = game.track,
                Privacy = game.privacy,
                Password = game.password,
                TrackGroup = game.track_group,
                Players = []
            });

            var resp = new Response<List<GameCreate>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new GameCreate {
                    id = NextId
                } ]
            };
            NextId++;
            return resp.Serialize();
        }

        public static string LaunchGame(Database database, Guid SessionID, int id)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var game = GameList.FirstOrDefault(match => match.HostPlayerId == user.UserId && match.Id == id);
            if (game != null)
            {
                game.State = GameState.ACTIVE;

                var track = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == game.Track);

                if (track != null)
                    database.PlayerCreationRacesStarted.Add(new PlayerCreationRaceStarted { PlayerCreationId = track.PlayerCreationId, StartedAt = TimeUtils.Now });

                foreach (var player in game.Players)
                {
                    database.OnlineRacesStarted.Add(new RaceStarted
                    {
                        PlayerId = player.PlayerId,
                        StartedAt = TimeUtils.Now,
                    });
                    player.State = GameState.ACTIVE;

                    var Player = database.Users.FirstOrDefault(match => match.UserId == player.PlayerId);
                    if (Player == null)
                    {
                        var character = database.PlayerCreations.FirstOrDefault(match => match.Type == PlayerCreationType.CHARACTER && match.PlayerCreationId == Player.CharacterIdx);
                        var kart = database.PlayerCreations.FirstOrDefault(match => match.Type == PlayerCreationType.KART && match.PlayerCreationId == Player.KartIdx);

                        if (character != null)
                            database.PlayerCreationRacesStarted.Add(new PlayerCreationRaceStarted { PlayerCreationId = character.PlayerCreationId, StartedAt = TimeUtils.Now });
                        if (kart != null)
                            database.PlayerCreationRacesStarted.Add(new PlayerCreationRaceStarted { PlayerCreationId = kart.PlayerCreationId, StartedAt = TimeUtils.Now });
                    }
                }
            }

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse()
            };
            return resp.Serialize();
        }

        public static string CancelGame(Database database, Guid SessionID, int id)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var game = GameList.FirstOrDefault(match => match.HostPlayerId == user.UserId && match.Id == id);
            if (game != null)
                game.State = GameState.CANCELLED;

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse()
            };
            return resp.Serialize();
        }

        public static string JoinGame(Database database, Guid SessionID, int game_id)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var game = GameList.FirstOrDefault(match => match.Id == game_id);
            game?.Players.Add(new GamePlayerData
            {
                PlayerId = user.UserId
            });

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse()
            };
            return resp.Serialize();
        }

        public static string LeaveGame(Database database, Guid SessionID, int game_id)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            user.OnlineDisconnected++;
            database.SaveChanges();

            var game = GameList.FirstOrDefault(match => match.Id == game_id);
            if (game != null)
            {
                var player = game.Players.FirstOrDefault(match => match.PlayerId == user.UserId);
                if (player != null)
                    player.State = GameState.DISCONNECTED;
            }

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse()
            };
            return resp.Serialize();
        }

        public static string RemovePlayer(Database database, Guid SessionID, int game_id)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var game = GameList.FirstOrDefault(match => match.Id == game_id);
            if (game != null)
            {
                game.Players.RemoveAll(match => match.PlayerId == user.UserId);
                if (game.Players.Count == 0)
                    GameList.Remove(game);
            }

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse()
            };
            return resp.Serialize();
        }

        public static string PlayerCheckin(Database database, Guid SessionID, int game_id)
        {
            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse()
            };
            return resp.Serialize();
        }

        public static string PlayerForfeit(Database database, Guid SessionID, int game_id)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            user.OnlineForfeit++;
            database.SaveChanges();

            var game = GameList.FirstOrDefault(match => match.Id == game_id);
            if (game != null)
            {
                var player = game.Players.FirstOrDefault(match => match.PlayerId == user.UserId);
                if (player != null)
                    player.State = GameState.FORFEIT;
            }

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse()
            };
            return resp.Serialize();
        }

        public static string PlayerFinish(Database database, Guid SessionID, int game_id)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var game = GameList.FirstOrDefault(match => match.Id == game_id);
            var player = user != null && game != null ? game.Players.FirstOrDefault(match => match.PlayerId == user.UserId) : null;

            if (user == null || game == null || player == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            bool IsWinner = !game.Players.Any(match => match.HasFinished);

            player.State = GameState.FINISHED;

            database.OnlineRacesFinished.Add(new RaceFinished {
                FinishedAt = TimeUtils.Now,
                IsWinner = IsWinner,
                PlayerId = user.UserId
            });

            var character = database.PlayerCreations.FirstOrDefault(match => match.Type == PlayerCreationType.CHARACTER && match.PlayerCreationId == user.CharacterIdx);
            var kart = database.PlayerCreations.FirstOrDefault(match => match.Type == PlayerCreationType.KART && match.PlayerCreationId == user.KartIdx);

            if (character != null)
            {
                character.RacesFinished++;
                if (IsWinner)
                    character.RacesWon++;
            }

            if (kart != null)
            {
                kart.RacesFinished++;
                if (IsWinner)
                    kart.RacesWon++;
            }

            if (IsWinner)
            {
                user.WinStreak++;
                if (user.WinStreak > user.LongestWinStreak)
                    user.LongestWinStreak = user.WinStreak;
            }
            else user.WinStreak = 0;

            if (game.Players.Count(match => match.HasFinished && match.State != GameState.DISCONNECTED && match.State != GameState.FORFEIT) == game.Players.Count(match => match.State != GameState.DISCONNECTED && match.State != GameState.FORFEIT))
            {
                PlayerCreationData creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == game.Track);
                if (creation != null)
                {
                    creation.RacesFinished++;
                    creation.RacesWon++;
                }    
                game.State = GameState.FINISHED;
            }

            database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse()
            };
            return resp.Serialize();
        }

        public static string PlayerPostStats(Database database, Guid SessionID, int game_id, GamePlayerStats stats)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var game = GameList.FirstOrDefault(match => match.Id == game_id);

            if (user == null || game == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (stats.is_complete)
            {
                var score = database.Scores.FirstOrDefault(match => match.PlayerId == user.UserId
                    && match.SubKeyId == stats.track_idx
                    && match.SubGroupId == (int)game.Type - 10
                    && match.Platform == session.Platform && match.IsMNR == session.IsMNR);

                if (score != null)
                {
                    if (score.FinishTime > stats.finish_time)
                    {
                        score.FinishTime = stats.finish_time;
                        score.UpdatedAt = TimeUtils.Now;
                        score.CharacterIdx = stats.character_idx;
                        score.KartIdx = stats.kart_idx;
                    }
                    if (score.BestLapTime > stats.best_lap_time)
                    {
                        score.BestLapTime = stats.best_lap_time;
                        score.UpdatedAt = TimeUtils.Now;
                        score.CharacterIdx = stats.character_idx;
                        score.KartIdx = stats.kart_idx;
                    }
                }
                else
                {
                    database.Scores.Add(new Score
                    {
                        CreatedAt = TimeUtils.Now,
                        UpdatedAt = TimeUtils.Now,
                        FinishTime = stats.finish_time,
                        Platform = session.Platform,
                        PlayerId = user.UserId,
                        SubGroupId = session.IsMNR ? (int)game.Type - 10 : (int)game.Type,
                        SubKeyId = stats.track_idx,
                        BestLapTime = stats.best_lap_time,
                        KartIdx = stats.kart_idx,
                        CharacterIdx = stats.character_idx,
                        IsMNR = session.IsMNR,
                        GhostCarDataMD5 = "",
                        PlaygroupSize = 0,
                        Points = 0,
                        Latitude = 0,
                        Longitude = 0,
                        LocationTag = null
                    });
                }
            }

            if (stats.kart_idx != user.KartIdx)
                user.KartIdx = stats.kart_idx;
            if (stats.character_idx != user.CharacterIdx)
                user.CharacterIdx = stats.character_idx;
            
            if (stats.longest_drift > user.LongestDrift)
                user.LongestDrift = stats.longest_drift;
            if (stats.longest_hang_time > user.LongestHangTime)
                user.LongestHangTime = stats.longest_hang_time;

            var character = database.PlayerCreations.FirstOrDefault(match => match.Type == PlayerCreationType.CHARACTER && match.PlayerCreationId == user.CharacterIdx);
            var kart = database.PlayerCreations.FirstOrDefault(match => match.Type == PlayerCreationType.KART && match.PlayerCreationId == user.KartIdx);

            if (character != null)
            {
                if (stats.longest_drift > character.LongestDrift)
                    character.LongestDrift = stats.longest_drift;
                if (stats.longest_hang_time > character.LongestHangTime)
                    character.LongestHangTime = stats.longest_hang_time;
                if (stats.best_lap_time > character.BestLapTime)
                    character.BestLapTime = stats.longest_hang_time;
            }

            if (kart != null)
            {
                if (stats.longest_drift > kart.LongestDrift)
                    kart.LongestDrift = stats.longest_drift;
                if (stats.longest_hang_time > kart.LongestHangTime)
                    kart.LongestHangTime = stats.longest_hang_time;
                if (stats.best_lap_time > kart.BestLapTime)
                    kart.BestLapTime = stats.longest_hang_time;
            }

            var Track = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == game.Track);

            if (Track != null)
            {
                if (stats.longest_drift > Track.LongestDrift)
                    Track.LongestDrift = stats.longest_drift;
                if (stats.longest_hang_time > Track.LongestHangTime)
                    Track.LongestHangTime = stats.longest_hang_time;
                if (stats.best_lap_time > Track.BestLapTime)
                    Track.BestLapTime = stats.longest_hang_time;
            }

            database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse()
            };
            return resp.Serialize();
        }
    }
}

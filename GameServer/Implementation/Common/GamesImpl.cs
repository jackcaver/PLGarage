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
using System.Text.RegularExpressions;

namespace GameServer.Implementation.Common
{
    public class GamesImpl
    {
        private static readonly List<GameData> GameList = [];
        private static int NextId = 0;

        public static string GetList(int page, int per_page, Filters filters)
        {
            if (page == 0) page = 1;

            var pageStart = PageCalculator.GetPageStart(page, per_page);
            var pageEnd = PageCalculator.GetPageStart(page, per_page);
            var total = GameList.Count();
            var totalPages = PageCalculator.GetTotalPages(total, per_page);

            var filteredGameList = new List<GameData>(GameList)
                .Where(match => (filters.game_state != null ? match.State == filters.game_state : true) &&
                                (filters.is_ranked != null ? match.IsRanked == filters.is_ranked : true) &&
                                (filters.game_type != null ? match.Type == filters.game_type : true) &&
                                (filters.platform != null ? match.Platform == filters.platform : true) &&
                                (filters.track_group != null ? match.TrackGroup != filters.track_group : true) &&
                                (filters.privacy != null ? match.Privacy != filters.privacy : true) &&
                                (!string.IsNullOrEmpty(filters.speed_class) ? match.SpeedClass != filters.speed_class : true) &&
                                (filters.track != null ? match.Track != filters.track : true) &&
                                (filters.number_laps != null ? match.NumberLaps != filters.number_laps : true))
                .Skip(pageStart)
                .Take(per_page)
                // TODO: AutoMapper
                .ToList();

            var gameList = new List<GameListGame>();

            for (int i = pageStart; i < pageEnd; i++)
            {
                var game = filteredGameList[i];
                if (game != null)
                {
                    gameList.Add(new GameListGame
                    {
                        Id = game.Id,
                        Name = game.Name,
                        HostPlayerId = game.HostPlayerId,
                        HostPlayerIpAddress = game.HostPlayerIP,
                        CurPlayers = game.Players.Count,
                        MaxPlayers = game.MaxPlayers,
                        MinPlayers = game.MinPlayers,
                        GameType = game.Type.ToString(),
                        GameStateId = (int)game.State,
                        IsRanked = game.IsRanked,
                        Track = game.Track,
                        LobbyChannelId = game.LobbyChannelId,
                        NumberLaps = game.NumberLaps,
                        SpeedClass = game.SpeedClass
                    });
                }
            }

            var resp = new Response<List<GameList>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new GameList {
                    Page = page,
                    Total = total,
                    TotalPages = totalPages,
                    Games = gameList
                } ]
            };
            return resp.Serialize();
        }

        public static string CreateGame(Database database, Guid SessionID, Game game, string HostIP)
        {
            var session = SessionImpl.GetSession(SessionID);
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
                    Id = NextId
                } ]
            };
            NextId++;
            return resp.Serialize();
        }

        public static string LaunchGame(Database database, Guid SessionID, int id)
        {
            var session = SessionImpl.GetSession(SessionID);
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

                var track = database.PlayerCreations.FirstOrDefault(match => match.Id == game.Track);

                if (track != null)
                    database.PlayerCreationRacesStarted.Add(new PlayerCreationRaceStarted { Creation = track, StartedAt = DateTime.UtcNow });

                foreach (var gamePlayer in game.Players)
                {
                    var player = database.Users.FirstOrDefault(match => match.UserId == gamePlayer.PlayerId);
                    if (player == null)
                    {
                        database.OnlineRacesStarted.Add(new RaceStarted
                        {
                            Player = player,
                            StartedAt = DateTime.UtcNow,
                        });
                        gamePlayer.State = GameState.ACTIVE;

                        var character = database.PlayerCreations.FirstOrDefault(match => match.Type == PlayerCreationType.CHARACTER && match.Id == player.CharacterIdx);
                        var kart = database.PlayerCreations.FirstOrDefault(match => match.Type == PlayerCreationType.KART && match.Id == player.KartIdx);

                        if (character != null)
                            database.PlayerCreationRacesStarted.Add(new PlayerCreationRaceStarted { Creation = character, StartedAt = DateTime.UtcNow });
                        if (kart != null)
                            database.PlayerCreationRacesStarted.Add(new PlayerCreationRaceStarted { Creation = kart, StartedAt = DateTime.UtcNow });
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
            var session = SessionImpl.GetSession(SessionID);
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
            var session = SessionImpl.GetSession(SessionID);
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
            var session = SessionImpl.GetSession(SessionID);
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
            var session = SessionImpl.GetSession(SessionID);
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
            var session = SessionImpl.GetSession(SessionID);
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
            var session = SessionImpl.GetSession(SessionID);
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
                FinishedAt = DateTime.UtcNow,
                IsWinner = IsWinner,
                Player = user
            });

            var character = database.PlayerCreations.FirstOrDefault(match => match.Type == PlayerCreationType.CHARACTER && match.Id == user.CharacterIdx);
            var kart = database.PlayerCreations.FirstOrDefault(match => match.Type == PlayerCreationType.KART && match.Id == user.KartIdx);

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

            if (game.Players.Where(match => match.State != GameState.DISCONNECTED && match.State != GameState.FORFEIT).All(match => match.HasFinished))
            {
                var creation = database.PlayerCreations.FirstOrDefault(match => match.Id == game.Track);
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
            var session = SessionImpl.GetSession(SessionID);
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
                var track = database.PlayerCreations.FirstOrDefault(match => match.Id == stats.track_idx);
                var score = database.Scores.FirstOrDefault(match => match.User.UserId == user.UserId
                    && match.Creation == track  // TODO: Will this work?
                    && match.SubGroupId == (int)game.Type - 10
                    && match.Platform == session.Platform && match.IsMNR == session.IsMNR);

                if (score != null)
                {
                    if (score.FinishTime > stats.finish_time)
                    {
                        score.FinishTime = stats.finish_time;
                        score.UpdatedAt = DateTime.UtcNow;
                        score.CharacterIdx = stats.character_idx;
                        score.KartIdx = stats.kart_idx;
                    }
                    if (score.BestLapTime > stats.best_lap_time)
                    {
                        score.BestLapTime = stats.best_lap_time;
                        score.UpdatedAt = DateTime.UtcNow;
                        score.CharacterIdx = stats.character_idx;
                        score.KartIdx = stats.kart_idx;
                    }
                }
                else
                {
                    database.Scores.Add(new Score
                    {
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        FinishTime = stats.finish_time,
                        Platform = session.Platform,
                        User = user,
                        SubGroupId = session.IsMNR ? (int)game.Type - 10 : (int)game.Type,
                        Creation = track,
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

            var character = database.PlayerCreations.FirstOrDefault(match => match.Type == PlayerCreationType.CHARACTER && match.Id == user.CharacterIdx);
            var kart = database.PlayerCreations.FirstOrDefault(match => match.Type == PlayerCreationType.KART && match.Id == user.KartIdx);

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

            var Track = database.PlayerCreations.FirstOrDefault(match => match.Id == game.Track);

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

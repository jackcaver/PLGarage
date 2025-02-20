using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using GameServer.Implementation.Common;
using GameServer.Models;
using GameServer.Models.Common;
using GameServer.Models.GameBrowser;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Common
{
    public class GamesController : Controller
    {
        private readonly Database database;

        public GamesController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("/resources/single_player_game.create_finish_and_post_stats.xml")]
        public IActionResult GetSinglePlayerXML()
        { //Because for whatever reason MNR: Road Trip Refuses to take my xml with LBPK variables in it -_-
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            var session = SessionImpl.GetSession(SessionID);
            string resp;
            if (session.IsMNR && System.IO.File.Exists("GameResources/MNR.single_player_game.create_finish_and_post_stats.xml"))
                resp = System.IO.File.ReadAllText("GameResources/MNR.single_player_game.create_finish_and_post_stats.xml");
            else if (System.IO.File.Exists("GameResources/single_player_game.create_finish_and_post_stats.xml"))
                resp = System.IO.File.ReadAllText("GameResources/single_player_game.create_finish_and_post_stats.xml");
            else
                return NotFound();
            return Content(resp, "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("single_player_games/create_finish_and_post_stats.xml")]
        public IActionResult PostSinglePlayerGameStats(Game game, GamePlayer game_player, GamePlayerStats game_player_stats)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            var Track = database.PlayerCreations.FirstOrDefault(match => match.Id == game.track_idx);
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            string FormScore = Request.Form["game_player_stats[score]"];
            string FormFinishTime = Request.Form["game_player_stats[finish_time]"];
            string FormPoints = Request.Form["game_player_stats[points]"];
            string FormVolatility = Request.Form["game_player_stats[volatility]"];
            string FormDeviation = Request.Form["game_player_stats[deviation]"];
            string FormBestLapTime = Request.Form["game_player_stats[best_lap_time]"];
            string FormLongestDrift = Request.Form["game_player_stats[longest_drift]"];
            string FormLongestHangTime = Request.Form["game_player_stats[longest_hang_time]"];
            string FormLatitude = Request.Form["game_player_stats[latitude]"];
            string FormLongitude = Request.Form["game_player_stats[longitude]"];

            if (session.IsMNR)
            {
                Track = database.PlayerCreations.FirstOrDefault(match => match.Id == game_player_stats.track_idx);
                game.host_player_id = user.UserId;
                game.track_idx = game_player_stats.track_idx;
            }
            UserGeneratedContentUtils.AddStoryLevel(database, game.track_idx);

            if (Track == null || user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            if (FormScore != null)
                game_player_stats.score = float.Parse(FormScore, CultureInfo.InvariantCulture.NumberFormat);
            if (FormFinishTime != null)
                game_player_stats.finish_time = float.Parse(FormFinishTime, CultureInfo.InvariantCulture.NumberFormat);
            if (FormPoints != null)
                game_player_stats.points = float.Parse(FormPoints, CultureInfo.InvariantCulture.NumberFormat);
            if (FormVolatility != null)
                game_player_stats.volatility = float.Parse(FormVolatility, CultureInfo.InvariantCulture.NumberFormat);
            if (FormDeviation != null)
                game_player_stats.deviation = float.Parse(FormDeviation, CultureInfo.InvariantCulture.NumberFormat);
            if (FormBestLapTime != null)
                game_player_stats.best_lap_time = float.Parse(FormBestLapTime, CultureInfo.InvariantCulture.NumberFormat);
            if (FormLongestDrift != null)
                game_player_stats.longest_drift = float.Parse(FormLongestDrift, CultureInfo.InvariantCulture.NumberFormat);
            if (FormLongestHangTime != null)
                game_player_stats.longest_hang_time = float.Parse(FormLongestHangTime, CultureInfo.InvariantCulture.NumberFormat);
            if (FormLatitude != null)
                game_player_stats.latitude = float.Parse(FormLatitude, CultureInfo.InvariantCulture.NumberFormat);
            if (FormLongitude != null)
                game_player_stats.longitude = float.Parse(FormLongitude, CultureInfo.InvariantCulture.NumberFormat);
            if (session.IsMNR)
                game_player_stats.ghost_car_data = Request.Form.Files.GetFile("game_player_stats[ghost_car_data]");

            if (FormLongitude != null && FormLatitude != null)
            {
                //gps isn't 100% accurate so here is my way to get around it
                game_player_stats.latitude = float.Parse(game_player_stats.latitude.ToString("0.000", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
                game_player_stats.longitude = float.Parse(game_player_stats.longitude.ToString("0.000", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
            }

            Score score;

            if(session.IsMNR && session.Platform == Platform.PS3 && user.CharacterIdx != game_player_stats.character_idx)
                user.CharacterIdx = game_player_stats.character_idx;

            if (session.IsMNR && session.Platform == Platform.PS3 && user.KartIdx != game_player_stats.kart_idx)
                user.KartIdx = game_player_stats.kart_idx;

            if (user.LongestDrift < game_player_stats.longest_drift)
                user.LongestDrift = game_player_stats.longest_drift;

            if (user.LongestHangTime < game_player_stats.longest_hang_time)
                user.LongestHangTime = game_player_stats.longest_hang_time;

            if (!session.IsMNR)
            {
                score = database.Scores.FirstOrDefault(match => match.User.UserId == game.host_player_id
                    && match.Creation.Id == game.track_idx
                    && match.SubGroupId == (int)game.game_type
                    && match.Platform == game.platform
                    && match.PlaygroupSize == game_player_stats.playgroup_size && match.IsMNR == session.IsMNR);
            }
            else if (session.Platform == Platform.PSV)
            {
                score = database.Scores.FirstOrDefault(match => match.User.UserId == game.host_player_id
                    && match.Creation.Id == game_player_stats.track_idx
                    && match.SubGroupId == (int)game.game_type - 10
                    && match.Platform == session.Platform && match.IsMNR == session.IsMNR
                    && match.Latitude.ToString("0.000", CultureInfo.InvariantCulture) == game_player_stats.latitude.ToString("0.000", CultureInfo.InvariantCulture)
                    && match.Longitude.ToString("0.000", CultureInfo.InvariantCulture) == game_player_stats.longitude.ToString("0.000", CultureInfo.InvariantCulture));
                if (score == null)
                {
                    score = database.Scores.FirstOrDefault(match => match.User.UserId == game.host_player_id
                        && match.Creation.Id == game_player_stats.track_idx
                        && match.SubGroupId == (int)game.game_type - 10
                        && match.Platform == session.Platform && match.IsMNR == session.IsMNR
                        && match.LocationTag == null);
                }
            }
            else
            {
                score = database.Scores.FirstOrDefault(match => match.User.UserId == game.host_player_id
                    && match.Creation.Id == game_player_stats.track_idx
                    && match.SubGroupId == (int)game.game_type - 10
                    && match.Platform == session.Platform && match.IsMNR == session.IsMNR);
            }

            database.PlayerCreationRacesStarted.Add(new PlayerCreationRaceStarted { Creation = score.Creation /* I think this is correct? TODO */, StartedAt = DateTime.UtcNow });
            Track.RacesFinished++;
            if (game_player_stats.is_winner == 1)
                Track.RacesWon++;

            if (session.IsMNR)
            {
                var character = database.PlayerCreations.FirstOrDefault(match => match.Id == game_player_stats.character_idx && match.Type == PlayerCreationType.CHARACTER);
                var kart = database.PlayerCreations.FirstOrDefault(match => match.Id == game_player_stats.kart_idx && match.Type == PlayerCreationType.KART);
                
                if (character != null)
                {
                    database.PlayerCreationRacesStarted.Add(new PlayerCreationRaceStarted { Creation = character });
                    character.RacesFinished++;
                    if (game_player_stats.is_winner == 1)
                        character.RacesWon++;

                    if (game_player_stats.longest_drift > character.LongestDrift)
                        character.LongestDrift = game_player_stats.longest_drift;
                    if (game_player_stats.longest_hang_time > character.LongestHangTime)
                        character.LongestHangTime = game_player_stats.longest_hang_time;
                    if (game_player_stats.best_lap_time > character.BestLapTime)
                        character.BestLapTime = game_player_stats.longest_hang_time;
                }

                if (kart != null)
                {
                    database.PlayerCreationRacesStarted.Add(new PlayerCreationRaceStarted { Creation = kart });
                    kart.RacesFinished++;
                    if (game_player_stats.is_winner == 1)
                        kart.RacesWon++;

                    if (game_player_stats.longest_drift > kart.LongestDrift)
                        kart.LongestDrift = game_player_stats.longest_drift;
                    if (game_player_stats.longest_hang_time > kart.LongestHangTime)
                        kart.LongestHangTime = game_player_stats.longest_hang_time;
                    if (game_player_stats.best_lap_time > kart.BestLapTime)
                        kart.BestLapTime = game_player_stats.longest_hang_time;
                }

                if (game_player_stats.longest_drift > Track.LongestDrift)
                    Track.LongestDrift = game_player_stats.longest_drift;
                if (game_player_stats.longest_hang_time > Track.LongestHangTime)
                    Track.LongestHangTime = game_player_stats.longest_hang_time;
                if (game_player_stats.best_lap_time > Track.BestLapTime)
                    Track.BestLapTime = game_player_stats.longest_hang_time;
            }

            var leaderboardQuery = database.Scores
                .Where(match => match.Creation.Id == game.track_idx && match.SubGroupId == (int)game.game_type &&
                    match.PlaygroupSize == game_player_stats.playgroup_size && match.Platform == game.platform);

            if (Track.ScoreboardMode == 1)
                leaderboardQuery = leaderboardQuery.OrderBy(match => match.FinishTime);
            else
                leaderboardQuery = leaderboardQuery.OrderByDescending(match => match.Points);

            var leaderboard = leaderboardQuery.ToList();

            if (leaderboard != null && !session.IsMNR)
            {
                var FastestTime = leaderboard.FirstOrDefault();
                var HighScore = leaderboard.FirstOrDefault();
                if (Track.ScoreboardMode == 1 && FastestTime != null && FastestTime.FinishTime > game_player_stats.finish_time)
                {
                    database.ActivityLog.Add(new ActivityEvent
                    {
                        Author = user,
                        Type = ActivityType.player_event,
                        List = ActivityList.both,
                        Topic = "player_beat_finish_time",
                        Description = $"{game_player_stats.finish_time}",
                        Creation = Track,
                        CreatedAt = DateTime.UtcNow,
                        AllusionId = Track.Id,
                        AllusionType = "PlayerCreation::Track"
                    });
                }
                if (Track.ScoreboardMode == 0 && HighScore != null && HighScore.Points < game_player_stats.score)
                {
                    database.ActivityLog.Add(new ActivityEvent
                    {
                        Author = user,
                        Type = ActivityType.player_event,
                        List = ActivityList.both,
                        Topic = "player_beat_score",
                        Description = $"{game_player_stats.score}",
                        Creation = Track,
                        CreatedAt = DateTime.UtcNow,
                        AllusionId = Track.Id,
                        AllusionType = "PlayerCreation::Track"
                    });
                }
            }
            string GhostDataMD5 = "";
            MemoryStream GhostData = new();
            if (session.IsMNR)
            {
                game_player_stats.ghost_car_data.OpenReadStream().CopyTo(GhostData);
                GhostData.Position = 0;
                GhostDataMD5 = UserGeneratedContentUtils.CalculateGhostCarDataMD5(GhostData);
                GhostData.Position = 0;
            }
            bool SaveGhost = false;

            if (score != null)
            {               
                if (score.FinishTime > game_player_stats.finish_time)
                {
                    score.FinishTime = game_player_stats.finish_time;
                    score.UpdatedAt = DateTime.UtcNow;
                    score.CharacterIdx = game_player_stats.character_idx;
                    score.KartIdx = game_player_stats.kart_idx;
                    SaveGhost = true;
                }
                if (score.Points < game_player_stats.score)
                {
                    score.Points = game_player_stats.score;
                    score.UpdatedAt = DateTime.UtcNow;
                }
                if (score.BestLapTime > game_player_stats.best_lap_time)
                {
                    score.BestLapTime = game_player_stats.best_lap_time;
                    score.UpdatedAt = DateTime.UtcNow;
                    score.CharacterIdx = game_player_stats.character_idx;
                    score.KartIdx = game_player_stats.kart_idx;
                    SaveGhost = true;
                }
                if (SaveGhost)
                    score.GhostCarDataMD5 = GhostDataMD5;
                if (session.Platform == Platform.PSV && SaveGhost)
                {
                    score.Latitude = game_player_stats.latitude;
                    score.Longitude = game_player_stats.longitude;
                    score.LocationTag = game_player_stats.location_tag;
                }
            }
            else
            {
                database.Scores.Add(new Score
                {
                    CreatedAt = DateTime.UtcNow,
                    FinishTime = game_player_stats.finish_time,
                    Platform = session.Platform,
                    User = user,    // TODO: Will this be reliable for LBPK?
                    PlaygroupSize = game_player_stats.playgroup_size,
                    Points = game_player_stats.score,
                    SubGroupId = session.IsMNR ? (int)game.game_type - 10 : (int)game.game_type,
                    Creation = score.Creation,  // TODO: Check
                    UpdatedAt = DateTime.UtcNow,
                    Latitude = game_player_stats.latitude,
                    Longitude = game_player_stats.longitude,
                    BestLapTime = game_player_stats.best_lap_time,
                    KartIdx = game_player_stats.kart_idx,
                    CharacterIdx = game_player_stats.character_idx,
                    GhostCarDataMD5 = GhostDataMD5,
                    IsMNR = session.IsMNR,
                    LocationTag = game_player_stats.location_tag
                });
                SaveGhost = true;
            }
            if (session.IsMNR && SaveGhost)
            {
                GhostData.Position = 0;
                UserGeneratedContentUtils.SaveGhostCarData(game.game_type, session.Platform,
                    game_player_stats.track_idx, database.Users.FirstOrDefault(match => match.Username == session.Username).UserId,
                    GhostData);
            }
            database.SaveChanges();

            var resp = new Response<List<GameResponse>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new GameResponse { // TODO: Does the game use the below?
                    Id = 0,
                    GamePlayerId = 0,
                    GamePlayerStatsId = 0
                } ]
            };

            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("/multiplayer_games.xml")]
        public IActionResult GetGameList(int page, int per_page, Filters filters)
        {
            if (Request.Query.ContainsKey("filters[game_state]"))
            {
                GameState StateFilter = GameState.PENDING;
                Enum.TryParse(Request.Query["filters[game_state]"], out StateFilter);
                filters.game_state = StateFilter;
            }
            if (Request.Query.ContainsKey("filters[game_type]"))
            {
                GameType TypeFilter = GameType.ONLINE_PURE_RACE;
                Enum.TryParse(Request.Query["filters[game_type]"], out TypeFilter);
                filters.game_type = TypeFilter;
            }
            if (Request.Query.ContainsKey("filters[platform]"))
            {
                Platform PlatformFilter = Platform.PSP;
                Enum.TryParse(Request.Query["filters[platform]"], out PlatformFilter);
                filters.platform = PlatformFilter;
            }
            if (Request.Query.ContainsKey("filters[track_group]"))
                filters.track_group = Request.Query["filters[track_group]"];
            if (Request.Query.ContainsKey("filters[is_ranked]"))
                filters.is_ranked = bool.Parse(Request.Query["filters[is_ranked]"]);
            if (Request.Query.ContainsKey("filters[speed_class]"))
                filters.speed_class = Request.Query["filters[speed_class]"];
            if (Request.Query.ContainsKey("filters[privacy]"))
                filters.privacy = Request.Query["filters[privacy]"];
            if (Request.Query.ContainsKey("filters[track]"))
                filters.track = int.Parse(Request.Query["filters[track]"]);
            if (Request.Query.ContainsKey("filters[number_laps]"))
                filters.number_laps = int.Parse(Request.Query["filters[number_laps]"]);
            return Content(GamesImpl.GetList(page, per_page, filters), "application/xml;charset=utf-8");
        }

        [HttpPut]
        [Route("/multiplayer_games.xml")]
        public IActionResult CreateGame(Game game)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            var HostIP = HttpContext.Connection.RemoteIpAddress.ToString();
            return Content(GamesImpl.CreateGame(database, SessionID, game, HostIP), "application/xml;charset=utf-8");
        }

        [HttpPut]
        [Route("/multiplayer_games/{id}.xml")]
        public IActionResult LaunchGame(int id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(GamesImpl.LaunchGame(database, SessionID, id), "application/xml;charset=utf-8");
        }

        [HttpDelete]
        [Route("/multiplayer_games/{id}.xml")]
        public IActionResult CancelGame(int id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(GamesImpl.CancelGame(database, SessionID, id), "application/xml;charset=utf-8");
        }

        [HttpPut]
        [Route("/multiplayer_games/{game_id}/join.xml")]
        public IActionResult JoinGame(int game_id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(GamesImpl.JoinGame(database, SessionID, game_id), "application/xml;charset=utf-8");
        }

        [HttpDelete]
        [Route("/multiplayer_games/{game_id}/players.xml")]
        public IActionResult RemovePlayer(int game_id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(GamesImpl.RemovePlayer(database, SessionID, game_id), "application/xml;charset=utf-8");
        }

        [HttpPut]
        [Route("/multiplayer_games/{game_id}/disconnect.xml")]
        public IActionResult LeaveGame(int game_id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(GamesImpl.LeaveGame(database, SessionID, game_id), "application/xml;charset=utf-8");
        }

        [HttpPut]
        [Route("/multiplayer_games/{game_id}/checkin.xml")]
        public IActionResult PlayerCheckin(int game_id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(GamesImpl.PlayerCheckin(database, SessionID, game_id), "application/xml;charset=utf-8");
        }

        [HttpPut]
        [Route("/multiplayer_games/{game_id}/forfeit.xml")]
        public IActionResult PlayerForfeit(int game_id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(GamesImpl.PlayerForfeit(database, SessionID, game_id), "application/xml;charset=utf-8");
        }

        [HttpPut]
        [Route("/multiplayer_games/{game_id}/finish.xml")]
        public IActionResult PlayerFinish(int game_id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(GamesImpl.PlayerFinish(database, SessionID, game_id), "application/xml;charset=utf-8");
        }

        [HttpPut]
        [Route("/multiplayer_games/{game_id}/stats.xml")]
        public IActionResult PlayerPostStats(int game_id, GamePlayerStats stats)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(GamesImpl.PlayerPostStats(database, SessionID, game_id, stats), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
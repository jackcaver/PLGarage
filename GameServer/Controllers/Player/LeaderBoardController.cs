using GameServer.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GameServer.Models.Request;
using GameServer.Models.PlayerData;
using GameServer.Implementation.Player;
using System;
using GameServer.Implementation.Common;
using GameServer.Models.GameBrowser;

namespace GameServer.Controllers.Player
{
    public class LeaderBoardController : Controller
    {
        private readonly Database database;

        public LeaderBoardController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("leaderboards/view.xml")]
        public IActionResult View(LeaderboardType type, GameType game_type, Platform? platform, int page, int per_page,
            int column_page, int cols_per_page, SortColumn sort_column, SortOrder sort_order, int limit)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);

            var session = Session.GetSession(SessionID);
            if (platform == null)
                platform = session.Platform;
            return Content(LeaderBoards.ViewLeaderBoard(database, SessionID, type, game_type, platform.Value, page, per_page, column_page, 
                cols_per_page, sort_column, sort_order, limit), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("leaderboards/friends_view.xml")]
        public IActionResult FriendsView(LeaderboardType type, GameType game_type, Platform? platform, int page, int per_page,
            int column_page, int cols_per_page, SortColumn sort_column, SortOrder sort_order, int limit)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);

            var session = Session.GetSession(SessionID);
            if (platform == null)
                platform = session.Platform;
            return Content(LeaderBoards.ViewLeaderBoard(database, SessionID, type, game_type, platform.Value, page, per_page, column_page,
                cols_per_page, sort_column, sort_order, limit, Request.Query["filters[username]"], true), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("sub_leaderboards/view.xml")]
        public IActionResult View(int sub_group_id, int sub_key_id, LeaderboardType type, Platform platform, int page, int per_page,
            int column_page, int cols_per_page, SortColumn sort_column, SortOrder sort_order, int? num_above_below, int limit, int playgroup_size, 
            float? latitude, float? longitude)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(LeaderBoards.ViewSubLeaderBoard(database, SessionID, sub_group_id, 
                sub_key_id, type, platform, page, per_page, column_page, cols_per_page, sort_column, 
                sort_order, num_above_below, limit, playgroup_size, latitude, longitude), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("sub_leaderboards/around_me.xml")]
        public IActionResult AroundMe(int sub_group_id, int sub_key_id, LeaderboardType type, Platform platform,
            int column_page, int cols_per_page, SortColumn sort_column, SortOrder sort_order, int num_above_below, int playgroup_size, int limit)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(LeaderBoards.ViewSubLeaderBoardAroundMe(database, SessionID, sub_group_id, sub_key_id, type, platform, 
                column_page, cols_per_page, sort_column, sort_order, num_above_below, playgroup_size, limit), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("sub_leaderboards/friends_view.xml")]
        public IActionResult FriendsView(int sub_group_id, int sub_key_id, LeaderboardType type, Platform platform, int page, int per_page,
            int column_page, int cols_per_page, SortColumn sort_column, SortOrder sort_order, int? num_above_below, int limit, 
            int playgroup_size, float? latitude, float? longitude)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(LeaderBoards.ViewSubLeaderBoard(database, SessionID, sub_group_id,
                sub_key_id, type, platform, page, per_page, column_page, cols_per_page, sort_column,
                sort_order, num_above_below, limit, playgroup_size, latitude, longitude, Request.Query["filters[username]"], true), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("sub_leaderboards/personal.xml")]
        public IActionResult PersonalView(int limit, int page, int per_page, LeaderboardType type, int sub_group_id, 
            int sub_key_id, Platform platform, Platform track_platform, SortOrder sort_order, SortColumn sort_column, 
            float longitude, float latitude)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(LeaderBoards.ViewPersonalSubLeaderBoard(database, SessionID, limit, page, per_page, type,
                sub_group_id, sub_key_id, platform, track_platform, sort_order, sort_column, longitude, latitude), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("ghost_car_data/{gameType}/{platform}/{track_id}/{player_id}/data.bin")]
        public IActionResult GetGhostCarDataOnPlatform(GameType gameType, Platform platform, int track_id, int player_id)
        {
            var data = UserGeneratedContentUtils.LoadGhostCarData(gameType, Platform.PSV, track_id, player_id);
            Response.Headers.Append("ETag", $"\"{UserGeneratedContentUtils.CalculateGhostCarDataMD5(gameType, Platform.PSV, track_id, player_id)}\"");
            Response.Headers.Append("Accept-Ranges", "bytes");
            if (data == null)
                return NotFound();
            return File(data, "application/octet-stream");
        }

        [HttpGet]
        [Route("ghost_car_data/{gameType}/{track_id}/{player_id}/data.bin")]
        public IActionResult GetGhostCarData(GameType gameType, int track_id, int player_id)
        {
            var data = UserGeneratedContentUtils.LoadGhostCarData(gameType, Platform.PS3, track_id, player_id);
            Response.Headers.Append("ETag", $"\"{UserGeneratedContentUtils.CalculateGhostCarDataMD5(gameType, Platform.PS3, track_id, player_id)}\"");
            Response.Headers.Append("Accept-Ranges", "bytes");
            if (data == null)
                return NotFound();
            return File(data, "application/octet-stream");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}

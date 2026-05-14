using GameServer.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GameServer.Models.Request;
using GameServer.Models.PlayerData;
using GameServer.Implementation.Player;
using GameServer.Implementation.Common;
using GameServer.Models;
using GameServer.Models.GameBrowser;
using Microsoft.AspNetCore.Authorization;

namespace GameServer.Controllers.Player
{
    public class LeaderBoardController(Database database, IUGCStorage storage) : Controller
    {
        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("leaderboards/view.xml")]
        public IActionResult View(LeaderboardType type, GameType game_type, Platform? platform, int page, int per_page,
            int column_page, int cols_per_page, SortColumn sort_column, SortOrder sort_order, int limit)
        {
            var session = Session.GetSession(database, User);
            if (platform == null)
                platform = session.Platform;
            return Content(LeaderBoards.ViewLeaderBoard(database, session, type, game_type, platform.Value, page, per_page, column_page, 
                cols_per_page, sort_column, sort_order, limit), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("leaderboards/friends_view.xml")]
        public IActionResult FriendsView(LeaderboardType type, GameType game_type, Platform? platform, int page, int per_page,
            int column_page, int cols_per_page, SortColumn sort_column, SortOrder sort_order, int limit)
        {
            var session = Session.GetSession(database, User);
            if (platform == null)
                platform = session.Platform;
            return Content(LeaderBoards.ViewLeaderBoard(database, session, type, game_type, platform.Value, page, per_page, column_page,
                cols_per_page, sort_column, sort_order, limit, Request.Query["filters[username]"], true), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("sub_leaderboards/view.xml")]
        public IActionResult View(int sub_group_id, int sub_key_id, LeaderboardType type, Platform platform, int page, int per_page,
            int column_page, int cols_per_page, SortColumn sort_column, SortOrder sort_order, int? num_above_below, int limit, int playgroup_size, 
            float? latitude, float? longitude)
        {
            var session = Session.GetSession(database, User);
            return Content(LeaderBoards.ViewSubLeaderBoard(database, session, sub_group_id, 
                sub_key_id, type, platform, page, per_page, column_page, cols_per_page, sort_column, 
                sort_order, num_above_below, limit, playgroup_size, latitude, longitude), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [Route("sub_leaderboards/around_me.xml")]
        public IActionResult AroundMe(int sub_group_id, int sub_key_id, LeaderboardType type, Platform platform,
            int column_page, int cols_per_page, SortColumn sort_column, SortOrder sort_order, int num_above_below, int playgroup_size, int limit)
        {
            var session = Session.GetSession(database, User);
            return Content(LeaderBoards.ViewSubLeaderBoardAroundMe(database, session, sub_group_id, sub_key_id, type, platform, 
                column_page, cols_per_page, sort_column, sort_order, num_above_below, playgroup_size, limit), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("sub_leaderboards/friends_view.xml")]
        public IActionResult FriendsView(int sub_group_id, int sub_key_id, LeaderboardType type, Platform platform, int page, int per_page,
            int column_page, int cols_per_page, SortColumn sort_column, SortOrder sort_order, int? num_above_below, int limit, 
            int playgroup_size, float? latitude, float? longitude)
        {
            var session = Session.GetSession(database, User);
            return Content(LeaderBoards.ViewSubLeaderBoard(database, session, sub_group_id,
                sub_key_id, type, platform, page, per_page, column_page, cols_per_page, sort_column,
                sort_order, num_above_below, limit, playgroup_size, latitude, longitude, Request.Query["filters[username]"], true), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [Route("sub_leaderboards/personal.xml")]
        public IActionResult PersonalView(int limit, int page, int per_page, LeaderboardType type, int sub_group_id, 
            int sub_key_id, Platform platform, Platform track_platform, SortOrder sort_order, SortColumn sort_column, 
            float longitude, float latitude)
        {
            var session = Session.GetSession(database, User);
            return Content(LeaderBoards.ViewPersonalSubLeaderBoard(database, session, limit, page, per_page, type,
                sub_group_id, sub_key_id, platform, track_platform, sort_order, sort_column, longitude, latitude), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("ghost_car_data/{gameType}/{platform}/{track_id}/{player_id}/data.bin")]
        public IActionResult GetGhostCarDataOnPlatform(GameType gameType, Platform platform, int track_id, int player_id)
        {
            var data = storage.LoadGhostCarData(gameType, platform, track_id, player_id);
            if (data == null)
                return NotFound();
            Response.Headers.Append("ETag", $"\"{UserGeneratedContentUtils.CalculateMD5(data)}\"");
            Response.Headers.Append("Accept-Ranges", "bytes");
            return File(data, "application/octet-stream");
        }

        [HttpGet]
        [Route("ghost_car_data/{gameType}/{track_id}/{player_id}/data.bin")]
        public IActionResult GetGhostCarData(GameType gameType, int track_id, int player_id)
        {
            var data = storage.LoadGhostCarData(gameType, Platform.PS3, track_id, player_id);
            if (data == null)
                return NotFound();
            Response.Headers.Append("ETag", $"\"{UserGeneratedContentUtils.CalculateMD5(data)}\"");
            Response.Headers.Append("Accept-Ranges", "bytes");
            return File(data, "application/octet-stream");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}

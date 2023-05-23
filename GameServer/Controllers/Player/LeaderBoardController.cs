using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using GameServer.Models.Request;
using GameServer.Models.PlayerData;
using GameServer.Implementation.Player;
using System;

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
        [Route("sub_leaderboards/view.xml")]
        public IActionResult View(int sub_group_id, int sub_key_id, LeaderboardType type, Platform platform, int page, int per_page,
            int column_page, int cols_per_page, SortColumn sort_column, SortOrder sort_order, int? num_above_below, int limit, int playgroup_size)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(LeaderBoards.ViewSubLeaderBoard(database, SessionID, sub_group_id, 
                sub_key_id, type, platform, page, per_page, column_page, cols_per_page, sort_column, 
                sort_order, num_above_below, limit, playgroup_size), "application/xml;charset=utf-8");
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
            int column_page, int cols_per_page, SortColumn sort_column, SortOrder sort_order, int? num_above_below, int limit, int playgroup_size)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(LeaderBoards.ViewSubLeaderBoard(database, SessionID, sub_group_id,
                sub_key_id, type, platform, page, per_page, column_page, cols_per_page, sort_column,
                sort_order, num_above_below, limit, playgroup_size, Request.Query["filters[username]"], true), "application/xml;charset=utf-8");
        }
    }
}

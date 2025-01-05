using GameServer.Implementation.Player;
using GameServer.Models.Common;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GameServer.Controllers.Player
{
    public class PlayerCommentsController : Controller
    {
        private readonly Database database;

        public PlayerCommentsController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("player_comments.xml")]
        public IActionResult GetComments(int page, int per_page, SortColumn sort_column, SortOrder sort_order, int limit, Platform platform)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCommentsImpl.ListComments(database, SessionID, page, per_page, limit,
                sort_column, platform, Request.Query["filters[player_id]"], Request.Query["filters[author_id]"]), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_comments.xml")]
        public IActionResult Create(PlayerComment player_comment)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCommentsImpl.CreateComment(database, SessionID, player_comment), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_comments/{id}.xml")]
        public IActionResult Delete(int id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCommentsImpl.RemoveComment(database, SessionID, id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_comment_ratings.xml")]
        public IActionResult Rate(PlayerCommentRating player_comment_rating)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCommentsImpl.RateComment(database, SessionID, player_comment_rating), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
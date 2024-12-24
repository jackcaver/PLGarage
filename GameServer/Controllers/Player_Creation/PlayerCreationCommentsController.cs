using GameServer.Implementation.Player_Creation;
using GameServer.Models.Common;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GameServer.Controllers.Player_Creation
{
    public class PlayerCreationCommentsController : Controller
    {
        private readonly Database database;

        public PlayerCreationCommentsController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("player_creation_comments.xml")]
        public IActionResult GetComments(int page, int per_page, SortColumn sort_column, SortOrder sort_order, int limit, Platform platform)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreationComments.ListComments(database, SessionID, page, per_page, sort_column, sort_order,
                limit, platform, Request.Query["filters[player_creation_id]"], Request.Query["filters[player_id]"]), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_comments.xml")]
        public IActionResult Create(PlayerCreationComment player_creation_comment)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreationComments.CreateComment(database, SessionID, player_creation_comment), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_comments/{id}.xml")]
        public IActionResult Delete(int id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreationComments.DeleteComment(database, SessionID, id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_comment_ratings.xml")]
        public IActionResult Rate(PlayerCreationCommentRating player_creation_comment_rating)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreationComments.RateComment(database, SessionID, player_creation_comment_rating), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
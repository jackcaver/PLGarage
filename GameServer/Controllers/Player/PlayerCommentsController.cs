using GameServer.Implementation.Common;
using GameServer.Implementation.Player;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player
{
    public class PlayerCommentsController(Database database) : Controller
    {
        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("player_comments.xml")]
        public IActionResult GetComments(int page, int per_page, SortColumn sort_column, SortOrder sort_order, int limit, Platform platform)
        {
            var user = Session.GetUser(database, User);
            return Content(PlayerComments.ListComments(database, user, page, per_page, limit,
                sort_column, platform, Request.Query["filters[player_id]"], Request.Query["filters[author_id]"]), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("player_comments.xml")]
        public IActionResult Create(PlayerComment player_comment)
        {
            var session = Session.GetSession(database, User);
            return Content(PlayerComments.CreateComment(database, session, player_comment), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("player_comments/{id}.xml")]
        public IActionResult Delete(int id)
        {
            var user = Session.GetUser(database, User);
            return Content(PlayerComments.RemoveComment(database, user, id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("player_comment_ratings.xml")]
        public IActionResult Rate(PlayerCommentRating player_comment_rating)
        {
            var user = Session.GetUser(database, User);
            return Content(PlayerComments.RateComment(database, user, player_comment_rating), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
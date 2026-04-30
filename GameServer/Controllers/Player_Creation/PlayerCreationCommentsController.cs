using GameServer.Implementation.Common;
using GameServer.Implementation.Player_Creation;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player_Creation
{
    public class PlayerCreationCommentsController(Database database) : Controller
    {
        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("player_creation_comments.xml")]
        public IActionResult GetComments(int page, int per_page, SortColumn sort_column, SortOrder sort_order, int limit, Platform platform)
        {
            var user = Session.GetUser(database, User);
            return Content(PlayerCreationComments.ListComments(database, user, page, per_page, sort_column, sort_order,
                limit, platform, Request.Query["filters[player_creation_id]"], Request.Query["filters[player_id]"]), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("player_creation_comments.xml")]
        public IActionResult Create(PlayerCreationComment player_creation_comment)
        {
            var session = Session.GetSession(database, User);
            return Content(PlayerCreationComments.CreateComment(database, session, player_creation_comment), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("player_creation_comments/{id}.xml")]
        public IActionResult Delete(int id)
        {
            var user = Session.GetUser(database, User);
            return Content(PlayerCreationComments.DeleteComment(database, user, id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("player_creation_comment_ratings.xml")]
        public IActionResult Rate(PlayerCreationCommentRating player_creation_comment_rating)
        {
            var user = Session.GetUser(database, User);
            return Content(PlayerCreationComments.RateComment(database, user, player_creation_comment_rating), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
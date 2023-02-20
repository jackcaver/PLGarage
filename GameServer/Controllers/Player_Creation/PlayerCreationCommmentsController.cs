using GameServer.Implementation.Player_Creation;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player_Creation
{
    public class PlayerCreationCommmentsController : Controller
    {
        private readonly Database database;

        public PlayerCreationCommmentsController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("player_creation_comments.xml")]
        public IActionResult GetComments(int page, int per_page, SortColumn sort_column, SortOrder sort_order, int limit, Platform platform)
        {
            return Content(PlayerCreationComments.ListComments(database, Request.Cookies["username"], page, per_page, sort_column, sort_order,
                limit, platform, Request.Query["filters[player_creation_id]"], Request.Query["filters[player_id]"]), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_comments.xml")]
        public IActionResult Create(PlayerCreationComment player_creation_comment)
        {
            return Content(PlayerCreationComments.CreateComment(database, Request.Cookies["username"], player_creation_comment), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_comments/{id}.xml")]
        public IActionResult Delete(int id)
        {
            return Content(PlayerCreationComments.DeleteComment(database, Request.Cookies["username"], id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_comment_ratings.xml")]
        public IActionResult Rate(PlayerCreationCommentRating player_creation_comment_rating)
        {
            return Content(PlayerCreationComments.RateComment(database, Request.Cookies["username"], player_creation_comment_rating), "application/xml;charset=utf-8");
        }
    }
}
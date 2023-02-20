using GameServer.Implementation.Player;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player
{
    public class PlayerCommmentsController : Controller
    {
        private readonly Database database;

        public PlayerCommmentsController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("player_comments.xml")]
        public IActionResult GetComments(int page, int per_page, SortColumn sort_column, SortOrder sort_order, int limit, Platform platform)
        {
            return Content(PlayerComments.ListComments(database, Request.Cookies["username"], page, per_page, limit,
                sort_column, platform, Request.Query["filters[player_id]"], Request.Query["filters[author_id]"]), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_comments.xml")]
        public IActionResult Create(PlayerComment player_comment)
        {
            return Content(PlayerComments.CreateComment(database, Request.Cookies["username"], player_comment), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_comments/{id}.xml")]
        public IActionResult Delete(int id)
        {
            return Content(PlayerComments.RemoveComment(database, Request.Cookies["username"], id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_comment_ratings.xml")]
        public IActionResult Rate(PlayerCommentRating player_comment_rating)
        {
            return Content(PlayerComments.RateComment(database, Request.Cookies["username"], player_comment_rating), "application/xml;charset=utf-8");
        }
    }
}
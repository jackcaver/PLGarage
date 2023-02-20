using GameServer.Implementation.Player_Creation;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player_Creation
{
    public class PlayerCreationReviewsController : Controller
    {
        private readonly Database database;

        public PlayerCreationReviewsController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("player_creation_reviews.xml")]
        public IActionResult List(int player_creation_id, int page, int per_page)
        {
            return Content(PlayerCreationReviews.ListReviews(database, Request.Cookies["username"], player_creation_id, page, per_page), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("player_creation_reviews/by_player.xml")]
        public IActionResult ByPlayer(int player_id, int page, int per_page)
        {
            return Content(PlayerCreationReviews.ListReviews(database, Request.Cookies["username"], 0, page, per_page, player_id, true), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_reviews.xml")]
        public IActionResult Create(int player_creation_id, string content, int? player_id, string tags)
        {
            return Content(PlayerCreationReviews.CreateReview(database, Request.Cookies["username"], player_creation_id,
                content, player_id, tags), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_reviews/{id}.xml")]
        public IActionResult Remove(int id)
        {
            return Content(PlayerCreationReviews.RemoveReview(database, Request.Cookies["username"], id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_review_ratings.xml")]
        public IActionResult Rate(int player_creation_review_id, bool rating)
        {
            return Content(PlayerCreationReviews.RateReview(database, Request.Cookies["username"], player_creation_review_id, rating), "application/xml;charset=utf-8");
        }
    }
}
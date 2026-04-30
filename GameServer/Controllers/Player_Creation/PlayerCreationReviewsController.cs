using GameServer.Implementation.Common;
using GameServer.Implementation.Player_Creation;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player_Creation
{
    public class PlayerCreationReviewsController(Database database) : Controller
    {
        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("player_creation_reviews.xml")]
        public IActionResult List(int player_creation_id, int page, int per_page)
        {
            var user = Session.GetUser(database, User);
            return Content(PlayerCreationReviews.ListReviews(database, user, player_creation_id, page, per_page), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("player_creation_reviews/by_player.xml")]
        public IActionResult ByPlayer(int player_id, int page, int per_page)
        {
            var user = Session.GetUser(database, User);
            return Content(PlayerCreationReviews.ListReviews(database, user, 0, page, per_page, player_id, true), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("player_creation_reviews.xml")]
        public IActionResult Create(int player_creation_id, string content, int? player_id, string tags)
        {
            var session = Session.GetSession(database, User);
            return Content(PlayerCreationReviews.CreateReview(database, session, player_creation_id,
                content, player_id, tags), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("player_creation_reviews/{id}.xml")]
        public IActionResult Remove(int id)
        {
            var user = Session.GetUser(database, User);
            return Content(PlayerCreationReviews.RemoveReview(database, user, id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("player_creation_review_ratings.xml")]
        public IActionResult Rate(int player_creation_review_id, bool rating)
        {
            var user = Session.GetUser(database, User);
            return Content(PlayerCreationReviews.RateReview(database, user, player_creation_review_id, rating), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
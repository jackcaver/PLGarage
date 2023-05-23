using GameServer.Implementation.Player_Creation;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;

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
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreationReviews.ListReviews(database, SessionID, player_creation_id, page, per_page), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("player_creation_reviews/by_player.xml")]
        public IActionResult ByPlayer(int player_id, int page, int per_page)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreationReviews.ListReviews(database, SessionID, 0, page, per_page, player_id, true), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_reviews.xml")]
        public IActionResult Create(int player_creation_id, string content, int? player_id, string tags)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreationReviews.CreateReview(database, SessionID, player_creation_id,
                content, player_id, tags), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_reviews/{id}.xml")]
        public IActionResult Remove(int id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreationReviews.RemoveReview(database, SessionID, id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_review_ratings.xml")]
        public IActionResult Rate(int player_creation_review_id, bool rating)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreationReviews.RateReview(database, SessionID, player_creation_review_id, rating), "application/xml;charset=utf-8");
        }
    }
}
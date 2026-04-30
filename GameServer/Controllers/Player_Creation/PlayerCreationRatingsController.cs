using GameServer.Implementation.Common;
using GameServer.Implementation.Player_Creation;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player_Creation
{
    public class PlayerCreationRatingsController(Database database) : Controller
    {
        [HttpGet]
        [Authorize]
        [Route("player_creation_ratings/view.xml")]
        public IActionResult View(int player_creation_id, int player_id)
        {
            var session = Session.GetSession(database, User);
            return Content(PlayerCreationRatings.View(database, session, player_creation_id, player_id), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("player_creation_ratings.xml")]
        public IActionResult List(int player_creation_id, int page, int per_page)
        {
            return Content(PlayerCreationRatings.List(database, player_creation_id, page, per_page), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("player_creation_ratings.xml")]
        public IActionResult Create(PlayerCreationRating player_creation_rating)
        {
            var session = Session.GetSession(database, User);
            return Content(PlayerCreationRatings.Create(database, session, player_creation_rating), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("player_creation_ratings/clear.xml")]
        public IActionResult Clear(int player_creation_id)
        {
            var user = Session.GetUser(database, User);
            return Content(PlayerCreationRatings.Clear(database, user, player_creation_id), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
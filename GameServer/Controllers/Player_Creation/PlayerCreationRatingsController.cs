using GameServer.Implementation.Player_Creation;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player_Creation
{
    public class PlayerCreationRatingsController : Controller
    {
        private readonly Database database;

        public PlayerCreationRatingsController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("player_creation_ratings/view.xml")]
        public IActionResult View(int player_creation_id, int player_id)
        {
            return Content(PlayerCreationRatings.View(database, Request.Cookies["username"], player_creation_id, player_id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_ratings.xml")]
        public IActionResult Create(PlayerCreationRating player_creation_rating)
        {
            return Content(PlayerCreationRatings.Create(database, Request.Cookies["username"], player_creation_rating), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_ratings/clear.xml")]
        public IActionResult Clear(int player_creation_id)
        {
            return Content(PlayerCreationRatings.Clear(database, Request.Cookies["username"], player_creation_id), "application/xml;charset=utf-8");
        }
    }
}
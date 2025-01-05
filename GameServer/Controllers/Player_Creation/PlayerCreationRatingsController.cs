using GameServer.Implementation.Player_Creation;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;

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
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreationRatingsImpl.View(database, SessionID, player_creation_id, player_id), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("player_creation_ratings.xml")]
        public IActionResult View(int player_creation_id, int page, int per_page)
        {
            return Content(PlayerCreationRatingsImpl.List(database, player_creation_id, page, per_page), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_ratings.xml")]
        public IActionResult Create(PlayerCreationRating player_creation_rating)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreationRatingsImpl.Create(database, SessionID, player_creation_rating), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_ratings/clear.xml")]
        public IActionResult Clear(int player_creation_id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreationRatingsImpl.Clear(database, SessionID, player_creation_id), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
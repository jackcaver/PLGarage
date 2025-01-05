using GameServer.Implementation.Player;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GameServer.Controllers.Player
{
    public class PlayerRatingsController : Controller
    {
        private readonly Database database;

        public PlayerRatingsController(Database database)
        {
            this.database = database;
        }

        [HttpPost]
        [Route("player_ratings.xml")]
        public IActionResult Create(PlayerRating player_rating)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerRatingsImpl.Create(database, SessionID, player_rating), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}

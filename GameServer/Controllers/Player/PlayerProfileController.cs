using GameServer.Implementation.Player;
using GameServer.Models.Common;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GameServer.Controllers.Player
{
    public class PlayerProfileController : Controller
    {
        private readonly Database database;

        public PlayerProfileController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("player_profile/view.xml")]
        public IActionResult ViewProfile(int player_id, Platform platform)
        {
            return Content(PlayerProfilesImpl.ViewProfile(database, player_id, platform), "application/xml;charset=utf-8");
        }

        [HttpPut]
        [HttpPost]
        [Route("player_profile.xml")]
        public IActionResult UpdateProfile(PlayerProfile player_profile)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerProfilesImpl.UpdateProfile(database, SessionID, player_profile), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
using GameServer.Implementation.Common;
using GameServer.Implementation.Player;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player
{
    public class PlayerProfileController(Database database) : Controller
    {
        [HttpGet]
        [Route("player_profile/view.xml")]
        public IActionResult ViewProfile(int player_id, Platform platform)
        {
            return Content(PlayerProfiles.ViewProfile(database, player_id, platform), "application/xml;charset=utf-8");
        }

        [HttpPut]
        [HttpPost]
        [Authorize]
        [Route("player_profile.xml")]
        public IActionResult UpdateProfile(PlayerProfile player_profile)
        {
            var user = Session.GetUser(database, User);
            return Content(PlayerProfiles.UpdateProfile(database, user, player_profile), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
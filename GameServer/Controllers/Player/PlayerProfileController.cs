using GameServer.Implementation.Player;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;

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
            return Content(PlayerProfiles.ViewProfile(database, player_id, platform), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_profile.xml")]
        public IActionResult UpdateProfile(PlayerProfile player_profile)
        {
            return Content(PlayerProfiles.UpdateProfile(database, Request.Cookies["username"], player_profile), "application/xml;charset=utf-8");
        }
    }
}
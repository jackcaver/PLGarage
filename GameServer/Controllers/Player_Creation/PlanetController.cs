using GameServer.Implementation.Common;
using GameServer.Implementation.Player_Creation;
using GameServer.Models;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player_Creation
{
    public class PlanetController(Database database, IUGCStorage storage) : Controller
    {
        [HttpGet]
        [Route("planet.xml")]
        public IActionResult GetPlanet(int player_id, bool is_counted)
        {
            return Content(PlayerCreations.GetPlanet(database, player_id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("planet.xml")]
        public IActionResult UpdatePlanet(PlayerCreation planet)
        {
            var session = Session.GetSession(database, User);
            planet.player_creation_type = PlayerCreationType.PLANET;
            planet.data = Request.Form.Files.GetFile("planet[data]");
            return Content(PlayerCreations.UpdatePlayerCreation(database, storage, session, planet), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("planet/profile.xml")]
        public IActionResult GetPlanetProfile(int player_id)
        {
            return Content(PlayerCreations.GetPlanetProfile(database, player_id), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
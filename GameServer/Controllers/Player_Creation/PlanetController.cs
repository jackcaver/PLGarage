using GameServer.Implementation.Player_Creation;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player_Creation
{
    public class PlanetController : Controller
    {
        private readonly Database database;

        public PlanetController(Database database)
        {
            this.database = database;
        }

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
            planet.player_creation_type = PlayerCreationType.PLANET;
            planet.data = Request.Form.Files.GetFile("planet[data]");
            return Content(PlayerCreations.UpdatePlayerCreation(database, Request.Cookies["username"], planet), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("planet/profile.xml")]
        public IActionResult GetPlanetProfile(int player_id)
        {
            return Content(PlayerCreations.GetPlanetProfile(database, player_id), "application/xml;charset=utf-8");
        }
    }
}
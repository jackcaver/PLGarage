using GameServer.Implementation.Player;
using GameServer.Models.PlayerData;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player
{
    public class PlayerController : Controller
    {
        private readonly Database database;

        public PlayerController(Database database)
        {
            this.database = database;
        }

        [Route("players/to_id.xml")]
        public IActionResult ToID(string username)
        {
            return Content(PlayerProfiles.GetPlayerID(database, username), "application/xml;charset=utf-8");
        }

        [Route("players/{id}/info.xml")]
        public IActionResult GetPlayerInfo(int id, Platform platfom)
        {
            return Content(PlayerProfiles.GetPlayerInfo(database, id, Request.Cookies["username"]), "application/xml;charset=utf-8");
        }
    }
}
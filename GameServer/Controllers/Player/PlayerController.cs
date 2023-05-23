using GameServer.Implementation.Player;
using GameServer.Models.PlayerData;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;

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
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerProfiles.GetPlayerInfo(database, id, SessionID), "application/xml;charset=utf-8");
        }
    }
}
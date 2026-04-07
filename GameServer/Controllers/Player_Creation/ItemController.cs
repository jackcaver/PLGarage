using GameServer.Implementation.Player_Creation;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GameServer.Controllers.Player_Creation
{
    public class ItemController(Database database) : Controller
    {
        [HttpPost]
        [Route("items.xml")]
        public IActionResult Create(PlayerCreation player_creation)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            player_creation.data = Request.Form.Files.GetFile("player_creation[data]");
            player_creation.preview = Request.Form.Files.GetFile("player_creation[preview]");
            return Content(PlayerCreations.CreatePlayerCreation(database, SessionID, player_creation));
        }
        
        [HttpPost]
        [Route("items/{id}/download.xml")]
        public IActionResult Download(int id, bool is_counted)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreations.GetPlayerCreation(database, SessionID, id, is_counted, true), "application/xml;charset=utf-8");
        }
    }
}

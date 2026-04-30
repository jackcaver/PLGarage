using GameServer.Implementation.Common;
using GameServer.Implementation.Player_Creation;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player_Creation
{
    public class ItemController(Database database) : Controller
    {
        [HttpPost]
        [Authorize]
        [Route("items.xml")]
        public IActionResult Create(PlayerCreation player_creation)
        {
            var session = Session.GetSession(database, User);
            player_creation.data = Request.Form.Files.GetFile("player_creation[data]");
            player_creation.preview = Request.Form.Files.GetFile("player_creation[preview]");
            return Content(PlayerCreations.CreatePlayerCreation(database, session, player_creation));
        }
        
        [HttpPost]
        [Authorize]
        [Route("items/{id}/download.xml")]
        public IActionResult Download(int id, bool is_counted)
        {
            var session = Session.GetSession(database, User);
            return Content(PlayerCreations.GetPlayerCreation(database, session, id, is_counted, true), "application/xml;charset=utf-8");
        }
    }
}

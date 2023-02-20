using System.Collections.Generic;
using GameServer.Implementation.Player_Creation;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player_Creation
{
    public class PlayerCreationsController : Controller
    {
        private readonly Database database;

        public PlayerCreationsController(Database database)
        {
            this.database = database;
        }

        [HttpPost]
        [Route("player_creations/verify.xml")]
        public IActionResult Verify(List<int> id, List<int> offline_id)
        {
            return Content(PlayerCreations.VerifyPlayerCreations(database, id, offline_id), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("player_creations/{id}/{file}")]
        public IActionResult GetData(int id, string file)
        {
            var data = UserGeneratedContentUtils.LoadPlayerCreation(id, file);
            if (data == null)
                return NotFound();
            string contentType = "application/octet-stream";
            if (file.EndsWith(".png"))
                contentType = "image/png";
            if (file.EndsWith(".jpg"))
                contentType = "image/jpeg";
            return File(data, contentType);
        }
    }
}
using System.Linq;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Api
{
    public class PlayerApiController : Controller
    {
        private readonly Database database;

        public PlayerApiController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("/api/player")]
        public IActionResult GetPlayer(string username)
        {
            var player = database.Users.FirstOrDefault(x => x.Username == username);

            if (player == null)
                return NotFound(new { error = "error_player_not_found"});

            return Json(new
            {
                player.UserId,
                player.Username,
                player.IsBanned,
                player.PolicyAccepted,
                player.CreatedAt
            });
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
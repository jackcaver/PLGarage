using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GameServer.Utils;

namespace GameServer.Controllers.Api
{
    public class PlayerCountApiController(Database database) : Controller
    {
        [HttpGet]
        [Route("/api/playercounts")]
        public IActionResult GetPlayerCount()
        {
            return Content($"{database.Users.Count(x => !x.IsBanned && x.PolicyAccepted)}");
        }

        [HttpGet]
        [Route("/api/playercounts/sessioncount")]
        public IActionResult GetSessionCount(bool? isMnr = null)
        {
            return Content($"{database.Sessions.Count(x => (isMnr == null || x.IsMNR == isMnr) &&
                                                      x.LastPing.AddMinutes(1) > TimeUtils.Now)}");
        }

        [HttpGet]
        [Route("/api/playercounts/presence")]
        public IActionResult GetPlayersPresence(bool? isMnr = null)
        {
            var playersPresence = database.Sessions
                .Where(x => (isMnr == null || x.IsMNR == isMnr) &&
                            x.LastPing.AddMinutes(1) > TimeUtils.Now)
                .Select(x => new
                {
                    x.UserId,
                    x.Username,
                    Presence = x.Presence.ToString(),
                    Platform = x.Platform.ToString(),
                    x.IsMNR
                })
                .OrderBy(x => x.Username)
                .ToList();

            return Json(playersPresence);
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}

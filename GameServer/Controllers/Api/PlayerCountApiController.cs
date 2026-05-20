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
        public IActionResult GetPlayersPresence(bool? isMnr = null, int page = 1, int perPage = 10)
        {
            if (page < 1) page = 1;
            if (perPage < 1) perPage = 10;
            if (perPage > 10) perPage = 10;

            var query = database.Sessions
                .Where(x => (isMnr == null || x.IsMNR == isMnr) &&
                            x.LastPing.AddMinutes(1) > TimeUtils.Now)
                .OrderBy(x => x.Username);

            var total = query.Count();

            var playersPresence = query
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .Select(x => new
                {
                    x.UserId,
                    x.Username,
                    Presence = x.Presence.ToString(),
                    Platform = x.Platform.ToString(),
                    x.IsMNR
                })
                .ToList();

            return Json(new { total, playersPresence });
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}

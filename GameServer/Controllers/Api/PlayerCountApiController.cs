using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GameServer.Implementation.Common;
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

            var sessions = query
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .Select(x => new
                {
                    x.SessionId,
                    x.UserId,
                    x.Username,
                    Presence = x.Presence.ToString(),
                    Platform = x.Platform.ToString(),
                    x.IsMNR
                })
                .ToList();

            var presence = sessions
                .Select(x => new
                {
                    x.UserId,
                    x.Username,
                    x.Presence,
                    x.Platform,
                    x.IsMNR,
                    IsRpcn = Session.TryGetSessionNetwork(x.SessionId, out var isRpcn) && isRpcn
                })
                .ToList();

            return Json(new { total, presence });
        }

        [HttpGet]
        [Route("/api/playercounts/platform")]
        public IActionResult GetPlatformCount(bool? isMnr = null)
        {
            var query = database.Sessions
                .Where(x => (isMnr == null || x.IsMNR == isMnr) &&
                            x.LastPing.AddMinutes(1) > TimeUtils.Now);

            var total = query.Count();

            var platforms = query
                .GroupBy(x => x.Platform)
                .Select(g => new
                {
                    platform = g.Key.ToString(),
                    count = g.Count()
                })
                .OrderBy(x => x.platform)
                .ToList();

            return Json(new
            {
                total,
                platforms
            });
        }

        [HttpGet]
        [Route("/api/playercounts/network")]
        public IActionResult GetNetworkCount(bool? isMnr = null)
        {
            var sessions = database.Sessions
                .Where(x => (isMnr == null || x.IsMNR == isMnr) &&
                            x.LastPing.AddMinutes(1) > TimeUtils.Now)
                .Select(x => new
                {
                    x.SessionId
                })
                .ToList();

            var rpcnSessions = sessions.Count(x => Session.TryGetSessionNetwork(x.SessionId, out var isRpcn) && isRpcn);
            var psnSessions = sessions.Count(x => Session.TryGetSessionNetwork(x.SessionId, out var isRpcn) && !isRpcn);

            return Json(new
            {
                total = sessions.Count,
                rpcn = rpcnSessions,
                psn = psnSessions
            });
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}

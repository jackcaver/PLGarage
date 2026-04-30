using System.Linq;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Controllers.Api
{
    public class PlayerCreationsApiController(Database database) : Controller
    {
        [HttpGet]
        [Route("/api/creationcount")]
        public IActionResult GetCreationCount()
        {
            var q = database.PlayerCreations
                .AsNoTracking()
                .Where(x => x.Type != PlayerCreationType.DELETED 
                && x.Type != PlayerCreationType.STORY);

            var total = q.Count();

            var lbpkTypeCounts = q
                .Where(x => !x.IsMNR)
                .GroupBy(x => x.Type)
                .Select(g => new
                {
                    Type = g.Key,
                    Count = g.Count()
                })
                .ToList()
                .ToDictionary(
                    g => g.Type.ToString(),
                    g => g.Count
                );

            var mnrRows = q
                .Where(x => x.IsMNR)
                .GroupBy(x => new { x.Platform, x.Type })
                .Select(g => new
                {
                    Platform = g.Key.Platform,
                    Type = g.Key.Type,
                    Count = g.Count()
                })
                .ToList();

            var mnrPlatformCounts = mnrRows
                .GroupBy(x => x.Platform)
                .ToDictionary(
                    g => g.Key.ToString(),
                    g => g.ToDictionary(
                        x => x.Type.ToString(),
                        x => x.Count
                    )
                );

            var lbpkTotal = q.Count(x => !x.IsMNR);
            var mnrTotal = q.Count(x => x.IsMNR);

            return Json(new
            {
                total,
                totalLBPK = lbpkTotal,
                totalMNR = mnrTotal,
                lbpk = lbpkTypeCounts,
                mnr = mnrPlatformCounts
            });
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Controllers.Api
{
    public class PlayerCreationsApiController : Controller
    {
        private readonly Database database;

        public PlayerCreationsApiController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("/api/creationcount")]
        public IActionResult GetCreationCount()
        {
            var q = database.PlayerCreations
                .AsNoTracking()
                .Where(x => x.Type != PlayerCreationType.DELETED);

            var total = q.Count();

            var grouped = q
                .GroupBy(x => new { x.IsMNR, x.Platform, x.Type })
                .Select(g => new
                {
                    IsMnr = g.Key.IsMNR,
                    Platform = g.Key.Platform,
                    Type = g.Key.Type,
                    Count = g.Count()
                })
                .ToList();

            var lbpkRows = grouped.Where(x => !x.IsMnr).ToList();
            var mnrRows = grouped.Where(x => x.IsMnr).ToList();

            var lbpkTypeCounts = lbpkRows
                .GroupBy(x => x.Type)
                .ToDictionary(
                    g => g.Key.ToString(),
                    g => g.Sum(x => x.Count)
                );

            var mnrPlatformCounts = mnrRows
                .GroupBy(x => x.Platform)
                .ToDictionary(
                    g => g.Key.ToString(),
                    g => g.ToDictionary(
                        x => x.Type.ToString(),
                        x => x.Count
                    )
                );

            var lbpkTotal = lbpkTypeCounts.Values.Sum();
            var mnrTotal = mnrRows.Sum(x => x.Count);

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
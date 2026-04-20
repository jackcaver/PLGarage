using System.Collections.Generic;
using System.Linq;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Controllers.Api
{
    public class PlayerCreationCountApiController : Controller
    {
        private readonly Database database;

        public PlayerCreationCountApiController(Database database)
        {
            this.database = database;
        }

        // this is probably useless since we can do multiple xml calls to get the same thing

        [HttpGet]
        [Route("/api/creationcount")]
        public IActionResult GetCreationCount()
        {
            var q = database.PlayerCreations
                .AsNoTracking()
                .Where(x => x.Type != PlayerCreationType.DELETED)
                .AsQueryable();

            var total = q.Count();

            var byTypeRows = q
                .GroupBy(x => x.Type)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToList();

            var byGameRows = q
                .GroupBy(x => x.IsMNR)
                .Select(g => new { IsMnr = g.Key, Count = g.Count() })
                .ToList();

            var byGamePlatformRows = q
                .GroupBy(x => new { x.IsMNR, x.Platform })
                .Select(g => new { IsMnr = g.Key.IsMNR, Platform = g.Key.Platform, Count = g.Count() })
                .ToList();

            var mnrByTypePlatformRows = q
                .Where(x => x.IsMNR)
                .GroupBy(x => new { x.Type, x.Platform })
                .Select(g => new { Type = g.Key.Type, Platform = g.Key.Platform, Count = g.Count() })
                .ToList();

            var byType = byTypeRows
                .OrderByDescending(r => r.Count)
                .ToDictionary(r => r.Type.ToString(), r => r.Count);

            int lbpkTotal = byGameRows.FirstOrDefault(r => r.IsMnr == false)?.Count ?? 0;
            int mnrTotal = byGameRows.FirstOrDefault(r => r.IsMnr == true)?.Count ?? 0;

            var lbpk = byGamePlatformRows
                .Where(r => r.IsMnr == false)
                .OrderByDescending(r => r.Count)
                .ToDictionary(r => r.Platform.ToString(), r => r.Count);

            var mnrPlatforms = byGamePlatformRows
                .Where(r => r.IsMnr == true)
                .OrderByDescending(r => r.Count)
                .ToDictionary(r => r.Platform.ToString(), r => r.Count);

            var mnrByTypeAndPlatform = new Dictionary<string, Dictionary<string, int>>();
            foreach (var row in mnrByTypePlatformRows.OrderByDescending(r => r.Count))
            {
                var typeKey = row.Type.ToString();
                var platKey = row.Platform.ToString();

                if (!mnrByTypeAndPlatform.TryGetValue(typeKey, out var platDict))
                {
                    platDict = new Dictionary<string, int>();
                    mnrByTypeAndPlatform[typeKey] = platDict;
                }

                platDict[platKey] = row.Count;
            }

            return Json(new
            {
                totals = new
                {
                    allCreations = total,
                    lbpkCreations = lbpkTotal,
                    mnrCreations = new
                    {
                        total = mnrTotal,
                        byPlatform = mnrPlatforms
                    }
                },
                breakdown = new
                {
                    byType = byType,
                    byGame = new
                    {
                        lbpk = new
                        {
                            total = lbpkTotal,
                            byPlatform = lbpk
                        },
                        mnr = new
                        {
                            total = mnrTotal,
                            byPlatform = mnrPlatforms,
                            byTypeAndPlatform = mnrByTypeAndPlatform
                        }
                    }
                }
            });
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
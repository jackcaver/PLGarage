using System.Collections.Generic;
using System.Linq;
using GameServer.Models.Config;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Controllers.Api
{
    public class PlayerApiController(Database database) : Controller
    {

        [HttpGet]
        [Route("/api/player/{username}")]
        public IActionResult GetPlayer(string username)
        {
            var player = database.Users
                .AsNoTracking()
                .Where(x => x.Username == username)
                .Select(x => new
                {
                    x.UserId,
                    x.Username,
                    x.Quote,
                    starRating = x.PlayerRatings.Count > 0 ? (float)x.PlayerRatings.Average(r => r.Rating) : 0,
                    onlineRaces = x.RacesStarted.Count,
                    onlineWins = x.RacesFinished.Count(r => r.IsWinner),
                    skillLevelIdPS3 = x.PlayerExperiencePoints.Where(p => p.Platform == Platform.PS3)
                        .Sum(p => p.Amount) + x.PlayerCreationPoints.Where(p => p.Platform == Platform.PS3)
                        .Sum(p => p.Amount),
                    skillLevelIdPSV = x.PlayerExperiencePoints.Where(p => p.Platform == Platform.PSV)
                        .Sum(p => p.Amount) + x.PlayerCreationPoints.Where(p => p.Platform == Platform.PSV)
                        .Sum(p => p.Amount),
                    skillRating = x.Points(Platform.PS3),
                    x.IsBanned,
                    x.CreatedAt,
                    creationTypes = x.PlayerCreations
                        .Where(c => c.Type != PlayerCreationType.DELETED)
                        .Select(c => new { c.Type, c.IsMNR, c.Platform })
                        .ToList()
                })
                .FirstOrDefault();

            if (player == null)
                return NotFound(new { error = "error_player_not_found"});

            var creationsCount = new
            {
                lbpk = player.creationTypes
                    .Where(c => !c.IsMNR)
                    .GroupBy(c => c.Type)
                    .ToDictionary(g => g.Key.ToString(), g => g.Count()),
                mnr = player.creationTypes
                    .Where(c => c.IsMNR)
                    .GroupBy(c => c.Platform)
                    .ToDictionary(
                        g => g.Key.ToString(),
                        g => g.GroupBy(c => c.Type)
                                .ToDictionary(t => t.Key.ToString(), t => t.Count())
                    )
            };

            var skillLevelPS3 = SkillConfig.Instance.GetSkillLevel(player.skillLevelIdPS3);
            var skillLevelPSV = SkillConfig.Instance.GetSkillLevel(player.skillLevelIdPSV);

            return Json(new
            {
                player.UserId,
                player.Username,
                player.Quote,
                starRating = player.starRating.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture),
                player.onlineRaces,
                player.onlineWins,
                skillLevels = new Dictionary<string, object>
                {
                    ["PS3"] = new { skillLevelPS3.Id, skillLevelPS3.Name },
                    ["PSV"] = new { skillLevelPSV.Id, skillLevelPSV.Name },
                },
                player.IsBanned,
                player.skillRating,
                player.CreatedAt,
                creationsCount
            });
        }

        [HttpGet]
        [Route("/api/player/{username}/comments")]
        public IActionResult GetPlayerComments(string username)
        {
            var player = database.Users
                .AsNoTracking()
                .FirstOrDefault(x => x.Username == username);

            if (player == null)
                return NotFound(new { error = "error_player_not_found"});

            var comments = database.PlayerComments
                .AsNoTracking()
                .Where(c => c.PlayerId == player.UserId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new
                {
                    c.Id,
                    comment = c.Body,
                    c.CreatedAt,
                    c.Author.Username
                })
                .ToList();

            return Json(comments);
        }

        [HttpGet]
        [Route("/api/player/{username}/hearted")]
        public IActionResult GetPlayerHearted(string username, bool? isMnr = null)
        {
            var player = database.Users
                .AsNoTracking()
                .FirstOrDefault(x => x.Username == username);

            if (player == null)
                return NotFound(new { error = "error_player_not_found" });

            var hearted = database.HeartedProfiles
                .AsNoTracking()
                .Where(x => x.UserId == player.UserId)
                .Where(x => isMnr == true ? x.IsMNR : !x.IsMNR)
                .OrderByDescending(x => x.HeartedAt)
                .Select(x => new
                {
                    x.HeartedUser.Username,
                    x.HeartedAt
                })
                .ToList();

            return Json(hearted);
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
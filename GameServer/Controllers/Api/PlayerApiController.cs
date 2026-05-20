using System;
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
        [Route("/api/player")]
        public IActionResult GetPlayer(int? id = null, string username = null)
        {
            if (id == null && username == null)
                return BadRequest(new { error = "error_missing_username_or_id" });

            var query = database.Users.AsNoTracking();

            if (id != null)
                query = query.Where(x => x.UserId == id);
            else
                query = query.Where(x => x.Username == username);

            var player = query
                .Select(x => new
                {
                    x.UserId,
                    x.Username,
                    x.Quote,
                    x.StarRating,
                    onlineRaces = x.RacesStarted.Count,
                    onlineFinished = x.OnlineFinished,
                    onlineForfeits = x.OnlineForfeit,
                    onlineWins = x.RacesFinished.Count(r => r.IsWinner),
                    skillLevelIdPS3 = x.PlayerExperiencePoints.Where(p => p.Platform == Platform.PS3)
                        .Sum(p => p.Amount) + x.PlayerCreationPoints.Where(p => p.Platform == Platform.PS3)
                        .Sum(p => p.Amount),
                    skillLevelIdPSV = x.PlayerExperiencePoints.Where(p => p.Platform == Platform.PSV)
                        .Sum(p => p.Amount) + x.PlayerCreationPoints.Where(p => p.Platform == Platform.PSV)
                        .Sum(p => p.Amount),
                    skillRating = x.Points(Platform.PS3),
                    x.WinStreak,
                    x.LongestWinStreak,
                    x.LongestDrift,
                    x.LongestHangTime,
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

            var user = database.Users
                .AsNoTracking()
                .FirstOrDefault(x => x.UserId == player.UserId);

            var presence = user?.Presence(database, Platform.PS3).ToString();

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
                player.StarRating,
                player.onlineRaces,
                player.onlineFinished,
                player.onlineForfeits,
                player.onlineWins,
                skillLevels = new Dictionary<string, object>
                {
                    ["PS3"] = new { skillLevelPS3.Id, skillLevelPS3.Name },
                    ["PSV"] = new { skillLevelPSV.Id, skillLevelPSV.Name },
                },
                player.skillRating,
                player.WinStreak,
                player.LongestWinStreak,
                player.LongestDrift,
                player.LongestHangTime,
                presence,
                player.IsBanned,
                player.CreatedAt,
                creationsCount
            });
        }

        [HttpGet]
        [Route("/api/player/{username}/comments")]
        public IActionResult GetPlayerComments(string username, int page = 1, int perPage = 10)
        {
            if (page < 1) page = 1;
            if (perPage < 1) perPage = 10;
            if (perPage > 10) perPage = 10;

            var player = database.Users
                .AsNoTracking()
                .FirstOrDefault(x => x.Username == username);

            if (player == null)
                return NotFound(new { error = "error_player_not_found"});

            var query = database.PlayerComments
                .AsNoTracking()
                .Where(c => c.PlayerId == player.UserId)
                .OrderByDescending(c => c.CreatedAt);

            var total = query.Count();

            var comments = query
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .Select(c => new
                {
                    c.Id,
                    comment = c.Body,
                    c.CreatedAt,
                    c.Author.Username
                })
                .ToList();

            return Json(new { total, comments });
        }

        [HttpGet]
        [Route("/api/player/{username}/hearted")]
        public IActionResult GetPlayerHearted(string username, bool? isMnr = null, int page = 1, int perPage = 10)
        {
            if (page < 1) page = 1;
            if (perPage < 1) perPage = 10;
            if (perPage > 10) perPage = 10;

            var player = database.Users
                .AsNoTracking()
                .FirstOrDefault(x => x.Username == username);

            if (player == null)
                return NotFound(new { error = "error_player_not_found" });

            var query = database.HeartedProfiles
                .AsNoTracking()
                .Where(x => x.UserId == player.UserId)
                .Where(x => isMnr == true ? x.IsMNR : !x.IsMNR)
                .OrderByDescending(x => x.HeartedAt);

            var total = query.Count();

            var hearted = query
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .Select(x => new
                {
                    x.HeartedUser.Username,
                    x.HeartedAt
                })
                .ToList();

            return Json(new { total, hearted });
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
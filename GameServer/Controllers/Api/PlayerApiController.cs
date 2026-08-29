using GameServer.Implementation.Common;
using System.Collections.Generic;
using System.Linq;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Controllers.Api
{
    public class PlayerApiController(Database database) : Controller
    {
        [HttpGet]
        [Route("/api/usernameToId")]
        public IActionResult GetUserIdFromUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest("error_missing_username");

            var user = database.Users
                .AsNoTracking()
                .FirstOrDefault(x => x.Username == username);

            if (user == null)
                return NotFound("error_player_not_found");

            return Content($"{user.UserId}");
        }

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
                    hearts = x.HeartedByProfiles.Count,
                    onlineRaces = x.RacesStarted.Count,
                    onlineFinished = x.OnlineFinished,
                    onlineForfeits = x.OnlineForfeit,
                    onlineWins = x.RacesFinished.Count(r => r.IsWinner),
                    skillLevelIdPS3 = x.SkillLevelId(Platform.PS3),
                    skillLevelNamePS3 = x.SkillLevelName(Platform.PS3),
                    skillLevelIdPSV = x.SkillLevelId(Platform.PSV),
                    skillLevelNamePSV = x.SkillLevelName(Platform.PSV),
                    skillLevelIdPSP = x.SkillLevelId(Platform.PSP),
                    skillLevelNamePSP = x.SkillLevelName(Platform.PSP),
                    totalXpPS3 = x.TotalXP(Platform.PS3),
                    creationXpPS3 = x.CreatorPoints(Platform.PS3),
                    raceXpPS3 = x.ExperiencePoints(Platform.PS3),
                    totalXpPSV = x.TotalXP(Platform.PSV),
                    creationXpPSV = x.CreatorPoints(Platform.PSV),
                    raceXpPSV = x.ExperiencePoints(Platform.PSV),
                    totalXpPSP = x.TotalXP(Platform.PSP),
                    creationXpPSP = x.CreatorPoints(Platform.PSP),
                    raceXpPSP = x.ExperiencePoints(Platform.PSP),
                    skillRating = x.Points(Platform.PS3),
                    x.WinStreak,
                    x.LongestWinStreak,
                    x.LongestDrift,
                    x.LongestHangTime,
                    x.ModMiles,
                    x.CharacterIdx,
                    x.KartIdx,
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

            var lbpkTypeValues = new[]
            {
                PlayerCreationType.PHOTO,
                PlayerCreationType.TRACK,
                PlayerCreationType.ITEM
            };

            var mnrTypeValues = new[]
            {
                PlayerCreationType.CHARACTER,
                PlayerCreationType.KART,
                PlayerCreationType.TRACK
            };

            var creationsCount = new
            {
                lbpk = lbpkTypeValues
                    .ToDictionary(
                        t => t.ToString(),
                        t => player.creationTypes.Count(c => !c.IsMNR && c.Type == t)
                    ),
                mnr = player.creationTypes
                    .Where(c => c.IsMNR)
                    .GroupBy(c => c.Platform)
                    .ToDictionary(
                        g => g.Key.ToString(),
                        g => mnrTypeValues
                                .ToDictionary(
                                    t => t.ToString(),
                                    t => g.Count(c => c.Type == t)
                                )
                    )
            };

            return Json(new
            {
                player.UserId,
                player.Username,
                player.Quote,
                player.StarRating,
                player.hearts,
                player.onlineRaces,
                player.onlineFinished,
                player.onlineForfeits,
                player.onlineWins,
                skillLevels = new Dictionary<string, object>
                {
                    ["PS3"] = new { Id = player.skillLevelIdPS3, Name = player.skillLevelNamePS3, xp = player.totalXpPS3, creationPoints = player.creationXpPS3, raceXp = player.raceXpPS3 },
                    ["PSV"] = new { Id = player.skillLevelIdPSV, Name = player.skillLevelNamePSV, xp = player.totalXpPSV, creationPoints = player.creationXpPSV, raceXp = player.raceXpPSV },
                    ["PSP"] = new { Id = player.skillLevelIdPSP, Name = player.skillLevelNamePSP, xp = player.totalXpPSP, creationPoints = player.creationXpPSP, raceXp = player.raceXpPSP }
                },
                player.skillRating,
                player.WinStreak,
                player.LongestWinStreak,
                player.LongestDrift,
                player.LongestHangTime,
                presence = new Dictionary<string, object>
                {
                    ["LBPK"] = Session.GetPresence(database, player.UserId, Platform.PS3, false).ToString(),
                    ["MNR"] = new Dictionary<string, string>
                    { 
                        ["PS3"] = Session.GetPresence(database, player.UserId, Platform.PS3, true).ToString(),
                        ["PSV"] = Session.GetPresence(database, player.UserId, Platform.PSV, true).ToString(),
                        ["PSP"] = Session.GetPresence(database, player.UserId, Platform.PSP, true).ToString()
                    }
                },
                player.ModMiles,
                player.CharacterIdx,
                player.KartIdx,
                player.IsBanned,
                player.CreatedAt,
                creationsCount
            });
        }

        [HttpGet]
        [Route("/api/player/{username}/comments")]
        public IActionResult GetPlayerComments(string username, int page = 1, int perPage = 10, SortOrder? sortOrder = null)
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
                .Where(c => c.PlayerId == player.UserId);

            var total = query.Count();
            var orderedQuery = ((sortOrder ?? SortOrder.desc) == SortOrder.asc)
                ? query.OrderBy(c => c.CreatedAt)
                : query.OrderByDescending(c => c.CreatedAt);

            var comments = orderedQuery
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
        public IActionResult GetPlayerHearted(string username, bool? isMnr = null, 
            int page = 1, int perPage = 10, SortOrder? sortOrder = null)
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
                .Where(x => isMnr == true ? x.IsMNR : !x.IsMNR);

            var total = query.Count();
            var orderedQuery = ((sortOrder ?? SortOrder.desc) == SortOrder.asc)
                ? query.OrderBy(x => x.HeartedAt)
                : query.OrderByDescending(x => x.HeartedAt);

            var hearted = orderedQuery
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .Select(x => new
                {
                    x.HeartedUserId,
                    x.HeartedUser.Username,
                    x.HeartedAt
                })
                .ToList();

            return Json(new { total, hearted });
        }

    [HttpGet]
    [Route("/api/recent/players/")]
    public IActionResult GetRecentPlayers(int count = 10, bool? playedMnr = null)
    {
        if (count < 1) count = 10;
        if (count > 10) count = 10;

        var query = database.Users.AsNoTracking()
            .Where(u => u.IsBanned == false);

        if (playedMnr.HasValue)
        {
            query = query.Where(u => u.PlayedMNR == playedMnr.Value);
        }

        query = query.Where(u => !u.IsBanned)
                     .OrderByDescending(u => u.CreatedAt);

        var total = query.Count();

        var recentPlayers = query
            .Take(count)
            .Select(u => new
            {
                u.UserId,
                u.Username,
                u.CreatedAt
            })
            .ToList();

        return Json(new { total, recentPlayers });
    }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
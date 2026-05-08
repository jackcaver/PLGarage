using System.Linq;
using System.Globalization;
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

        [HttpGet]
        [Route("/api/creation/{id}")]
        public IActionResult GetCreationsById(int id)
        {
            var creations = database.PlayerCreations
                .AsNoTracking()
                .Where(x => x.PlayerCreationId == id
                && x.ModerationStatus != ModerationStatus.BANNED
                && x.ModerationStatus != ModerationStatus.ILLEGAL)
                .Select(x => new
                {
                    x.PlayerCreationId,
                    x.Name,
                    x.Description,
                    rating = x.Ratings.Count != 0 ? (float?)x.Ratings.Average(r => r.Rating) : 0,
                    x.Author.Username,
                    x.Type,
                    x.Tags,
                    x.Platform,
                    x.IsMNR,
                    x.CreatedAt,
                    downloadsAllTime = x.Downloads.Count,
                    downloadsThisWeek = x.Downloads.Count(d => d.DownloadedAt >= TimeUtils.ThisWeekStart),
                    downloadsLastWeek = x.Downloads.Count(d => d.DownloadedAt >= TimeUtils.LastWeekStart && d.DownloadedAt < TimeUtils.ThisWeekStart),
                    recordBestLapTime = x.Type == PlayerCreationType.TRACK && x.IsMNR
                        ? x.Scores.Where(s => s.SubGroupId != 700).OrderBy(s => s.BestLapTime).Select(s => (float?)s.BestLapTime).FirstOrDefault()
                        : null,
                    recordScore = x.Type == PlayerCreationType.TRACK && !x.IsMNR
                        ? x.Scores.Max(s => (float?)s.Points)
                        : null,
                    recordFinishTime = x.Type == PlayerCreationType.TRACK && !x.IsMNR
                        ? x.Scores.Max(s => (float?)s.FinishTime)
                        : null,
                    recordLongestDrift = x.LongestDrift,
                    recordLongestHangTime = x.LongestHangTime
                })
                .ToList();

            if (creations.Count == 0)
                return NotFound(new { error = "error_creation_not_found"});

            return Json(creations.Select(x => new
            {
                x.PlayerCreationId,
                x.Name,
                x.Description,
                rating = (x.rating ?? 0).ToString("0.0", CultureInfo.InvariantCulture),
                creatorUsername = x.Username,
                Type = x.Type.ToString(),
                x.Tags,
                Platform = x.Platform.ToString(),
                x.IsMNR,
                x.CreatedAt,
                downloads = new
                {
                    all_time = x.downloadsAllTime,
                    this_week = x.downloadsThisWeek,
                    last_week = x.downloadsLastWeek
                },
                records = x.Type == PlayerCreationType.TRACK
                    ? x.IsMNR
                        ? (object)new { bestLapTime = x.recordBestLapTime, longestDrift = x.recordLongestDrift, longestHangTime = x.recordLongestHangTime }
                        : new { score = x.recordScore, finishTime = x.recordFinishTime }
                    : null
            }));
        }

        [HttpGet]
        [Route("api/creations/search")]
        public IActionResult SearchCreations(string query, PlayerCreationType? type, bool? isMnr, int page = 1, int pageSize = 10)
        {
            var q = database.PlayerCreations
                .AsNoTracking()
                .Where(x => x.Type != PlayerCreationType.DELETED 
                && x.Type != PlayerCreationType.STORY
                && x.ModerationStatus != ModerationStatus.BANNED
                && x.ModerationStatus != ModerationStatus.ILLEGAL);

            if (!string.IsNullOrEmpty(query))
            {
                q = q.Where(x => x.Name.Contains(query) || x.Author.Username.Contains(query));
            }

            if (type.HasValue)
            {
                q = q.Where(x => x.Type == type.Value);
            }

            if (isMnr.HasValue)
            {
                q = q.Where(x => x.IsMNR == isMnr.Value);
            }

            var totalResults = q.Count();

            var creations = q
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.PlayerCreationId,
                    x.Name,
                    x.Description,
                    rating = x.Ratings.Count != 0 ? (float?)x.Ratings.Average(r => r.Rating) : 0,
                    x.Author.Username,
                    x.Type,
                    x.Tags,
                    x.Platform,
                    x.IsMNR,
                    x.CreatedAt,
                    downloadsAllTime = x.Downloads.Count,
                    downloadsThisWeek = x.Downloads.Count(d => d.DownloadedAt >= TimeUtils.ThisWeekStart),
                    downloadsLastWeek = x.Downloads.Count(d => d.DownloadedAt >= TimeUtils.LastWeekStart && d.DownloadedAt < TimeUtils.ThisWeekStart),
                    recordBestLapTime = x.Type == PlayerCreationType.TRACK && x.IsMNR
                        ? x.Scores.Where(s => s.SubGroupId != 700).OrderBy(s => s.BestLapTime).Select(s => (float?)s.BestLapTime).FirstOrDefault()
                        : null,
                    recordScore = x.Type == PlayerCreationType.TRACK && !x.IsMNR
                        ? x.Scores.Max(s => (float?)s.Points)
                        : null,
                    recordFinishTime = x.Type == PlayerCreationType.TRACK && !x.IsMNR
                        ? x.Scores.Max(s => (float?)s.FinishTime)
                        : null,
                    recordLongestDrift = x.LongestDrift,
                    recordLongestHangTime = x.LongestHangTime
                })
                .ToList();

            if (creations.Count == 0)
                return NotFound(new { error = "error_creation_not_found"});

            return Json(creations.Select(x => new
            {
                x.PlayerCreationId,
                x.Name,
                x.Description,
                rating = (x.rating ?? 0).ToString("0.0", CultureInfo.InvariantCulture),
                creatorUsername = x.Username,
                Type = x.Type.ToString(),
                x.Tags,
                Platform = x.Platform.ToString(),
                x.IsMNR,
                x.CreatedAt,
                downloads = new
                {
                    all_time = x.downloadsAllTime,
                    this_week = x.downloadsThisWeek,
                    last_week = x.downloadsLastWeek
                },
                records = x.Type == PlayerCreationType.TRACK
                    ? x.IsMNR
                        ? (object)new { bestLapTime = x.recordBestLapTime, longestDrift = x.recordLongestDrift, longestHangTime = x.recordLongestHangTime }
                        : new { score = x.recordScore, finishTime = x.recordFinishTime }
                    : null
            }));
        }

        [HttpGet]
        [Route("/api/creations/recent")]
        public IActionResult GetRecentCreations(int count = 10, bool? isMnr = null)
        {
            if (count > 10) count = 10;

            var creations = database.PlayerCreations
                .AsNoTracking()
                .Where(x => x.Type != PlayerCreationType.DELETED 
                && x.Type != PlayerCreationType.STORY
                && x.ModerationStatus != ModerationStatus.BANNED
                && x.ModerationStatus != ModerationStatus.ILLEGAL
                && (!isMnr.HasValue || x.IsMNR == isMnr.Value))
                .OrderByDescending(x => x.CreatedAt)
                .Take(count)
                .Select(x => new
                {
                    x.PlayerCreationId,
                    x.Name,
                    x.Description,
                    rating = x.Ratings.Count != 0 ? (float?)x.Ratings.Average(r => r.Rating) : 0,
                    x.Author.Username,
                    x.Type,
                    x.Tags,
                    x.Platform,
                    x.IsMNR,
                    x.CreatedAt,
                    downloadsAllTime = x.Downloads.Count,
                    downloadsThisWeek = x.Downloads.Count(d => d.DownloadedAt >= TimeUtils.ThisWeekStart),
                    downloadsLastWeek = x.Downloads.Count(d => d.DownloadedAt >= TimeUtils.LastWeekStart && d.DownloadedAt < TimeUtils.ThisWeekStart),
                    recordBestLapTime = x.Type == PlayerCreationType.TRACK && x.IsMNR
                        ? x.Scores.Where(s => s.SubGroupId != 700).OrderBy(s => s.BestLapTime).Select(s => (float?)s.BestLapTime).FirstOrDefault()
                        : null,
                    recordScore = x.Type == PlayerCreationType.TRACK && !x.IsMNR
                        ? x.Scores.Max(s => (float?)s.Points)
                        : null,
                    recordFinishTime = x.Type == PlayerCreationType.TRACK && !x.IsMNR
                        ? x.Scores.Max(s => (float?)s.FinishTime)
                        : null,
                    recordLongestDrift = x.LongestDrift,
                    recordLongestHangTime = x.LongestHangTime
                })
                .ToList();

            if (creations.Count == 0)                
                return NotFound(new { error = "error_creation_not_found"});

            return Json(creations.Select(x => new
            {
                x.PlayerCreationId,
                x.Name,
                x.Description,
                rating = (x.rating ?? 0).ToString("0.0", CultureInfo.InvariantCulture),
                creatorUsername = x.Username,
                Type = x.Type.ToString(),
                x.Tags,
                Platform = x.Platform.ToString(),
                x.IsMNR,
                x.CreatedAt,
                downloads = new
                {
                    all_time = x.downloadsAllTime,
                    this_week = x.downloadsThisWeek,
                    last_week = x.downloadsLastWeek
                },
                records = x.Type == PlayerCreationType.TRACK
                    ? x.IsMNR
                        ? (object)new { bestLapTime = x.recordBestLapTime, longestDrift = x.recordLongestDrift, longestHangTime = x.recordLongestHangTime }
                        : new { score = x.recordScore, finishTime = x.recordFinishTime }
                    : null
            }));
        }

        [HttpGet]
        [Route("/api/creations/{username}")]
        public IActionResult GetCreationsByUsername(string username)
        {
            var creations = database.PlayerCreations
                .AsNoTracking()
                .Where(x => x.Author.Username == username
                && x.Type != PlayerCreationType.DELETED
                && x.ModerationStatus != ModerationStatus.BANNED
                && x.ModerationStatus != ModerationStatus.ILLEGAL)
                .Select(x => new
                {
                    x.PlayerCreationId,
                    x.Name,
                    x.Description,
                    rating = x.Ratings.Count != 0 ? (float?)x.Ratings.Average(r => r.Rating) : 0,
                    x.Author.Username,
                    x.Type,
                    x.Tags,
                    x.Platform,
                    x.IsMNR,
                    x.CreatedAt,
                    downloadsAllTime = x.Downloads.Count,
                    downloadsThisWeek = x.Downloads.Count(d => d.DownloadedAt >= TimeUtils.ThisWeekStart),
                    downloadsLastWeek = x.Downloads.Count(d => d.DownloadedAt >= TimeUtils.LastWeekStart && d.DownloadedAt < TimeUtils.ThisWeekStart),
                    recordBestLapTime = x.Type == PlayerCreationType.TRACK && x.IsMNR
                        ? x.Scores.Where(s => s.SubGroupId != 700).OrderBy(s => s.BestLapTime).Select(s => (float?)s.BestLapTime).FirstOrDefault()
                        : null,
                    recordScore = x.Type == PlayerCreationType.TRACK && !x.IsMNR
                        ? x.Scores.Max(s => (float?)s.Points)
                        : null,
                    recordFinishTime = x.Type == PlayerCreationType.TRACK && !x.IsMNR
                        ? x.Scores.Max(s => (float?)s.FinishTime)
                        : null,
                    recordLongestDrift = x.LongestDrift,
                    recordLongestHangTime = x.LongestHangTime
                })
                .ToList();

            if (creations.Count == 0)
                return NotFound(new { error = "error_player_not_found"});

            return Json(creations.Select(x => new
            {
                x.PlayerCreationId,
                x.Name,
                x.Description,
                rating = (x.rating ?? 0).ToString("0.0", CultureInfo.InvariantCulture),
                creatorUsername = x.Username,
                Type = x.Type.ToString(),
                x.Tags,
                Platform = x.Platform.ToString(),
                x.IsMNR,
                x.CreatedAt,
                downloads = new
                {
                    all_time = x.downloadsAllTime,
                    this_week = x.downloadsThisWeek,
                    last_week = x.downloadsLastWeek
                },
                records = x.Type == PlayerCreationType.TRACK
                    ? x.IsMNR
                        ? (object)new { bestLapTime = x.recordBestLapTime, longestDrift = x.recordLongestDrift, longestHangTime = x.recordLongestHangTime }
                        : new { score = x.recordScore, finishTime = x.recordFinishTime }
                    : null
            }));
        }

        [HttpGet]
        [Route("/api/creation/{id}/comments")]
        public IActionResult GetCreationComments(int id)
        {
            var creation = database.PlayerCreations
                .AsNoTracking()
                .Where(x => x.PlayerCreationId == id)
                .Select(x => new { x.IsMNR })
                .FirstOrDefault();

            if (creation == null)
                return NotFound(new { error = "error_creation_not_found" });

            if (creation.IsMNR)
            {
                var mnrComments = database.PlayerCreationRatings
                    .AsNoTracking()
                    .Where(x => x.PlayerCreationId == id && x.Comment != null && x.Comment != "")
                    .OrderBy(x => x.RatedAt)
                    .Select(x => new
                    {
                        x.Id,
                        x.Comment,
                        CreatedAt = x.RatedAt,
                        x.Player.Username
                    })
                    .ToList();

                return Json(mnrComments);
            }
            else
            {
                var comments = database.PlayerCreationComments
                    .AsNoTracking()
                    .Where(x => x.PlayerCreationId == id)
                    .OrderBy(x => x.CreatedAt)
                    .Select(x => new
                    {
                        x.Id,
                        comment = x.Body,
                        x.CreatedAt,
                        x.Player.Username
                    })
                    .ToList();

                return Json(comments);
            }
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
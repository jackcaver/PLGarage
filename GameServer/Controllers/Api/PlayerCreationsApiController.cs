using System.Linq;
using System.Globalization;
using GameServer.Models.PlayerData;
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
                    pointsAllTime = x.Points.Sum(p => p.Amount),
                    pointsThisWeek = x.Points.Where(p => p.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount),
                    pointsLastWeek = x.Points.Where(p => p.CreatedAt >= TimeUtils.LastWeekStart && p.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount),
                    downloadsAllTime = x.Downloads.Count,
                    downloadsThisWeek = x.Downloads.Count(d => d.DownloadedAt >= TimeUtils.ThisWeekStart),
                    downloadsLastWeek = x.Downloads.Count(d => d.DownloadedAt >= TimeUtils.LastWeekStart && d.DownloadedAt < TimeUtils.ThisWeekStart),
                    viewsAllTime = x.Views.Count,
                    viewsThisWeek = x.Views.Count(v => v.ViewedAt >= TimeUtils.ThisWeekStart),
                    viewsLastWeek = x.Views.Count(v => v.ViewedAt >= TimeUtils.LastWeekStart && v.ViewedAt < TimeUtils.ThisWeekStart),
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
                points = new
                {
                    all_time = x.pointsAllTime,
                    this_week = x.pointsThisWeek,
                    last_week = x.pointsLastWeek
                },
                downloads = new
                {
                    all_time = x.downloadsAllTime,
                    this_week = x.downloadsThisWeek,
                    last_week = x.downloadsLastWeek
                },
                views = new
                {
                    all_time = x.viewsAllTime,
                    this_week = x.viewsThisWeek,
                    last_week = x.viewsLastWeek
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
                    pointsAllTime = x.Points.Sum(p => p.Amount),
                    pointsThisWeek = x.Points.Where(p => p.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount),
                    pointsLastWeek = x.Points.Where(p => p.CreatedAt >= TimeUtils.LastWeekStart && p.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount),
                    downloadsAllTime = x.Downloads.Count,
                    downloadsThisWeek = x.Downloads.Count(d => d.DownloadedAt >= TimeUtils.ThisWeekStart),
                    downloadsLastWeek = x.Downloads.Count(d => d.DownloadedAt >= TimeUtils.LastWeekStart && d.DownloadedAt < TimeUtils.ThisWeekStart),
                    viewsAllTime = x.Views.Count,
                    viewsThisWeek = x.Views.Count(v => v.ViewedAt >= TimeUtils.ThisWeekStart),
                    viewsLastWeek = x.Views.Count(v => v.ViewedAt >= TimeUtils.LastWeekStart && v.ViewedAt < TimeUtils.ThisWeekStart),
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

            return Json(new {
                totalResults,
                creations = creations.Select(x => new
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
                    points = new
                    {
                        all_time = x.pointsAllTime,
                        this_week = x.pointsThisWeek,
                        last_week = x.pointsLastWeek
                    },
                    downloads = new
                    {
                        all_time = x.downloadsAllTime,
                        this_week = x.downloadsThisWeek,
                        last_week = x.downloadsLastWeek
                    },
                    views = new
                    {
                        all_time = x.viewsAllTime,
                        this_week = x.viewsThisWeek,
                        last_week = x.viewsLastWeek
                    },
                    records = x.Type == PlayerCreationType.TRACK
                        ? x.IsMNR
                            ? (object)new { bestLapTime = x.recordBestLapTime, longestDrift = x.recordLongestDrift, longestHangTime = x.recordLongestHangTime }
                            : new { score = x.recordScore, finishTime = x.recordFinishTime }
                        : null
                })
            });
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
                    pointsAllTime = x.Points.Sum(p => p.Amount),
                    pointsThisWeek = x.Points.Where(p => p.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount),
                    pointsLastWeek = x.Points.Where(p => p.CreatedAt >= TimeUtils.LastWeekStart && p.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount),
                    downloadsAllTime = x.Downloads.Count,
                    downloadsThisWeek = x.Downloads.Count(d => d.DownloadedAt >= TimeUtils.ThisWeekStart),
                    downloadsLastWeek = x.Downloads.Count(d => d.DownloadedAt >= TimeUtils.LastWeekStart && d.DownloadedAt < TimeUtils.ThisWeekStart),
                    viewsAllTime = x.Views.Count,
                    viewsThisWeek = x.Views.Count(v => v.ViewedAt >= TimeUtils.ThisWeekStart),
                    viewsLastWeek = x.Views.Count(v => v.ViewedAt >= TimeUtils.LastWeekStart && v.ViewedAt < TimeUtils.ThisWeekStart),
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
                points = new
                {
                    all_time = x.pointsAllTime,
                    this_week = x.pointsThisWeek,
                    last_week = x.pointsLastWeek
                },
                downloads = new
                {
                    all_time = x.downloadsAllTime,
                    this_week = x.downloadsThisWeek,
                    last_week = x.downloadsLastWeek
                },
                views = new
                {
                    all_time = x.viewsAllTime,
                    this_week = x.viewsThisWeek,
                    last_week = x.viewsLastWeek
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
            var q = database.PlayerCreations
                .AsNoTracking()
                .Where(x => x.Author.Username == username
                && x.Type != PlayerCreationType.DELETED
                && x.ModerationStatus != ModerationStatus.BANNED
                && x.ModerationStatus != ModerationStatus.ILLEGAL);

            var totalResults = q.Count();

            var creations = q
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
                    pointsAllTime = x.Points.Sum(p => p.Amount),
                    pointsThisWeek = x.Points.Where(p => p.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount),
                    pointsLastWeek = x.Points.Where(p => p.CreatedAt >= TimeUtils.LastWeekStart && p.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount),
                    downloadsAllTime = x.Downloads.Count,
                    downloadsThisWeek = x.Downloads.Count(d => d.DownloadedAt >= TimeUtils.ThisWeekStart),
                    downloadsLastWeek = x.Downloads.Count(d => d.DownloadedAt >= TimeUtils.LastWeekStart && d.DownloadedAt < TimeUtils.ThisWeekStart),
                    viewsAllTime = x.Views.Count,
                    viewsThisWeek = x.Views.Count(v => v.ViewedAt >= TimeUtils.ThisWeekStart),
                    viewsLastWeek = x.Views.Count(v => v.ViewedAt >= TimeUtils.LastWeekStart && v.ViewedAt < TimeUtils.ThisWeekStart),
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

            return Json(new {
                totalResults,
                creations = creations.Select(x => new
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
                    points = new
                    {
                        all_time = x.pointsAllTime,
                        this_week = x.pointsThisWeek,
                        last_week = x.pointsLastWeek
                    },
                    downloads = new
                    {
                        all_time = x.downloadsAllTime,
                        this_week = x.downloadsThisWeek,
                        last_week = x.downloadsLastWeek
                    },
                    views = new
                    {
                        all_time = x.viewsAllTime,
                        this_week = x.viewsThisWeek,
                        last_week = x.viewsLastWeek
                    },
                    records = x.Type == PlayerCreationType.TRACK
                        ? x.IsMNR
                            ? (object)new { bestLapTime = x.recordBestLapTime, longestDrift = x.recordLongestDrift, longestHangTime = x.recordLongestHangTime }
                            : new { score = x.recordScore, finishTime = x.recordFinishTime }
                        : null
                })
            });
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

        [HttpGet]
        [Route("/api/topmods")]
        public IActionResult GetTopMods([FromQuery] Platform platform = Platform.PS3)
        {
            return JsonTopCreations(PlayerCreationType.CHARACTER, platform);
        }

        [HttpGet]
        [Route("/api/topkarts")]
        public IActionResult GetTopKarts([FromQuery] Platform platform = Platform.PS3)
        {
            return JsonTopCreations(PlayerCreationType.KART, platform);
        }

        [HttpGet]
        [Route("/api/toptracks")]
        public IActionResult GetTopTracks([FromQuery] Platform platform = Platform.PS3)
        {
            return JsonTopCreations(PlayerCreationType.TRACK, platform);
        }

        [HttpGet]
        [Route("/api/mosthearted")]
        public IActionResult GetMostHeartedTracks([FromQuery] Platform platform = Platform.PS3)
        {
            var query = database.PlayerCreations
                .AsNoTracking()
                .Where(x => x.Platform == platform
                    && !x.IsMNR
                    && x.Type == PlayerCreationType.TRACK
                    && x.Type != PlayerCreationType.DELETED
                    && x.Type != PlayerCreationType.STORY
                    && x.ModerationStatus != ModerationStatus.BANNED
                    && x.ModerationStatus != ModerationStatus.ILLEGAL)
                .OrderByDescending(x => x.HeartsCount)
                .ThenByDescending(x => x.CreatedAt);

            var total = query.Count();

            var creations = query
                .Take(5)
                .Select(x => new
                {
                    id = x.PlayerCreationId,
                    x.Name,
                    x.Description,
                    x.Type,
                    creatorUsername = x.Author.Username,
                    platform = x.Platform.ToString(),
                    x.Tags,
                    x.CreatedAt,
                    x.UpdatedAt,
                    hearts = x.HeartsCount,
                    rating = x.RatingUp,
                    racesStarted = x.RacesStartedCount,
                    recordScore = x.Scores.Max(s => (float?)s.Points),
                    recordFinishTime = x.Scores.Max(s => (float?)s.FinishTime),
                })
                .ToList();

            return Json(creations.Select(x => new
            {
                x.id,
                x.Name,
                x.Description,
                Type = x.Type.ToString(),
                x.creatorUsername,
                x.platform,
                x.Tags,
                x.CreatedAt,
                x.UpdatedAt,
                x.hearts,
                x.rating,
                x.racesStarted,
                records = new
                {
                    score = x.recordScore,
                    finishTime = x.recordFinishTime
                }
            }));
        }

        [HttpGet]
        [Route("/api/teampicks")]
        public IActionResult GetTeamPicks(int page = 1, int pageSize = 10)
        {
            var query = database.PlayerCreations
                .AsNoTracking()
                .Where(x => x.IsTeamPick
                    && x.Type != PlayerCreationType.DELETED
                    && x.Type != PlayerCreationType.STORY
                    && x.ModerationStatus != ModerationStatus.BANNED
                    && x.ModerationStatus != ModerationStatus.ILLEGAL)
                .OrderByDescending(x => x.UpdatedAt);

            var totalResults = query.Count();

            var creations = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    id = x.PlayerCreationId,
                    x.Name,
                    x.Description,
                    Type = x.Type.ToString(),
                    creatorUsername = x.Author.Username,
                    platform = x.Platform.ToString(),
                    isMnr = x.IsMNR,
                    x.Tags,
                    x.CreatedAt,
                    x.UpdatedAt,
                    hearts = x.HeartsCount,
                })
                .ToList();

            return Json(new
            {
                totalResults,
                creations
            });
        }

        private IActionResult JsonTopCreations(
            PlayerCreationType playerCreationType,
            Platform platform)
        {
            var query = database.PlayerCreations
                .AsNoTracking()
                .Where(x => x.Platform == platform
                    && x.IsMNR
                    && x.Type == playerCreationType
                    && x.Type != PlayerCreationType.DELETED
                    && x.Type != PlayerCreationType.STORY
                    && x.ModerationStatus != ModerationStatus.BANNED
                    && x.ModerationStatus != ModerationStatus.ILLEGAL);

            query = query.OrderByDescending(x => x.PointsToday);

            var total = query.Count();

            var creations = query
                .Take(5)
                .Select(x => new
                {
                    id = x.PlayerCreationId,
                    x.Name,
                    x.Description,
                    x.Type,
                    creatorUsername = x.Author.Username,
                    platform = x.Platform.ToString(),
                    isMnr = x.IsMNR,
                    x.Tags,
                    x.CreatedAt,
                    x.UpdatedAt,
                    pointsToday = x.PointsToday,
                    points = x.PointsAmount,
                    pointsThisWeek = x.PointsThisWeek,
                    pointsLastWeek = x.PointsLastWeek,
                    downloads = x.DownloadsCount,
                    downloadsThisWeek = x.DownloadsThisWeek,
                    downloadsLastWeek = x.DownloadsLastWeek,
                    views = x.ViewsCount,
                    viewsThisWeek = x.ViewsThisWeek,
                    viewsLastWeek = x.ViewsLastWeek,
                    hearts = x.HeartsCount,
                    racesStarted = x.RacesStartedCount,
                    longestDrift = x.LongestDrift,
                    longestHangTime = x.LongestHangTime,
                    recordBestLapTime = x.Type == PlayerCreationType.TRACK && x.IsMNR
                        ? x.Scores.Where(s => s.SubGroupId != 700).OrderBy(s => s.BestLapTime).Select(s => (float?)s.BestLapTime).FirstOrDefault()
                        : null,
                    recordScore = x.Type == PlayerCreationType.TRACK && !x.IsMNR
                        ? x.Scores.Max(s => (float?)s.Points)
                        : null,
                    recordFinishTime = x.Type == PlayerCreationType.TRACK && !x.IsMNR
                        ? x.Scores.Max(s => (float?)s.FinishTime)
                        : null,
                    ratingValue = x.Ratings.Count != 0 ? (float?)x.Ratings.Average(r => r.Rating) : 0
                })
                .ToList();

            return Json(creations.Select(x => new
            {
                x.id,
                x.Name,
                x.Description,
                Type = x.Type.ToString(),
                x.creatorUsername,
                x.platform,
                x.isMnr,
                x.Tags,
                x.CreatedAt,
                x.UpdatedAt,
                x.hearts,
                x.racesStarted,
                x.longestDrift,
                x.longestHangTime,
                points = new
                {
                    today = x.pointsToday,
                    all_time = x.points,
                    this_week = x.pointsThisWeek,
                    last_week = x.pointsLastWeek
                },
                downloads = new
                {
                    all_time = x.downloads,
                    this_week = x.downloadsThisWeek,
                    last_week = x.downloadsLastWeek
                },
                views = new
                {
                    all_time = x.views,
                    this_week = x.viewsThisWeek,
                    last_week = x.viewsLastWeek
                },
                records = x.Type == PlayerCreationType.TRACK
                    ? x.isMnr
                        ? (object)new
                        {
                            bestLapTime = x.recordBestLapTime,
                            x.longestDrift,
                            x.longestHangTime
                        }
                        : new
                        {
                            score = x.recordScore,
                            finishTime = x.recordFinishTime
                        }
                    : null,
                rating = (x.ratingValue ?? 0).ToString("0.0", CultureInfo.InvariantCulture)
            }));
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
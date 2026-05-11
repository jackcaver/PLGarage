using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Controllers.Api
{
    [ApiController]
    [Route("api/score")]
    public class ScoreApiController(Database database) : Controller
    {
        private const int TimeTrialGroup = 703;
        private const int RaceGroup = 701;

        [HttpGet]
        public IActionResult Leaderboard(
            [FromQuery] int trackId,
            [FromQuery] Platform? platform = null,
            [FromQuery] string sortBy = null,
            [FromQuery] bool? mnr = null,
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10)
        {
            if (page < 1) page = 1;
            if (perPage < 1) perPage = 10;
            if (perPage > 10) perPage = 10;

            PlayerCreationData track = database.PlayerCreations
                .AsNoTracking()
                .Include(c => c.Author)
                .Include(c => c.Ratings)
                .FirstOrDefault(c => c.PlayerCreationId == trackId);

            if (track == null)
                return NotFound(new { error = "error_creation_not_found", trackId });

            var dto = new ScoreSnapshotDto
            {
                track = new ScoreTrackDto
                {
                    id = track.PlayerCreationId,
                    name = track.Name,
                    rating = track.StarRating,
                    creatorId = track.Author?.UserId,
                    creatorUsername = track.Author?.Username
                }
            };

            var baseQ = database.Scores
                .AsNoTracking()
                .Include(s => s.User)
                .Where(s => s.SubKeyId == trackId);

            var mnrQ = baseQ
                .Where(s => s.IsMNR)
                .Where(s => s.SubGroupId == TimeTrialGroup);

            IQueryable<Score> q;

            if (mnr == true)
            {
                q = mnrQ;

                if (platform == null)
                {
                    platform = q
                        .GroupBy(s => s.Platform)
                        .OrderByDescending(g => g.Count())
                        .Select(g => (Platform?)g.Key)
                        .FirstOrDefault();
                }

                if (platform != null)
                    q = q.Where(s => s.Platform == platform.Value);

                sortBy ??= "bestLapTime";
            }
            else if (mnr == false)
            {
                q = baseQ
                    .Where(s => !s.IsMNR)
                    .Where(s => s.SubGroupId == RaceGroup);

                if (platform == null)
                {
                    platform = q
                        .GroupBy(s => s.Platform)
                        .OrderByDescending(g => g.Count())
                        .Select(g => (Platform?)g.Key)
                        .FirstOrDefault();
                }

                if (platform != null)
                    q = q.Where(s => s.Platform == platform.Value);

                sortBy ??= "finishTime";
            }
            else if (mnrQ.Any())
            {
                q = mnrQ;

                if (platform == null)
                {
                    platform = q
                        .GroupBy(s => s.Platform)
                        .OrderByDescending(g => g.Count())
                        .Select(g => (Platform?)g.Key)
                        .FirstOrDefault();
                }

                if (platform != null)
                    q = q.Where(s => s.Platform == platform.Value);

                sortBy ??= "bestLapTime";
            }
            else
            {
                q = baseQ
                    .Where(s => !s.IsMNR)
                    .Where(s => s.SubGroupId == RaceGroup);

                if (platform == null)
                {
                    platform = q
                        .GroupBy(s => s.Platform)
                        .OrderByDescending(g => g.Count())
                        .Select(g => (Platform?)g.Key)
                        .FirstOrDefault();
                }

                if (platform != null)
                    q = q.Where(s => s.Platform == platform.Value);

                sortBy ??= "finishTime";
            }

            if (sortBy == "score")
                q = q.OrderByDescending(s => s.Points).ThenBy(s => s.Id);
            else if (sortBy == "finishTime")
                q = q.OrderBy(s => s.FinishTime).ThenBy(s => s.UpdatedAt).ThenBy(s => s.Id);
            else if (sortBy == "bestLapTime")
                q = q.OrderBy(s => s.BestLapTime).ThenBy(s => s.UpdatedAt).ThenBy(s => s.Id);
            else if (sortBy == "id")
                q = q.OrderBy(s => s.Id);

            dto.platform = platform?.ToString();
            dto.total = q.Count();
            dto.scores = GetTopScores(q, page, perPage);

            return Ok(dto);
        }

        private List<ScoreEntryDto> GetTopScores(IQueryable<Score> q, int page, int perPage)
        {
            var skip = (page - 1) * perPage;

            var rows = q.Skip(skip).Take(perPage)
                .Select(s => new
                {
                    s.Id,
                    s.PlayerId,
                    s.User.Username,
                    s.Points,
                    s.BestLapTime,
                    s.FinishTime,
                    s.UpdatedAt
                })
                .ToList();

            var result = new List<ScoreEntryDto>(rows.Count);
            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                result.Add(new ScoreEntryDto
                {
                    rank = skip + i + 1,
                    id = r.Id,
                    playerId = r.PlayerId,
                    playerUsername = r.Username,
                    score = r.Points,
                    bestLapTime = r.BestLapTime,
                    finishTime = r.FinishTime,
                    updatedAt = r.UpdatedAt
                });
            }

            return result;
        }

        [HttpGet("/api/leaderboards/creations")]
        public IActionResult CreationLeaderboard(
            [FromQuery] PlayerCreationType? type = null,
            [FromQuery] string sortBy = "xp",
            [FromQuery] Platform platform = Platform.PS3,
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10)
        {
            if (page < 1) page = 1;
            if (perPage < 1) perPage = 10;
            if (perPage > 10) perPage = 10;

            var filtered = database.PlayerCreations
                .AsNoTracking()
                .Where(c => c.IsMNR && c.Platform == platform && c.Type != PlayerCreationType.DELETED);

            if (type.HasValue)
                filtered = filtered.Where(c => c.Type == type.Value);

            var projected = filtered.Select(c => new
            {
                c.PlayerCreationId,
                c.Name,
                c.Type,
                c.PlayerId,
                c.Author.Username,
                Xp = c.Points.Sum(p => (int?)p.Amount) ?? 0,
                Downloads = c.Hearts.Count(),
                Views = c.Views.Count()
            });

            var sort = (sortBy ?? "xp").ToLowerInvariant();

            var orderedQuery = sort switch
            {
                "downloads" => projected.OrderByDescending(c => c.Downloads),
                "views" => projected.OrderByDescending(c => c.Views),
                _ => projected.OrderByDescending(c => c.Xp)
            };

            var skip = (page - 1) * perPage;
            var rows = orderedQuery
                .ThenBy(c => c.PlayerCreationId)
                .Skip(skip)
                .Take(perPage)
                .ToList();

            var results = rows.Select((c, idx) => new
            {
                rank = skip + idx + 1,
                creationId = c.PlayerCreationId,
                name = c.Name,
                type = c.Type.ToString(),
                creatorId = c.PlayerId,
                creatorUsername = c.Username,
                xp = c.Xp,
                downloads = c.Downloads,
                views = c.Views
            });

            return Ok(new
            {
                platform = platform.ToString(),
                type = type?.ToString(),
                sortBy = sort,
                total = filtered.Count(),
                results
            });
        }

        [HttpGet("/api/leaderboards/players")]
        public IActionResult PlayerLeaderboard(
            [FromQuery] string sortBy = "totalXp",
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10)
        {
            if (page < 1) page = 1;
            if (perPage < 1) perPage = 10;
            if (perPage > 10) perPage = 10;

            var filtered = database.Users
                .AsNoTracking()
                .Where(u => u.PlayedMNR && !u.IsBanned);

            var projected = filtered
                .Select(u => new
                {
                    u.UserId,
                    u.Username,
                    Races = u.RacesStarted.Count(),
                    Wins = u.RacesFinished.Count(r => r.IsWinner),
                    u.LongestWinStreak,
                    CurrentStreak = u.WinStreak,
                    Airtime = u.LongestHangTime,
                    Drift = u.LongestDrift,
                    RaceXp = u.PlayerExperiencePoints.Sum(p => (int?)p.Amount) ?? 0,
                    CreationXp = u.PlayerCreationPoints.Sum(p => (int?)p.Amount) ?? 0,
                    PointsCount = u.PlayerPoints.Count(),
                    PointsAverage = u.PlayerPoints.Average(p => (float?)p.Amount) ?? 0
                })
                .Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.Races,
                    u.Wins,
                    u.LongestWinStreak,
                    u.CurrentStreak,
                    u.Airtime,
                    u.Drift,
                    u.RaceXp,
                    u.CreationXp,
                    u.PointsCount,
                    u.PointsAverage,
                    SkillRating = u.PointsCount < 10
                        ? 1500
                        : u.PointsAverage < 0
                            ? 0
                            : u.PointsAverage > 3000
                                ? 3000
                                : (int)u.PointsAverage
                });

            var sort = (sortBy ?? "totalXp").ToLowerInvariant();

            var orderedQuery = sort switch
            {
                "onlineRaces" => projected.OrderByDescending(u => u.Races),
                "onlineWins" => projected.OrderByDescending(u => u.Wins),
                "longestWinStreak" => projected.OrderByDescending(u => u.LongestWinStreak),
                "longestHangTime" => projected.OrderByDescending(u => u.Airtime),
                "longestDrift" => projected.OrderByDescending(u => u.Drift),
                "skillRating" => projected.OrderByDescending(u => u.SkillRating),
                _ => projected.OrderByDescending(u => u.RaceXp + u.CreationXp)
            };

            var skip = (page - 1) * perPage;
            var rows = orderedQuery
                .ThenBy(u => u.UserId)
                .Skip(skip)
                .Take(perPage)
                .ToList();

            IEnumerable<object> results = rows.Select((u, i) => new
            {
                rank = skip + i + 1,
                playerId = u.UserId,
                username = u.Username,
                totalXp = u.RaceXp + u.CreationXp,
                raceXp = u.RaceXp,
                creationXp = u.CreationXp,
                onlineRaces = u.Races,
                onlineWins = u.Wins,
                longestWinStreak = u.LongestWinStreak,
                currentStreak = u.CurrentStreak,
                longestHangTime = u.Airtime,
                longestDrift = u.Drift,
                skillRating = u.SkillRating
            });

            return Ok(new
            {
                sortBy = sort,
                total = filtered.Count(),
                results
            });
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
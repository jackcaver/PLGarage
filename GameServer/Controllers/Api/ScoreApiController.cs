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
            [FromQuery] string sortBy = "time",
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

            IQueryable<Models.PlayerData.Score> q;

            if (mnrQ.Any())
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

                q = q.OrderBy(s => s.BestLapTime)
                     .ThenBy(s => s.UpdatedAt)
                     .ThenBy(s => s.Id);
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

                q = q.OrderBy(s => s.FinishTime)
                     .ThenBy(s => s.UpdatedAt)
                     .ThenBy(s => s.Id);
            }

            if (sortBy == "score")
                q = q.OrderByDescending(s => s.Points).ThenBy(s => s.Id);

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
                    s.PlayerId,
                    Username = s.User.Username,
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

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
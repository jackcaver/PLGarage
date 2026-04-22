using GameServer.Implementation.Common;
using GameServer.Models.Config;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Controllers.Api
{
    [ApiController]
    public class HotlapApiController : Controller
    {
        private readonly Database database;

        private const int HotlapSubGroupId = 700;

        public HotlapApiController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("/api/hotlap")]
        public IActionResult GetHotlap([FromQuery] int max = 10)
        {
            max = Math.Clamp(max, 0, 10);

            HotLapData hotlap = ContentUpdates.ReadHotlapData();
            if (hotlap == null)
                return NotFound(new { error = "error_no_hotlap_file" });

            PlayerCreationData track = database.PlayerCreations
                .AsNoTracking()
                .Include(c => c.Author)
                .Include(c => c.Ratings)
                .FirstOrDefault(c => c.PlayerCreationId == hotlap.TrackId);

            if (track == null)
                return NotFound(new { error = "error_creation_not_found", trackId = hotlap.TrackId });

            var dto = new HotlapSnapshotDto
            {
                track = new HotlapTrackDto
                {
                    id = track.PlayerCreationId,
                    name = track.Name,
                    rating = track.StarRating,
                    creatorId = track.Author?.UserId,
                    creatorUsername = track.Author?.Username
                },
                resetInSeconds = (int)TimeUtils.UntilNextDay.TotalSeconds,
                topTimes = max == 0 ? new List<HotlapTimeDto>() : GetTopTimes(track.PlayerCreationId, max)
            };

            return Ok(dto);
        }

        private List<HotlapTimeDto> GetTopTimes(int trackId, int limit)
        {
            var rows = database.Scores
                .AsNoTracking()
                .Include(s => s.User)
                .Where(s => s.IsMNR
                    && s.SubGroupId == HotlapSubGroupId
                    && s.SubKeyId == trackId)
                .OrderBy(s => s.BestLapTime)
                .ThenBy(s => s.UpdatedAt)
                .ThenBy(s => s.Id)
                .Take(limit)
                .Select(s => new { s.PlayerId, Username = s.User.Username, s.BestLapTime, s.UpdatedAt})
                .ToList();

            var result = new List<HotlapTimeDto>(rows.Count);
            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                result.Add(new HotlapTimeDto
                {
                    rank = i + 1,
                    playerId = r.PlayerId,
                    playerUsername = r.Username,
                    bestLapTime = r.BestLapTime,
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
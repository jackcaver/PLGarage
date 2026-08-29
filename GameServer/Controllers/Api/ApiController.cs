using GameServer.Implementation.Common;
using GameServer.Models.Config;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.PlayerData;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Controllers.Api
{
    public class ApiController(Database database) : Controller
    {
        [HttpGet]
        [Route("api/GetInstanceName")]
        public IActionResult GetInstanceName()
        {
            return Content(ServerConfig.Instance.InstanceName);
        }

        [Route("api/Gateway")]
        public async Task StartServerCommunication()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                Guid ServerID = Guid.Empty;
                if (Request.Headers.TryGetValue("server_id", out StringValues server_id))
                    ServerID = Guid.Parse(server_id);
                await ServerCommunication.HandleConnection(webSocket, ServerID);
            }
        }

        [HttpGet]
        [Route("api/VotePackage")]
        public IActionResult GetVotingOptions(int trackId, int playerCount = 1)
        {
            Guid ServerID = Guid.Empty;
            if (Request.Headers.TryGetValue("server_id", out StringValues server_id))
                ServerID = Guid.Parse(server_id);

            if (ServerCommunication.GetServer(ServerID) == null)
                return StatusCode(403);
            
            int maxHumansCap = database.PlayerCreations
                .Where(match => (match.Type == PlayerCreationType.TRACK || match.Type == PlayerCreationType.STORY) 
                    && !match.IsMNR && match.Platform == Platform.PS3)
                .OrderByDescending(c => c.MaxHumans)
                .Select(match => match.MaxHumans)
                .FirstOrDefault();
            
            if (playerCount > maxHumansCap)
                playerCount = maxHumansCap;

            List<int> trackIDs = [];

            Random random = new();
            var creations = database.PlayerCreations
                .AsNoTracking()
                .AsSplitQuery()
                .Where(match => (match.Type == PlayerCreationType.TRACK || match.Type == PlayerCreationType.STORY) 
                    && !match.IsMNR && match.PlayerCreationId != trackId && match.Platform == Platform.PS3
                    && playerCount <= match.MaxHumans)
                .Select(c => new { c.PlayerCreationId, c.Type, c.Coolness, c.IsTeamPick });

            var count = creations.Count(match => match.Type != PlayerCreationType.STORY);
            
            if (count > 30)
            {
                int take = (int)Math.Round(count * 0.3);
                
                var top = creations
                    .OrderByDescending(c => c.Coolness)
                    .Select(c => c.PlayerCreationId)
                    .Take(take);
                var bottom = creations
                    .OrderByDescending(c => c.IsTeamPick)
                    .ThenBy(c => c.Coolness)
                    .Select(c => c.PlayerCreationId)
                    .Take(take);

                int track1 = top.Skip(random.Next(top.Count() - 1)).FirstOrDefault();
                int track3 = bottom.Skip(random.Next(bottom.Count() - 1)).FirstOrDefault();

                var candidates = creations
                    .OrderByDescending(c => c.Coolness)
                    .Where(match => match.PlayerCreationId != track1 && match.PlayerCreationId != track3)
                    .Select(c => c.PlayerCreationId);
                
                int track2 = candidates.Skip(random.Next(candidates.Count() - 1)).FirstOrDefault();
                
                trackIDs.AddRange([track1, track2, track3]);
            }
            else if (count > 3)
                trackIDs.AddRange(creations.OrderByDescending(c => c.Coolness)
                    .Select(c => c.PlayerCreationId)
                    .Skip(random.Next(count - 3))
                    .Take(3).ToList());
            else
                trackIDs.AddRange(creations.Select(c => c.PlayerCreationId)
                    .Take(3).ToList());

            return Json(trackIDs);
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}

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
using Newtonsoft.Json;
using Microsoft.Extensions.Primitives;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Controllers.Api
{
    public class CommonApiController : Controller
    {
        private readonly Database database;

        public CommonApiController(Database database)
        {
            this.database = database;
        }

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
        public IActionResult GetVotingOptions(int trackId)
        {
            Guid ServerID = Guid.Empty;
            if (Request.Headers.TryGetValue("server_id", out StringValues server_id))
                ServerID = Guid.Parse(server_id);

            if (ServerCommunication.GetServer(ServerID) == null)
                return StatusCode(403);

            List<int> TrackIDs = [];

            Random random = new();
            var creations = database.PlayerCreations
                .Include(x => x.Downloads)
                .Where(match => match.Type == PlayerCreationType.TRACK && !match.IsMNR
                    && match.PlayerCreationId != trackId && match.Platform == Platform.PS3)
                .OrderByDescending(p => p.Downloads.Count)
                .Select(p => p.PlayerCreationId);

            var count = creations.Count();
            if (count > 3)
            {
                creations = creations.Skip(random.Next(count-3));
            }

            var list = creations.Take(3).ToList();
            foreach (var creation in list)
            {
                TrackIDs.Add(creation);
            }

            return Content(JsonConvert.SerializeObject(TrackIDs));
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}

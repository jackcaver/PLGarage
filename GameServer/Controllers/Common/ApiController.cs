using GameServer.Implementation.Common;
using GameServer.Models.Config;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Primitives;
using GameServer.Models.Common;

namespace GameServer.Controllers.Common
{
    public class ApiController : Controller
    {
        private readonly Database database;

        public ApiController(Database database)
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
                await ServerCommunicationImpl.HandleConnection(webSocket, ServerID);
            }
        }

        [HttpGet]
        [Route("api/VotePackage")]
        public IActionResult GetVotingOptions(int trackId)
        {
            Guid ServerID = Guid.Empty;
            if (Request.Headers.TryGetValue("server_id", out StringValues server_id))
                ServerID = Guid.Parse(server_id);

            if (ServerCommunicationImpl.GetServer(ServerID) == null)
                return StatusCode(403);

            List<int> TrackIDs = [];

            Random random = new();
            var creations = database.PlayerCreations.Where(match => match.Type == PlayerCreationType.TRACK && !match.IsMNR && match.PlayerCreationId != trackId && match.Platform == Platform.PS3).ToList();

            if (creations.Count <= 3)
            {
                foreach (var creation in creations)
                {
                    TrackIDs.Add(creation.PlayerCreationId);
                }
            }
            else
            {
                while (TrackIDs.Count < 3)
                {
                    var id = creations[random.Next(0, creations.Count - 1)].PlayerCreationId;
                    if (!TrackIDs.Contains(id))
                        TrackIDs.Add(id);
                }
            }

            return Content(JsonConvert.SerializeObject(TrackIDs));
        }

        [HttpGet]
        [Route("api/GetPSID")]
        public IActionResult GetPSID()
        {
            // TODO
            return Content(null);
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}

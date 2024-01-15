using GameServer.Implementation.Common;
using GameServer.Models.Config;
using GameServer.Models.ServerCommunication;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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

        [Route("api/ServerCommunication")]
        public async Task StartServerCommunication()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                Guid ServerID = Guid.Empty;
                if (Request.Headers.ContainsKey("server_id"))
                    ServerID = Guid.Parse(Request.Headers["server_id"]);
                await ServerCommunication.HandleConnection(database, webSocket, ServerID);
            }
        }
    }
}

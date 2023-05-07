using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GameServer.Models;
using GameServer.Models.Config;
using GameServer.Models.Config.ServerList;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace GameServer.Controllers.Common
{
    public class ServerController : Controller
    {
        private readonly Database database;

        public ServerController(Database database)
        {
            this.database = database;
        }

        [HttpPost]
        [Route("servers/select.xml")]
        public IActionResult ServerSelect(ServerType server_type, string server_version)
        {
            var session_uuid = Guid.NewGuid().ToString();
            Server server = ServerConfig.Instance.ServerList[server_type];
            var user = database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var resp = new Response<List<server>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<server> { new server {
                    server_type = server_type.ToString(),
                    address = server.Address,
                    port = server.Port,
                    session_uuid = session_uuid,
                    server_private_key = server.ServerPrivateKey,
                    ticket = new ticket {
                        session_uuid = session_uuid,
                        player_id = user.UserId,
                        username = user.Username,
                        expiration_date = DateTime.UtcNow.AddMinutes(2).ToString("ddd MMM dd hh:mm:ss zzz yyyy", CultureInfo.InvariantCulture.DateTimeFormat),//"Tue Oct 09 23:25:57 +0000 2023", 
                        signature = "98b93493e8beb1318533fb87897f1e80"
                    }
                } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GameServer.Implementation.Common;
using GameServer.Models;
using GameServer.Models.Response;
using GameServer.Models.ServerCommunication;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;

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
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            ServerInfo server = ServerCommunication.GetServer(server_type);

            if (user == null || server == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var resp = new Response<List<ServerResponse>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<ServerResponse> { new ServerResponse {
                    server_type = server_type.ToString(),
                    address = server.Address,
                    port = server.Port,
                    session_uuid = SessionID.ToString(),
                    server_private_key = server.ServerPrivateKey,
                    ticket = new TicketResponse {
                        session_uuid = SessionID.ToString(),
                        player_id = user.UserId,
                        username = user.Username,
                        expiration_date = session.ExpiryDate.ToString("ddd MMM dd hh:mm:ss zzz yyyy", CultureInfo.InvariantCulture.DateTimeFormat),//"Tue Oct 09 23:25:57 +0000 2023", 
                        signature = "98b93493e8beb1318533fb87897f1e80"
                    }
                } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
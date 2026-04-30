using System.Collections.Generic;
using System.Globalization;
using GameServer.Implementation.Common;
using GameServer.Models;
using GameServer.Models.Response;
using GameServer.Models.ServerCommunication;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Common
{
    public class ServerController(Database database) : Controller
    {
        [HttpPost]
        [Authorize]
        [Route("servers/select.xml")]
        public IActionResult ServerSelect(ServerType server_type, string server_version)
        {
            var session = Session.GetSession(database, User);
            var user = session.User;

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
                response = [ new ServerResponse {
                    server_type = server_type.ToString(),
                    address = server.Address,
                    port = server.Port,
                    session_uuid = session.SessionId.ToString(),
                    server_private_key = server.ServerPrivateKey,
                    ticket = new TicketResponse {
                        session_uuid = session.SessionId.ToString(),
                        player_id = user.UserId,
                        username = user.Username,
                        expiration_date = TimeUtils.Now.AddDays(1).ToString("ddd MMM dd hh:mm:ss zzz yyyy", CultureInfo.InvariantCulture.DateTimeFormat),
                        signature = "98b93493e8beb1318533fb87897f1e80"
                    }
                } ]
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
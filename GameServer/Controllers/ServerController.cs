using System;
using System.Collections.Generic;
using GameServer.Models;
using GameServer.Models.Config;
using GameServer.Models.Config.ServerList;
using GameServer.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace GameServer.Controllers
{
    public class ServerController : Controller
    {
        [HttpPost]
        [Route("servers/select.xml")]
        public IActionResult ServerSelect(ServerType server_type, string server_version)
        {
            Server server = ServerConfig.Instance.ServerList[server_type];
            Log.Information(server_type.ToString());
            var resp = new Response<List<server>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<server> { new server { 
                    server_type = server_type.ToString(), 
                    address = server.Address, 
                    port = server.Port, 
                    session_uuid = "9aa555a8-cc1a-11e8-81c9-22000acbd9b1", 
                    server_private_key = server.ServerPrivateKey, 
                    ticket = new ticket { 
                        session_uuid = "9aa555a8-cc1a-11e8-81c9-22000acbd9b1", 
                        player_id = 1, 
                        username = Request.Cookies["username"], 
                        expiration_date = "Tue Oct 09 23:25:57 +0000 2023", 
                        signature = "98b93493e8beb1318533fb87897f1e80" 
                    } 
                } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
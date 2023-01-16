using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers
{
    public class SessionController : Controller
    {
        private readonly Database database;

        public SessionController(Database database)
        {
            this.database = database;
        }

        [HttpPost]
        [Route("session/login_np.xml")]
        public IActionResult Login(Platform platform, string ticket, string hmac, string console_id) //TODO: add psn ticket reader
        {
            string username = Request.Cookies["username"];
            var user = this.database.Users.FirstOrDefault(match => match.Username == username);

            if (user == null || user.Username == "ufg")
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var resp = new Response<List<login_data>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<login_data> { 
                    new login_data { 
                        ip_address = HttpContext.Connection.RemoteIpAddress.ToString(), 
                        login_time = DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:sszzz"), 
                        platform = Platform.PS3.ToString(), 
                        player_id = user.UserId, 
                        player_name = user.Username, 
                        presence = user.Presence.ToString() 
                    } 
                }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("session/set_presence.xml")]
        public IActionResult SetPresence(string presence)
        {
            int id = -130;
            string username = Request.Cookies["username"], message = "The player doesn't exist";

            var user = this.database.Users.FirstOrDefault(match => match.Username == username);

            if (user != null)
            {
                id = 0;
                message = "Successful completion";
                Presence userPresence = Presence.OFFLINE;
                Enum.TryParse(presence.Split("\0")[0], out userPresence);
                user.Presence = userPresence;
                this.database.SaveChanges();
            }

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = id, message = message },
                response = new EmptyResponse {}
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("session/ping.xml")]
        public IActionResult Ping()
        {
            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse {}
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    } 
}
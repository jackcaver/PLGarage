using System;
using System.Collections.Generic;
using GameServer.Models;
using GameServer.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace GameServer.Controllers
{
    public class SessionController : Controller
    {
        [HttpPost]
        [Route("session/login_np.xml")]
        public IActionResult Login(string platform, string ticket, string hmac, string console_id)//TODO: add psn ticket reader and 
        {
            var resp = new Response<List<login_data>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<login_data> { new login_data { ip_address = HttpContext.Connection.RemoteIpAddress.ToString(), login_time = DateTime.Now.ToString(), platform = "PS3", player_id = 1, player_name = Request.Cookies["username"], presence = "ONLINE" } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("session/set_presence.xml")]
        public IActionResult SetPresence(string presence)
        {
            Log.Information($"{Request.Cookies["username"]}: has set status to {presence}");
            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
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
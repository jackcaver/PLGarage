using System.Collections.Generic;
using GameServer.Models;
using GameServer.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player
{
    public class BuddiesController : Controller
    {
        [HttpPost]
        [Route("buddies/replicate.xml")]
        public IActionResult Replicate(List<string> usernames, List<string> blocked_usernames)
        {
            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
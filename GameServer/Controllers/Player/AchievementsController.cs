using System.Collections.Generic;
using GameServer.Models;
using GameServer.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player
{
    public class AchievementsController : Controller
    {
        [Route("achievements.xml")]
        public IActionResult Get()
        {
            var resp = new Response<List<achievements>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<achievements> { new achievements { total = 0, AchievementList = new List<achievement> { } } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
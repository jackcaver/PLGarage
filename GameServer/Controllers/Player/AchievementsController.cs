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
            var resp = new Response<List<Achievements>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<Achievements> { new Achievements { total = 0, AchievementList = new List<Achievement> { } } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
using System.Collections.Generic;
using GameServer.Models;
using GameServer.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Common
{
    public class PlayerGlickosController : Controller
    {
        [HttpGet]
        [Route("player_glickos/bulk_fetch.xml")]
        public IActionResult BulkFetch(int player_ids)
        {
            var resp = new Response<List<player_metrics>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_metrics> { new player_metrics { total = 0 } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
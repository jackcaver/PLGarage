using GameServer.Models;
using GameServer.Models.Request;
using GameServer.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace GameServer.Controllers
{
    public class ModerationController : Controller
    {
        [HttpPost]
        [Route("grief_report.xml")]
        public IActionResult GriefReport(GriefReport grief_report) 
        {
            Log.Information(grief_report.reason);
            var resp = new Response<PlayerIDResponse> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new PlayerIDResponse { player_id = 1 }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
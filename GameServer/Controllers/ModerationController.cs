using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Linq;

namespace GameServer.Controllers
{
    public class ModerationController : Controller
    {
        private readonly Database database;

        public ModerationController(Database database)
        {
            this.database = database;
        }

        [HttpPost]
        [Route("grief_report.xml")]
        public IActionResult GriefReport(GriefReport grief_report) 
        {
            this.database.GriefReports.Add(new GriefReportData
            {
                BadRectTop = grief_report.bad_rect_data.top,
                BadRectBottom = grief_report.bad_rect_data.bottom,
                Comments = grief_report.comments,
                Context = grief_report.context,
                Reason = grief_report.reason 
            });
            this.database.SaveChanges();

            UserGeneratedContentUtils.SaveGriefReportData(this.database.GriefReports.Count(), 
                Request.Form.Files.GetFile("grief_report[data]").OpenReadStream(), 
                Request.Form.Files.GetFile("grief_report[preview]").OpenReadStream());
            var resp = new Response<EmptyResponse> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
using GameServer.Implementation.Common;
using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace GameServer.Controllers.Common
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
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            var session = Session.GetSession(SessionID);
            var user = this.database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            database.GriefReports.Add(new GriefReportData
            {
                UserId = user.UserId,
                BadRectTop = grief_report.bad_rect_data.top,
                BadRectBottom = grief_report.bad_rect_data.bottom,
                Comments = grief_report.comments,
                Context = grief_report.context,
                Reason = grief_report.reason
            });
            database.SaveChanges();

            UserGeneratedContentUtils.SaveGriefReportData(database.GriefReports.Count(),
                Request.Form.Files.GetFile("grief_report[data]").OpenReadStream(),
                Request.Form.Files.GetFile("grief_report[preview]").OpenReadStream());
            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
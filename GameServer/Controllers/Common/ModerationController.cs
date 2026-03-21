using GameServer.Implementation.Common;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;

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
            grief_report.data = Request.Form.Files.GetFile("grief_report[data]");
            grief_report.preview = Request.Form.Files.GetFile("grief_report[preview]");
            return Content(Moderation.GriefReport(database, SessionID, grief_report), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_complaints.xml")]
        public IActionResult PlayerComplaints(PlayerComplaint player_complaint)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(Moderation.PlayerComplaints(database, SessionID, player_complaint), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_complaints.xml")]
        public IActionResult PlayerCreationComplaints(PlayerCreationComplaint player_creation_complaint)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            player_creation_complaint.preview = Request.Form.Files.GetFile("player_creation_complaint[preview]");
            return Content(Moderation.PlayerCreationComplaints(database, SessionID, player_creation_complaint), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
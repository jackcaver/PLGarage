using GameServer.Implementation.Common;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Common
{
    public class ModerationController(Database database) : Controller
    {
        [HttpPost]
        [Authorize]
        [Route("grief_report.xml")]
        public IActionResult GriefReport(GriefReport grief_report)
        {
            var user = Session.GetUser(database, User);
            grief_report.data = Request.Form.Files.GetFile("grief_report[data]");
            grief_report.preview = Request.Form.Files.GetFile("grief_report[preview]");
            return Content(Moderation.GriefReport(database, user, grief_report), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("player_complaints.xml")]
        public IActionResult PlayerComplaints(PlayerComplaint player_complaint)
        {
            var user = Session.GetUser(database, User);
            return Content(Moderation.PlayerComplaints(database, user, player_complaint), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("player_creation_complaints.xml")]
        public IActionResult PlayerCreationComplaints(PlayerCreationComplaint player_creation_complaint)
        {
            var user = Session.GetUser(database, User);
            player_creation_complaint.preview = Request.Form.Files.GetFile("player_creation_complaint[preview]");
            return Content(Moderation.PlayerCreationComplaints(database, user, player_creation_complaint), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
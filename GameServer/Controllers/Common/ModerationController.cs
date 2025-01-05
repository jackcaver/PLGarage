using GameServer.Implementation.Common;
using GameServer.Models.PlayerData.PlayerCreations;
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
            return Content(ModerationImpl.GriefReport(database, SessionID, grief_report), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_complaints.xml")]
        public IActionResult PlayerComplaints(PlayerComplaint player_complaint)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(ModerationImpl.PlayerComplaints(database, SessionID, player_complaint), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_complaints.xml")]
        public IActionResult PlayerCreationComplaints(PlayerCreationComplaint player_creation_complaint)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            player_creation_complaint.preview = Request.Form.Files.GetFile("player_creation_complaint[preview]");
            return Content(ModerationImpl.PlayerCreationComplaints(database, SessionID, player_creation_complaint), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("/moderation/login")]
        public IActionResult Login(string login, string password)
        {
            Guid sessionID = Guid.NewGuid();
            Response.Cookies.Append("moderation_session_id", sessionID.ToString());
            return Content(ModerationImpl.Login(database, sessionID, login, password));
        }

        [HttpGet]
        [Route("/moderation/grief_reports")]
        public IActionResult GetGriefReports(string context, int? from)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("moderation_session_id"))
                SessionID = Guid.Parse(Request.Cookies["moderation_session_id"]);

            if (!ModerationImpl.IsLoggedIn(SessionID)) return StatusCode(403);

            return Content(ModerationImpl.GetGriefReports(database, context, from));
        }

        [HttpGet]
        [Route("/moderation/grief_reports/{id}")]
        public IActionResult GetGriefReport(int id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("moderation_session_id"))
                SessionID = Guid.Parse(Request.Cookies["moderation_session_id"]);

            if (!ModerationImpl.IsLoggedIn(SessionID)) return StatusCode(403);

            var report = ModerationImpl.GetGriefReport(database, id);

            if (report == null)
                return NotFound();
            else
                return Content(report);
        }

        [HttpGet]
        [Route("/moderation/grief_reports/{id}/data.xml")]
        public IActionResult GetGriefReportDataFile(int id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("moderation_session_id"))
                SessionID = Guid.Parse(Request.Cookies["moderation_session_id"]);

            if (!ModerationImpl.IsLoggedIn(SessionID)) return StatusCode(403);

            var file = UserGeneratedContentUtils.LoadGriefReportData(id, "data.xml");

            if (file != null)
                return File(file, "application/xml;charset=utf-8");
            else
                return NotFound();
        }

        [HttpGet]
        [Route("/moderation/grief_reports/{id}/preview.png")]
        public IActionResult GetGriefReportPreview(int id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("moderation_session_id"))
                SessionID = Guid.Parse(Request.Cookies["moderation_session_id"]);

            if (!ModerationImpl.IsLoggedIn(SessionID)) return StatusCode(403);

            var file = UserGeneratedContentUtils.LoadGriefReportData(id, "preview.png");

            if (file != null)
                return File(file, "image/png");
            else
                return NotFound();
        }

        [HttpPost]
        [Route("/moderation/setStatus")]
        public IActionResult SetModerationStatus(int id, ModerationStatus status)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("moderation_session_id"))
                SessionID = Guid.Parse(Request.Cookies["moderation_session_id"]);

            if (!ModerationImpl.IsLoggedIn(SessionID)) return StatusCode(403);

            var result = ModerationImpl.SetModerationStatus(database, id, status);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }

        [HttpPost]
        [Route("/moderation/setBan")]
        public IActionResult SetBan(int id, bool isBanned)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("moderation_session_id"))
                SessionID = Guid.Parse(Request.Cookies["moderation_session_id"]);

            if (!ModerationImpl.IsLoggedIn(SessionID)) return StatusCode(403);

            var result = ModerationImpl.SetBan(database, id, isBanned);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }

        [HttpGet]
        [Route("/moderation/player_complaints")]
        public IActionResult GetPlayerComplaints(int? from, int? playerID)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("moderation_session_id"))
                SessionID = Guid.Parse(Request.Cookies["moderation_session_id"]);

            if (!ModerationImpl.IsLoggedIn(SessionID)) return StatusCode(403);

            return Content(ModerationImpl.GetPlayerComplaints(database, from, playerID));
        }

        [HttpGet]
        [Route("/moderation/player_complaints/{id}")]
        public IActionResult GetPlayerComplaint(int id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("moderation_session_id"))
                SessionID = Guid.Parse(Request.Cookies["moderation_session_id"]);

            if (!ModerationImpl.IsLoggedIn(SessionID)) return StatusCode(403);

            var report = ModerationImpl.GetPlayerComplaint(database, id);

            if (report == null)
                return NotFound();
            else
                return Content(report);
        }

        [HttpGet]
        [Route("/moderation/player_creation_complaints")]
        public IActionResult GetPlayerCreationComplaints(int? from, int? playerID, int? playerCreationID)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("moderation_session_id"))
                SessionID = Guid.Parse(Request.Cookies["moderation_session_id"]);

            if (!ModerationImpl.IsLoggedIn(SessionID)) return StatusCode(403);

            return Content(ModerationImpl.GetPlayerCreationComplaints(database, from, playerID, playerCreationID));
        }

        [HttpGet]
        [Route("/moderation/player_creation_complaints/{id}")]
        public IActionResult GetPlayerCreationComplaint(int id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("moderation_session_id"))
                SessionID = Guid.Parse(Request.Cookies["moderation_session_id"]);

            if (!ModerationImpl.IsLoggedIn(SessionID)) return StatusCode(403);

            var report = ModerationImpl.GetPlayerCreationComplaint(database, id);

            if (report == null)
                return NotFound();
            else
                return Content(report);
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
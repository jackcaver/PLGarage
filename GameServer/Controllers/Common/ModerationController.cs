using GameServer.Implementation.Common;
using GameServer.Models.Moderation;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace GameServer.Controllers.Common
{
    public class ModerationController : Controller
    {
        private readonly Database database;

        public ModerationController(Database database)
        {
            this.database = database;
        }

        #region Game
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
        #endregion

        #region ModeratorSelf
        [HttpPost]
        [Route("/moderation/login")]
        public IActionResult Login(string login, string password)
        {
            var result = Moderation.Login(database, login, password);

            if (result == null)
                return Content("error");
            else
                Response.Cookies.Append("Token", result);

            return Content("ok");
        }

        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/permissions")]
        public IActionResult GetPermissions()
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (int.TryParse(uidString, out int userID))
                return Content(Moderation.GetPermissions(database, userID));
            else
                return NotFound();
        }

        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/set_username")]
        public IActionResult SetUsername(string username)
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (int.TryParse(uidString, out int userID))
                return Content(Moderation.SetUsername(database, userID, username));
            else
                return NotFound();
        }

        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/set_password")]
        public IActionResult SetPassword(string password)
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (int.TryParse(uidString, out int userID))
                return Content(Moderation.SetPassword(database, userID, password));
            else
                return NotFound();
        }
        #endregion

        #region GriefReports
        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/grief_reports")]
        public IActionResult GetGriefReports(int page, int per_page, string context, int? from)
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (!(int.TryParse(uidString, out int userID)
                && database.Moderators.Any(match => match.ID == userID && match.ViewGriefReports)))
                return StatusCode(403);

            return Content(Moderation.GetGriefReports(database, page, per_page, context, from));
        }

        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/grief_reports/{id}")]
        public IActionResult GetGriefReport(int id)
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (!(int.TryParse(uidString, out int userID)
                && database.Moderators.Any(match => match.ID == userID && match.ViewGriefReports)))
                return StatusCode(403);

            var report = Moderation.GetGriefReport(database, id);

            if (report == null)
                return NotFound();
            else
                return Content(report);
        }

        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/grief_reports/{id}/data.xml")]
        public IActionResult GetGriefReportDataFile(int id)
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (!(int.TryParse(uidString, out int userID)
                && database.Moderators.Any(match => match.ID == userID && match.ViewGriefReports)))
                return StatusCode(403);

            var file = UserGeneratedContentUtils.LoadGriefReportData(id, "data.xml");

            if (file != null)
                return File(file, "application/xml;charset=utf-8");
            else
                return NotFound();
        }

        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/grief_reports/{id}/preview.png")]
        public IActionResult GetGriefReportPreview(int id)
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (!(int.TryParse(uidString, out int userID)
                && database.Moderators.Any(match => match.ID == userID && match.ViewGriefReports)))
                return StatusCode(403);

            var file = UserGeneratedContentUtils.LoadGriefReportData(id, "preview.png");

            if (file != null)
                return File(file, "image/png");
            else
                return NotFound();
        }
        #endregion

        #region CreationManagement
        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/setStatus")]
        public IActionResult SetModerationStatus(int id, ModerationStatus status)
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (!(int.TryParse(uidString, out int userID)
                && database.Moderators.Any(match => match.ID == userID && match.ChangeCreationStatus)))
                return StatusCode(403);

            var result = Moderation.SetModerationStatus(database, id, status);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }

        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/player_creations")]
        public IActionResult GetPlayerCreationsWithStatus(int page, int per_page, ModerationStatus status)
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (!(int.TryParse(uidString, out int userID)
                && database.Moderators.Any(match => match.ID == userID && match.ChangeCreationStatus)))
                return StatusCode(403);

            return Content(Moderation.GetPlayerCreationsWithStatus(database, page, per_page, status));
        }
        #endregion

        #region UserManagement
        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/setBan")]
        public IActionResult SetBan(int id, bool isBanned)
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (!(int.TryParse(uidString, out int userID)
                && database.Moderators.Any(match => match.ID == userID && match.BanUsers)))
                return StatusCode(403);

            var result = Moderation.SetBan(database, id, isBanned);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }

        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/setUserSettings")]
        public IActionResult SetUserSettings(int id, bool ShowCreationsWithoutPreviews, bool AllowOppositePlatform)
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (!(int.TryParse(uidString, out int userID)
                && database.Moderators.Any(match => match.ID == userID && match.ChangeUserSettings)))
                return StatusCode(403);

            var result = Moderation.SetUserSettings(database, id, ShowCreationsWithoutPreviews, AllowOppositePlatform);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }

        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/users")]
        public IActionResult GetUsers(int page, int per_page, bool? PlayedMNR, bool? IsPSNLinked, bool? IsRPCNLinked)
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (!(int.TryParse(uidString, out int userID)
                && database.Moderators.Any(match => match.ID == userID 
                    && (match.BanUsers || match.ChangeUserSettings))))
                return StatusCode(403);

            return Content(Moderation.GetUsers(database, page, per_page, PlayedMNR, IsPSNLinked, IsRPCNLinked));
        }
        #endregion

        #region PlayerComplaints
        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/player_complaints")]
        public IActionResult GetPlayerComplaints(int page, int per_page, int? from, int? playerID)
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (!(int.TryParse(uidString, out int userID)
                && database.Moderators.Any(match => match.ID == userID && match.ViewPlayerComplaints)))
                return StatusCode(403);

            return Content(Moderation.GetPlayerComplaints(database, page, per_page, from, playerID));
        }

        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/player_complaints/{id}")]
        public IActionResult GetPlayerComplaint(int id)
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (!(int.TryParse(uidString, out int userID)
                && database.Moderators.Any(match => match.ID == userID && match.ViewPlayerComplaints)))
                return StatusCode(403);

            var report = Moderation.GetPlayerComplaint(database, id);

            if (report == null)
                return NotFound();
            else
                return Content(report);
        }
        #endregion

        #region PlayerCreationComplaints
        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/player_creation_complaints")]
        public IActionResult GetPlayerCreationComplaints(int page, int per_page, int? from, int? playerID, int? playerCreationID)
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (!(int.TryParse(uidString, out int userID)
                && database.Moderators.Any(match => match.ID == userID && match.ViewPlayerCreationComplaints)))
                return StatusCode(403);

            return Content(Moderation.GetPlayerCreationComplaints(database, page, per_page, from, playerID, playerCreationID));
        }

        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/player_creation_complaints/{id}")]
        public IActionResult GetPlayerCreationComplaint(int id)
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (!(int.TryParse(uidString, out int userID)
                && database.Moderators.Any(match => match.ID == userID && match.ViewPlayerCreationComplaints)))
                return StatusCode(403);

            var report = Moderation.GetPlayerCreationComplaint(database, id);

            if (report == null)
                return NotFound();
            else
                return Content(report);
        }
        #endregion

        #region ModeratorManagement
        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/moderators")]
        public IActionResult GetModerators(int page, int per_page)
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (!(int.TryParse(uidString, out int userID)
                && database.Moderators.Any(match => match.ID == userID && match.ManageModerators)))
                return StatusCode(403);

            return Content(Moderation.GetModerators(database, page, per_page));
        }

        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/moderators")]
        public IActionResult CreateModerator(string username, string password, ModeratorPermissions permissions)
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (!(int.TryParse(uidString, out int userID)
                && database.Moderators.Any(match => match.ID == userID 
                    && match.ManageModerators 
                    && (!permissions.BanUsers || match.BanUsers)
                    && (!permissions.ChangeCreationStatus || match.ChangeCreationStatus)
                    && (!permissions.ChangeUserSettings || match.ChangeUserSettings)
                    && (!permissions.ViewGriefReports || match.ViewGriefReports)
                    && (!permissions.ViewPlayerComplaints || match.ViewPlayerComplaints)
                    && (!permissions.ViewPlayerCreationComplaints || match.ViewPlayerCreationComplaints))))
                return StatusCode(403);
            
            return Content(Moderation.CreateModerator(database, username, password, permissions));
        }

        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/moderators/{id}")]
        public IActionResult GetModerator(int id)
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (!(int.TryParse(uidString, out int userID)
                && database.Moderators.Any(match => match.ID == userID && match.ManageModerators)))
                return StatusCode(403);

            var report = Moderation.GetModerator(database, id);

            if (report == null)
                return NotFound();
            else
                return Content(report);
        }

        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/{id}/set_username")]
        public IActionResult SetUsernameForOtherUser(int id, string username)
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (!(int.TryParse(uidString, out int userID)
                && database.Moderators.Any(match => match.ID == userID && match.ManageModerators)))
                return StatusCode(403);

            return Content(Moderation.SetUsername(database, id, username));
        }

        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/{id}/set_password")]
        public IActionResult SetPasswordForOtherUser(int id, string password)
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (!(int.TryParse(uidString, out int userID)
                && database.Moderators.Any(match => match.ID == userID && match.ManageModerators)))
                return StatusCode(403);

            return Content(Moderation.SetPassword(database, id, password));
        }

        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/{id}/set_permissions")]
        public IActionResult SetPermissions(int id, ModeratorPermissions permissions)
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (!(int.TryParse(uidString, out int userID) && id != userID
                && database.Moderators.Any(match => match.ID == userID
                    && match.ManageModerators
                    && (!permissions.BanUsers || match.BanUsers)
                    && (!permissions.ChangeCreationStatus || match.ChangeCreationStatus)
                    && (!permissions.ChangeUserSettings || match.ChangeUserSettings)
                    && (!permissions.ViewGriefReports || match.ViewGriefReports)
                    && (!permissions.ViewPlayerComplaints || match.ViewPlayerComplaints)
                    && (!permissions.ViewPlayerCreationComplaints || match.ViewPlayerCreationComplaints))))
                return StatusCode(403);

            return Content(Moderation.SetPermissions(database, id, permissions));
        }

        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/moderation/moderators/{id}")]
        public IActionResult DeleteModerator(int id)
        {
            var uidString = User.FindFirstValue(JWTUtils.UserID);
            if (!(int.TryParse(uidString, out int userID)
                && database.Moderators.Any(match => match.ID == userID && match.ManageModerators)))
                return StatusCode(403);

            if (userID == id) //why would you want to do this?
                return Content("error_cannot_remove_yourself");

            var result = Moderation.DeleteModerator(database, id);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
using GameServer.Implementation.Common;
using GameServer.Models;
using GameServer.Models.Moderation;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;


namespace GameServer.Controllers.Api
{
    public class ModerationApiController(Database database, IUGCStorage storage) : Controller
    {
        #region ModeratorSelf
        [HttpPost]
        [Route("/api/moderation/login")]
        public IActionResult Login(string login, string password)
        {
            var result = Moderation.Login(database, login, password);

            if (result == null)
                return Content("error");
            else
                Response.Cookies.Append("Token", result);

            return Content("ok");
        }
        
        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/refresh_token")]
        public IActionResult RefreshToken()
        {
            var session = Moderation.GetSession(database, User);
            if (session?.User == null)
                return NotFound();
            
            var result = Moderation.RefreshToken(database, session);

            if (result == null)
                return Content("error");
            else
                Response.Cookies.Append("Token", result);

            return Content("ok");
        }

        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/permissions")]
        public IActionResult GetPermissions()
        {
            var user = Moderation.GetUser(database, User);
            if (user != null)
                return Content(Moderation.GetPermissions(database, user));
            else
                return NotFound();
        }
        
        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/session")]
        public IActionResult GetSession()
        {
            var session = Moderation.GetSession(database, User);
            if (session?.User == null)
                return NotFound();
            
            return Content(JsonConvert.SerializeObject(session));
        }

        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/sessions")]
        public IActionResult GetSessions(int page, int per_page)
        {
            var user = Moderation.GetUser(database, User);
            if (user != null)
                return Content(Moderation.GetSessions(database, user.ID, page, per_page));
            else
                return NotFound();
        }
        
        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/sessions/{id}")]
        public IActionResult RemoveSession(Guid id)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null)
                return NotFound();

            var result = Moderation.RemoveSession(database, user.ID, id);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }
        
        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/sessions")]
        public IActionResult RemoveAllSessions()
        {
            var user = Moderation.GetUser(database, User);
            if (user == null)
                return NotFound();

            var result = Moderation.RemoveAllSessions(database, user.ID);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }
        
        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/set_username")]
        public IActionResult SetUsername(string username)
        {
            var user = Moderation.GetUser(database, User);
            if (user != null)
                return Content(Moderation.SetUsername(database, user, username));
            else
                return NotFound();
        }

        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/set_password")]
        public IActionResult SetPassword(string password)
        {
            var user = Moderation.GetUser(database, User);
            if (user != null)
                return Content(Moderation.SetPassword(database, user, password));
            else
                return NotFound();
        }
        #endregion

        #region GriefReports
        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/grief_reports")]
        public IActionResult GetGriefReports(int page, int per_page, string context, int? from)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ViewGriefReports)
                return StatusCode(403);

            return Content(Moderation.GetGriefReports(database, page, per_page, context, from));
        }

        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/grief_reports/{id}")]
        public IActionResult GetGriefReport(int id)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ViewGriefReports)
                return StatusCode(403);

            var report = Moderation.GetGriefReport(database, id);

            if (report == null)
                return NotFound();
            else
                return Content(report);
        }

        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/grief_reports/{id}/data.xml")]
        public IActionResult GetGriefReportDataFile(int id)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ViewGriefReports)
                return StatusCode(403);

            var file = storage.LoadGriefReportData(id, "data.xml");

            if (file != null)
                return File(file, "application/xml;charset=utf-8");
            else
                return NotFound();
        }

        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/grief_reports/{id}/preview.png")]
        public IActionResult GetGriefReportPreview(int id)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ViewGriefReports)
                return StatusCode(403);

            var file = storage.LoadGriefReportData(id, "preview.png");

            if (file != null)
                return File(file, "image/png");
            else
                return NotFound();
        }
        #endregion

        #region CreationManagement
        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/setStatus")]
        public IActionResult SetModerationStatus(int id, ModerationStatus status)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ChangeCreationStatus)
                return StatusCode(403);

            var result = Moderation.SetModerationStatus(database, id, status);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }

        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/player_creations")]
        public IActionResult GetPlayerCreationsWithStatus(int page, int per_page, ModerationStatus status)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !(user.ChangeCreationStatus || user.ResetCreationStats || user.RemovePlayerCreations))
                return StatusCode(403);

            return Content(Moderation.GetPlayerCreationsWithStatus(database, page, per_page, status));
        }

        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/player_creations/{playerCreationID}/stats")]
        public IActionResult ResetCreationStats(int playerCreationID)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ResetCreationStats)
                return StatusCode(403);

            var result = Moderation.ResetCreationStats(database, playerCreationID);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }
        
        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/player_creations/{playerCreationID}")]
        public IActionResult RemovePlayerCreation(int playerCreationID)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.RemovePlayerCreations)
                return StatusCode(403);

            var result = Moderation.RemovePlayerCreation(database, storage, playerCreationID);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }

        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/player_creations/{playerCreationID}/comment/{id}")]
        public IActionResult RemovePlayerCreationComment(int playerCreationID, int id)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.RemovePlayerCreationComments)
                return StatusCode(403);

            var result = Moderation.RemovePlayerCreationComment(database, playerCreationID, id);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }

        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/player_creations/{playerCreationID}/comments")]
        public IActionResult RemoveAllPlayerCreationComments(int playerCreationID)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.RemovePlayerCreationComments)
                return StatusCode(403);

            var result = Moderation.RemoveAllPlayerCreationComments(database, playerCreationID);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }
        #endregion

        #region UserManagement
        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/setBan")]
        public IActionResult SetBan(int id, bool isBanned)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.BanUsers)
                return StatusCode(403);

            var result = Moderation.SetBan(database, id, isBanned);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }

        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/setUserSettings")]
        public IActionResult SetUserSettings(int id, bool ShowCreationsWithoutPreviews, bool AllowOppositePlatform)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ChangeUserSettings)
                return StatusCode(403);

            var result = Moderation.SetUserSettings(database, id, ShowCreationsWithoutPreviews, AllowOppositePlatform);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }

        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/setUserQuota")]
        public IActionResult SetUserQuota(int id, int quota)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ChangeUserQuota)
                return StatusCode(403);

            var result = Moderation.SetUserQuota(database, id, quota);

            if (result == null)
                return NotFound();
            else if (result == "error_invalid_quota")
                return BadRequest();
            else
                return Content(result);
        }

        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/users")]
        public IActionResult GetUsers(
            int page, int per_page,
            bool? PlayedMNR, bool? IsPSNLinked, bool? IsRPCNLinked,
            bool? IsBanned
        )
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !(user.BanUsers || user.ChangeUserSettings || user.ChangeUserQuota 
                                  || user.ResetUserStats || user.RemovePlayerCreations || user.RemoveUsers || user.ManageUserSessions))
                return StatusCode(403);

            return Content(Moderation.GetUsers(database, page, per_page, PlayedMNR, IsPSNLinked, IsRPCNLinked, IsBanned));
        }
        
        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/users/{id}")]
        public IActionResult RemoveUser(int id)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.RemoveUsers)
                return StatusCode(403);

            var result = Moderation.RemoveUser(database, id);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }

        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/users/{id}/stats")]
        public IActionResult ResetUserProfile(int id, bool removeCreations = false)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ResetUserStats 
                || (removeCreations && !user.RemovePlayerCreations))
                return StatusCode(403);

            var result = Moderation.ResetUserProfile(database, storage, id, removeCreations);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }
        
        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/users/{id}/creations")]
        public IActionResult RemovePlayerCreations(int id)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.RemovePlayerCreations)
                return StatusCode(403);

            var result = Moderation.RemovePlayerCreations(database, storage, id);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }
        
        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/users/{id}/sessions")]
        public IActionResult GetUserSessions(int id, int page, int per_page)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageUserSessions)
                return StatusCode(403);

            var report = Moderation.GetUserSessions(database, id, page, per_page);

            if (report == null)
                return NotFound();
            else
                return Content(report);
        }
        
        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/users/{id}/{sessionID}")]
        public IActionResult RemoveUserSession(int id, Guid sessionID)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageUserSessions)
                return StatusCode(403);

            var report = Moderation.RemoveUserSession(database, id, sessionID);

            if (report == null)
                return NotFound();
            else
                return Content(report);
        }
        
        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/users/{id}/sessions")]
        public IActionResult RemoveUserSessions(int id)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageUserSessions)
                return StatusCode(403);

            var report = Moderation.RemoveAllUserSessions(database, id);

            if (report == null)
                return NotFound();
            else
                return Content(report);
        }

        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/users/{id}/profile_comments")]
        public IActionResult RemoveProfileComments(int id)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.RemoveProfileComments)
                return StatusCode(403);

            var result = Moderation.RemoveProfileComments(database, id);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }

        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/users/profile_comments/{commentId}")]
        public IActionResult RemoveProfileComment(int commentId)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.RemoveProfileComments)
                return StatusCode(403);

            var result = Moderation.RemoveProfileComment(database, commentId);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }
        #endregion

        #region PlayerComplaints
        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/player_complaints")]
        public IActionResult GetPlayerComplaints(int page, int per_page, int? from, int? playerID)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ViewPlayerComplaints)
                return StatusCode(403);

            return Content(Moderation.GetPlayerComplaints(database, page, per_page, from, playerID));
        }

        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/player_complaints/{id}")]
        public IActionResult GetPlayerComplaint(int id)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ViewPlayerComplaints)
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
        [Route("/api/moderation/player_creation_complaints")]
        public IActionResult GetPlayerCreationComplaints(int page, int per_page, int? from, int? playerID, int? playerCreationID)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ViewPlayerCreationComplaints)
                return StatusCode(403);

            return Content(Moderation.GetPlayerCreationComplaints(database, page, per_page, from, playerID, playerCreationID));
        }

        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/player_creation_complaints/{id}")]
        public IActionResult GetPlayerCreationComplaint(int id)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ViewPlayerCreationComplaints)
                return StatusCode(403);

            var report = Moderation.GetPlayerCreationComplaint(database, id);

            if (report == null)
                return NotFound();
            else
                return Content(report);
        }
        
        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/player_creation_complaints/{id}/preview.png")]
        public IActionResult GetPlayerCreationComplaintPreview(int id)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ViewGriefReports)
                return StatusCode(403);

            var file = storage.LoadPlayerCreationComplaintPreview(id);

            if (file != null)
                return File(file, "image/png");
            else
                return NotFound();
        }
        #endregion

        #region SystemEvents
        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/system_events")]
        public IActionResult GetSystemEvents(int page, int per_page)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageSystemEvents)
                return StatusCode(403);

            return Content(Moderation.GetSystemEvents(database, page, per_page));
        }
        
        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/system_events")]
        public IActionResult CreateSystemEvent(string topic, string description, string imageURL)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageSystemEvents)
                return StatusCode(403);

            return Content(Moderation.CreateSystemEvent(database, topic, description, imageURL));
        }
        
        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/system_events/{id}")]
        public IActionResult EditSystemEvent(int id, string topic, string description, string imageURL)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageSystemEvents)
                return StatusCode(403);

            var result = Moderation.EditSystemEvent(database, id, topic, description, imageURL);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }
        
        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/system_events/{id}")]
        public IActionResult DeleteSystemEvent(int id)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageSystemEvents)
                return StatusCode(403);

            var result = Moderation.DeleteSystemEvent(database, id);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }
        #endregion

        #region Announcements
        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/announcements")]
        public IActionResult GetAnnouncements(int page, int per_page, Platform? platform)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageAnnouncements)
                return StatusCode(403);

            return Content(Moderation.GetAnnouncements(database, page, per_page, platform));
        }

        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/announcements")]
        public IActionResult CreateAnnouncement(string languageCode, string subject, string text, Platform platform)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageAnnouncements)
                return StatusCode(403);

            return Content(Moderation.CreateAnnouncement(database, languageCode, subject, text, platform));
        }

        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/announcements/{id}")]
        public IActionResult EditAnnouncement(int id, string languageCode, string subject, string text, Platform platform)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageAnnouncements)
                return StatusCode(403);

            var result = Moderation.EditAnnouncement(database, id, languageCode, subject, text, platform);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }

        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/announcements/{id}")]
        public IActionResult DeleteAnnouncement(int id)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageAnnouncements)
                return StatusCode(403);

            var result = Moderation.DeleteAnnouncement(database, id);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }
        #endregion

        #region Hotlap
        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/hotlap")]
        public IActionResult GetHotLap()
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageHotlap)
                return StatusCode(403);

            return Content(Moderation.GetHotLap(database));
        }
        
        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/hotlap")]
        public IActionResult SetHotLap(int creation)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageHotlap)
                return StatusCode(403);

            return Content(Moderation.SetHotLap(database, creation));
        }
        
        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/hotlap/reset")]
        public IActionResult ResetHotLap()
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageHotlap)
                return StatusCode(403);

            ContentUpdates.GetNewHotLap(database);
            
            return Content("ok");
        }
        
        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/hotlap/until_next")]
        public IActionResult GetNextHotLapReset()
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageHotlap)
                return StatusCode(403);

            return Content(((int)TimeUtils.UntilNextDay.TotalSeconds).ToString());
        }
        
        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/hotlap/queue")]
        public IActionResult GetHotLapQueue(int page, int per_page)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageHotlap)
                return StatusCode(403);

            return Content(Moderation.GetHotLapQueue(database, page, per_page));
        }
        
        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/hotlap/queue")]
        public IActionResult AddToHotLapQueue(int creation)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageHotlap)
                return StatusCode(403);

            return Content(Moderation.AddToHotLapQueue(database, creation));
        }
        
        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/hotlap/queue")]
        public IActionResult RemoveFromHotLapQueue(int? index, int? creation)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageHotlap)
                return StatusCode(403);

            return Content(Moderation.RemoveFromHotLapQueue(database, index, creation));
        }

        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/hotlap/score/{scoreId}")]
        public IActionResult RemoveHotLapScoreById(int scoreId)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageHotlap)
                return StatusCode(403);

            var result = Moderation.RemoveHotLapScoreById(database, scoreId);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }
        #endregion

        #region ScoreManagement
        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/score/{scoreId}")]
        public IActionResult RemoveScoreById(int scoreId)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.RemoveScores)
                return StatusCode(403);

            var result = Moderation.RemoveScoreById(database, storage, scoreId);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }

        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/scores/{trackId}")]
        public IActionResult RemoveAllScoresForTrack(int trackId)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.RemoveScores)
                return StatusCode(403);

            var result = Moderation.RemoveAllScoresForTrack(database, storage, trackId);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }

        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/scores/player/{playerId}")]
        public IActionResult RemoveAllScoresForPlayer(int playerId)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.RemoveScores)
                return StatusCode(403);

            var result = Moderation.RemoveAllScoresForPlayer(database, storage, playerId);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }
        #endregion

        #region Whitelist
        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/whitelist")]
        public IActionResult GetWhitelist(int page, int per_page)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageWhitelist)
                return StatusCode(403);

            return Content(Moderation.GetWhitelist(page, per_page));
        }
        
        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/whitelist")]
        public IActionResult AddToWhitelist(string username)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageWhitelist)
                return StatusCode(403);

            var result = Moderation.AddToWhitelist(username);
            
            if (result == null)
                return NotFound();
            else
                return Content(result);
        }
        
        [HttpPatch]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/whitelist")]
        public IActionResult UpdateWhitelist(string oldUsername, string newUsername)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageWhitelist)
                return StatusCode(403);

            var result = Moderation.UpdateWhitelist(oldUsername, newUsername);
            
            if (result == null)
                return NotFound();
            else
                return Content(result);
        }
        
        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/whitelist")]
        public IActionResult RemoveFromWhitelist(string username)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageWhitelist)
                return StatusCode(403);

            var result = Moderation.RemoveFromWhitelist(username);
            
            if (result == null)
                return NotFound();
            else
                return Content(result);
        }
        #endregion

        #region TeamPicksManagement
        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/team_picks/{creationID}")]
        public IActionResult AddToTeamPicks(int creationID)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageTeamPicks)
                return StatusCode(403);

            var result = Moderation.AddToTeamPicks(database, creationID);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }

        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/team_picks/{creationID}")]
        public IActionResult RemoveFromTeamPicks(int creationID)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageTeamPicks)
                return StatusCode(403);

            var result = Moderation.RemoveFromTeamPicks(database, creationID);

            if (result == null)
                return NotFound();
            else
                return Content(result);
        }

        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/team_picks")]
        public IActionResult ClearTeamPicks()
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageTeamPicks)
                return StatusCode(403);

            return Content(Moderation.ClearTeamPicks(database));
        }
        #endregion
        
        #region ModeratorManagement
        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/moderators")]
        public IActionResult GetModerators(int page, int per_page)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageModerators)
                return StatusCode(403);

            return Content(Moderation.GetModerators(database, page, per_page));
        }

        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/moderators")]
        public IActionResult CreateModerator(string username, string password, ModeratorPermissions permissions)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageModerators || !user.HasPermissions(permissions))
                return StatusCode(403);

            return Content(Moderation.CreateModerator(database, username, password, permissions));
        }

        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/moderators/{id}")]
        public IActionResult GetModerator(int id)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageModerators)
                return StatusCode(403);

            var report = Moderation.GetModerator(database, id);

            if (report == null)
                return NotFound();
            else
                return Content(report);
        }
        
        [HttpGet]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/moderators/{id}/sessions")]
        public IActionResult GetModeratorSessions(int id, int page, int per_page)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageModerators)
                return StatusCode(403);

            var report = Moderation.GetSessions(database, id, page, per_page);

            if (report == null)
                return NotFound();
            else
                return Content(report);
        }
        
        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/moderators/{id}/{sessionID}")]
        public IActionResult RemoveModeratorSession(int id, Guid sessionID)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageModerators)
                return StatusCode(403);

            var report = Moderation.RemoveSession(database, id, sessionID);

            if (report == null)
                return NotFound();
            else
                return Content(report);
        }
        
        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/moderators/{id}/sessions")]
        public IActionResult RemoveModeratorSessions(int id)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageModerators)
                return StatusCode(403);

            var report = Moderation.RemoveAllSessions(database, id);

            if (report == null)
                return NotFound();
            else
                return Content(report);
        }

        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/{id}/set_username")]
        public IActionResult SetUsernameForOtherUser(int id, string username)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageModerators)
                return StatusCode(403);

            return Content(Moderation.SetUsername(database, id, username));
        }

        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/{id}/set_password")]
        public IActionResult SetPasswordForOtherUser(int id, string password)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageModerators)
                return StatusCode(403);

            return Content(Moderation.SetPassword(database, id, password));
        }

        [HttpPost]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/{id}/set_permissions")]
        public IActionResult SetPermissions(int id, ModeratorPermissions permissions)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageModerators || !user.HasPermissions(permissions))
                return StatusCode(403);

            return Content(Moderation.SetPermissions(database, id, permissions));
        }

        [HttpDelete]
        [Authorize(Policy = JWTUtils.ModeratorPolicy)]
        [Route("/api/moderation/moderators/{id}")]
        public IActionResult DeleteModerator(int id)
        {
            var user = Moderation.GetUser(database, User);
            if (user == null || !user.ManageModerators)
                return StatusCode(403);

            if (user.ID == id) //why would you want to do this?
                return Content("error_cannot_remove_yourself");

            var result = Moderation.DeleteModerator(database, id, user);

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

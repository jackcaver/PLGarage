using GameServer.Models;
using GameServer.Models.Moderation;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace GameServer.Implementation.Common
{
    public class Moderation
    {
        #region Game
        public static string GriefReport(Database database, Guid SessionID, GriefReport grief_report)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
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
                grief_report.data.OpenReadStream(),
                grief_report.preview.OpenReadStream());
            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static string PlayerComplaints(Database database, Guid SessionID, PlayerComplaint player_complaint)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var player = database.Users.FirstOrDefault(match => match.UserId == player_complaint.player_id);

            if (user == null || player == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.PlayerComplaints.Add(new PlayerComplaintData
            {
                UserId = user.UserId,
                PlayerId = player_complaint.player_id,
                Reason = player_complaint.player_complaint_reason,
                Comments = player_complaint.player_comments
            });
            database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static string PlayerCreationComplaints(Database database, Guid SessionID, PlayerCreationComplaint player_creation_complaint)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == player_creation_complaint.player_creation_id);

            if (user == null || creation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.PlayerCreationComplaints.Add(new PlayerCreationComplaintData
            {
                UserId = user.UserId,
                PlayerId = player_creation_complaint.owner_player_id,
                PlayerCreationId = player_creation_complaint.player_creation_id,
                Reason = player_creation_complaint.player_complaint_reason,
                Comments = player_creation_complaint.player_comments
            });
            database.SaveChanges();

            if (player_creation_complaint.preview != null)
                UserGeneratedContentUtils.SavePlayerCreationComplaintPreview(database.PlayerCreationComplaints.Count(),
                    player_creation_complaint.preview.OpenReadStream());
            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }
        #endregion

        #region ModeratorSelf
        public static string Login(Database database, string username, string password)
        {
            var moderator = database.Moderators.FirstOrDefault(match => match.Username == username);

            if (moderator == null || !BCrypt.Net.BCrypt.Verify(password, moderator.Password))
                return null;

            return JWTUtils.GenerateToken(moderator.ID, true);
        }

        public static string SetUsername(Database database, int userID, string username)
        {
            var moderator = database.Moderators.FirstOrDefault(match => match.ID == userID);

            if (moderator == null || string.IsNullOrEmpty(username))
                return moderator == null ? "error_moderator_not_found" : "error_username_is_empty";

            moderator.Username = username;
            database.SaveChanges();

            return "ok";
        }

        public static string SetPassword(Database database, int userID, string password)
        {
            var moderator = database.Moderators.FirstOrDefault(match => match.ID == userID);

            if (moderator == null || string.IsNullOrEmpty(password))
                return moderator == null ? "error_moderator_not_found" : "error_password_is_empty";

            moderator.Password = BCrypt.Net.BCrypt.HashPassword(password);
            database.SaveChanges();

            return "ok";
        }

        public static string GetPermissions(Database database, int userID)
        {
            var moderator = database.Moderators.FirstOrDefault(match => match.ID == userID);

            if (moderator == null)
                return null;

            var permissions = new ModeratorPermissions
            {
                BanUsers = moderator.BanUsers,
                ChangeCreationStatus = moderator.ChangeCreationStatus,
                ChangeUserSettings = moderator.ChangeUserSettings,
                ManageModerators = moderator.ManageModerators,
                ViewGriefReports = moderator.ViewGriefReports,
                ViewPlayerComplaints = moderator.ViewPlayerComplaints,
                ViewPlayerCreationComplaints = moderator.ViewPlayerCreationComplaints
            };

            return JsonConvert.SerializeObject(permissions);
        }
        #endregion

        #region GriefReports
        public static string GetGriefReports(Database database, int page, int per_page, string context, int? from)
        {
            if (page <= 0)
                page = 1;
            if (per_page <= 0)
                per_page = 1;

            IQueryable<GriefReportData> query = database.GriefReports.Where(match => from == null || match.UserId == from);

            if (!string.IsNullOrEmpty(context))
                query = query.Where(match => match.Context == context);

            var pageStart = PageCalculator.GetPageStart(page, per_page);

            var reports = query.Skip(pageStart).Take(per_page).ToList();

            return JsonConvert.SerializeObject(reports);
        }

        public static string GetGriefReport(Database database, int id)
        {
            var report = database.GriefReports.FirstOrDefault(match => match.Id == id);

            if (report == null)
                return null;

            return JsonConvert.SerializeObject(report);
        }
        #endregion

        #region CreationManagement
        public static string SetModerationStatus(Database database, int id, ModerationStatus status)
        {
            var creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == id);

            if (creation == null)
                return null;

            creation.ModerationStatus = status;
            database.SaveChanges();

            return "ok";
        }

        public static string GetPlayerCreationsWithStatus(Database database, int page, int per_page, ModerationStatus status)
        {
            if (page <= 0)
                page = 1;
            if (per_page <= 0)
                per_page = 1;

            var query = database.PlayerCreations.Where(match => match.ModerationStatus == status);

            var pageStart = PageCalculator.GetPageStart(page, per_page);

            var creations = query.Select(creation => new MinimalCreationInfo
            {
                ID = creation.PlayerCreationId,
                Name = creation.Name,
                Description = creation.Description,
                Type = creation.Type,
                OriginalPlayerID = creation.OriginalPlayerId,
                ParentPlayerID = creation.ParentPlayerId,
                PlayerID = creation.PlayerId,
                ParentCreationID = creation.ParentCreationId,
                ModerationStatus = creation.ModerationStatus,
                IsMNR = creation.IsMNR
            }).Skip(pageStart).Take(per_page).ToList();

            return JsonConvert.SerializeObject(creations);
        }
        #endregion

        #region UserManagement
        public static string SetBan(Database database, int id, bool isBanned)
        {
            var user = database.Users.FirstOrDefault(match => match.UserId == id);

            if (user == null)
                return null;

            user.IsBanned = isBanned;
            database.SaveChanges();

            return "ok";
        }

        public static string SetUserSettings(Database database, int id, bool ShowCreationsWithoutPreviews, bool AllowOppositePlatform)
        {
            var user = database.Users.FirstOrDefault(match => match.UserId == id);

            if (user == null)
                return null;

            user.ShowCreationsWithoutPreviews = ShowCreationsWithoutPreviews;
            user.AllowOppositePlatform = AllowOppositePlatform;
            database.SaveChanges();

            return "ok";
        }

        public static string GetUsers(Database database, int page, int per_page, bool? PlayedMNR, bool? IsPSNLinked, bool? IsRPCNLinked)
        {
            if (page <= 0)
                page = 1;
            if (per_page <= 0)
                per_page = 1;

            var query = database.Users.AsQueryable();

            if (PlayedMNR != null)
                query = query.Where(match => match.PlayedMNR == PlayedMNR);

            if (IsPSNLinked != null)
                query = query.Where(match => match.PSNID != 0);

            if (IsRPCNLinked != null)
                query = query.Where(match => match.RPCNID != 0);

            var pageStart = PageCalculator.GetPageStart(page, per_page);

            var creations = query.Select(user => new MinimalUserInfo
            {
                ID = user.UserId,
                Username = user.Username,
                PlayedMNR = user.PlayedMNR,
                IsPSNLinked = user.PSNID != 0,
                IsRPCNLinked = user.RPCNID != 0,
                AllowOppositePlatform = user.AllowOppositePlatform,
                ShowCreationsWithoutPreviews = user.ShowCreationsWithoutPreviews,
            }).Skip(pageStart).Take(per_page).ToList();

            return JsonConvert.SerializeObject(creations);
        }
        #endregion

        #region PlayerComplaints
        public static string GetPlayerComplaints(Database database, int page, int per_page, int? from, int? playerID)
        {
            if (page <= 0)
                page = 1;
            if (per_page <= 0)
                per_page = 1;

            IQueryable<PlayerComplaintData> query = database.PlayerComplaints.Where(match => from == null || match.UserId == from);

            if (playerID != null)
                query = query.Where(match => match.PlayerId == playerID);

            var pageStart = PageCalculator.GetPageStart(page, per_page);

            var reports = query.Skip(pageStart).Take(per_page).ToList();

            return JsonConvert.SerializeObject(reports);
        }

        public static string GetPlayerComplaint(Database database, int id)
        {
            var complaint = database.PlayerComplaints.FirstOrDefault(match => match.Id == id);

            if (complaint == null)
                return null;

            return JsonConvert.SerializeObject(complaint);
        }
        #endregion

        #region PlayerCreationComplaints
        public static string GetPlayerCreationComplaints(Database database, int page, int per_page, int? from, int? playerID, int? playerCreationID)
        {
            if (page <= 0)
                page = 1;
            if (per_page <= 0)
                per_page = 1;

            IQueryable<PlayerCreationComplaintData> query = database.PlayerCreationComplaints.Where(match => from == null || match.UserId == from);

            if (playerID != null)
                query = query.Where(match => match.PlayerId == playerID);
            if (playerCreationID != null)
                query = query.Where(match => match.PlayerCreationId == playerCreationID);

            var pageStart = PageCalculator.GetPageStart(page, per_page);

            var reports = query.Skip(pageStart).Take(per_page).ToList();

            return JsonConvert.SerializeObject(reports);
        }

        public static string GetPlayerCreationComplaint(Database database, int id)
        {
            var complaint = database.PlayerCreationComplaints.FirstOrDefault(match => match.Id == id);

            if (complaint == null)
                return null;

            return JsonConvert.SerializeObject(complaint);
        }
        #endregion

        #region ModeratorManagement
        public static string CreateModerator(Database database, string username, string password, ModeratorPermissions permissions)
        {
            var usernameIsTaken = database.Moderators.Any(match => match.Username == username);
            if (usernameIsTaken || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return usernameIsTaken ? "error_username_is_taken" : string.IsNullOrEmpty(username) ? "error_username_is_empty" : "error_password_is_empty";

            database.Moderators.Add(new Moderator
            {
                Username = username,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                BanUsers = permissions.BanUsers,
                ChangeCreationStatus = permissions.ChangeCreationStatus,
                ChangeUserSettings = permissions.ChangeUserSettings,
                ManageModerators = permissions.ManageModerators,
                ViewGriefReports = permissions.ViewGriefReports,
                ViewPlayerComplaints = permissions.ViewPlayerComplaints,
                ViewPlayerCreationComplaints = permissions.ViewPlayerCreationComplaints
            });

            database.SaveChanges();

            return "ok";
        }

        public static void CreateDefaultModerator(Database database)
        {
            if (database.Moderators.Any())
                return;

            CreateModerator(database, "admin", "admin", new ModeratorPermissions {
                BanUsers = true,
                ChangeCreationStatus = true,
                ChangeUserSettings = true,
                ManageModerators = true,
                ViewGriefReports = true,
                ViewPlayerComplaints = true,
                ViewPlayerCreationComplaints = true
            });
        }

        public static string DeleteModerator(Database database, int userID)
        {
            var moderator = database.Moderators.FirstOrDefault(match => match.ID == userID);

            if (moderator == null) 
                return null;

            database.Moderators.Remove(moderator);
            database.SaveChanges();

            return "ok";
        }

        public static string GetModerators(Database database, int page, int per_page)
        {
            if (page <= 0)
                page = 1;
            if (per_page <= 0)
                per_page = 1;

            var pageStart = PageCalculator.GetPageStart(page, per_page);

            var moderators = database.Moderators.Skip(pageStart).Take(per_page).ToList();

            return JsonConvert.SerializeObject(moderators);
        }

        public static string GetModerator(Database database, int userID)
        {
            var moderator = database.Moderators.FirstOrDefault(match => match.ID == userID);

            if (moderator == null)
                return null;

            return JsonConvert.SerializeObject(moderator);
        }

        public static string SetPermissions(Database database, int userID, ModeratorPermissions permissions)
        {
            var moderator = database.Moderators.FirstOrDefault(match => match.ID == userID);

            if (moderator == null)
                return "error_moderator_not_found";

            moderator.BanUsers = permissions.BanUsers;
            moderator.ChangeCreationStatus = permissions.ChangeCreationStatus;
            moderator.ChangeUserSettings = permissions.ChangeUserSettings;
            moderator.ManageModerators = permissions.ManageModerators;
            moderator.ViewGriefReports = permissions.ViewGriefReports;
            moderator.ViewPlayerComplaints = permissions.ViewPlayerComplaints;
            moderator.ViewPlayerCreationComplaints = permissions.ViewPlayerCreationComplaints;
            database.SaveChanges();

            return "ok";
        }
        #endregion
    }
}

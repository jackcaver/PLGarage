using GameServer.Models;
using GameServer.Models.Config;
using GameServer.Models.Moderation;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using Newtonsoft.Json;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
                ChangeUserQuota = moderator.ChangeUserQuota,
                ManageModerators = moderator.ManageModerators,
                ViewGriefReports = moderator.ViewGriefReports,
                ViewPlayerComplaints = moderator.ViewPlayerComplaints,
                ViewPlayerCreationComplaints = moderator.ViewPlayerCreationComplaints,
                ManageAnnouncements = moderator.ManageAnnouncements,
                ManageHotlap = moderator.ManageHotlap,
                ManageSystemEvents = moderator.ManageSystemEvents
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

            return JsonConvert.SerializeObject(new ModerationPageResponse<GriefReportData>
            {
                Total = query.Count(),
                Page = reports
            });
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

            return JsonConvert.SerializeObject(new ModerationPageResponse<MinimalCreationInfo>
            {
                Total = query.Count(),
                Page = creations
            });
        }

        public static string ResetCreationStats(Database database, int playerCreationID)
        {
            if (!database.PlayerCreations.Any(c => c.PlayerCreationId == playerCreationID))
                return null;

            database.PlayerCreationDownloads
                .Where(x => x.PlayerCreationId == playerCreationID)
                .ExecuteDelete();

            database.PlayerCreationViews
                .Where(x => x.PlayerCreationId == playerCreationID)
                .ExecuteDelete();

            database.PlayerCreationRatings
                .Where(x => x.PlayerCreationId == playerCreationID)
                .ExecuteDelete();

            database.PlayerCreationPoints
                .Where(x => x.PlayerCreationId == playerCreationID)
                .ExecuteDelete();

            database.PlayerCreationComments
                .Where(x => x.PlayerCreationId == playerCreationID)
                .ExecuteDelete();

            database.PlayerCreationReviews
                .Where(x => x.PlayerCreationId == playerCreationID)
                .ExecuteDelete();

            return "ok";
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
        public static string SetUserQuota(Database database, int id, int quota)
        {
            var user = database.Users.FirstOrDefault(match => match.UserId == id);

            if (user == null)
                return null;

            if (quota < 0)
                return "error_invalid_quota";

            user.Quota = quota;
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

            return JsonConvert.SerializeObject(new ModerationPageResponse<PlayerComplaintData>
            {
                Total = query.Count(),
                Page = reports
            });
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

            return JsonConvert.SerializeObject(new ModerationPageResponse<PlayerCreationComplaintData>
            {
                Total = query.Count(),
                Page = reports
            });
        }

        public static string GetPlayerCreationComplaint(Database database, int id)
        {
            var complaint = database.PlayerCreationComplaints.FirstOrDefault(match => match.Id == id);

            if (complaint == null)
                return null;

            return JsonConvert.SerializeObject(complaint);
        }
        #endregion

        #region SystemEvents
        public static string GetSystemEvents(Database database, int page, int per_page)
        {
            if (page <= 0)
                page = 1;
            if (per_page <= 0)
                per_page = 1;

            var query = database.ActivityLog.Where(match => match.Type == ActivityType.system_event);

            var pageStart = PageCalculator.GetPageStart(page, per_page);

            var events = query.Select(systemEvent => new MinimalSystemEventInfo
            {
               CreatedAt = systemEvent.CreatedAt,
               Topic = systemEvent.Topic,
               Description = systemEvent.Description,
               ImageURL = systemEvent.ImageURL
            }).Skip(pageStart).Take(per_page).ToList();

            return JsonConvert.SerializeObject(new ModerationPageResponse<MinimalSystemEventInfo>
            {
                Total = query.Count(),
                Page = events
            });
        }
        
        public static string CreateSystemEvent(Database database, string topic, string description, string imageURL)
        {
            database.ActivityLog.Add(new()
            {
                Type = ActivityType.system_event,
                CreatedAt = TimeUtils.Now,
                Topic = topic,
                Description = description,
                ImageURL = imageURL,
                List = ActivityList.news_feed
            });
            
            database.SaveChanges();

            return "ok";
        }

        public static string EditSystemEvent(Database database, int id, string topic, string description, string imageURL)
        {
            var systemEvent = database.ActivityLog.FirstOrDefault(match => match.Id == id && match.Type == ActivityType.system_event);

            if (systemEvent == null)
                return null;
            
            systemEvent.Topic = topic;
            systemEvent.Description = description;
            systemEvent.ImageURL = imageURL;
            database.SaveChanges();

            return "ok";
        }
        
        public static string DeleteSystemEvent(Database database, int id)
        {
            var activityEvent = database.ActivityLog.FirstOrDefault(match => match.Id == id && match.Type == ActivityType.system_event);

            if (activityEvent == null)
                return null;

            database.ActivityLog.Remove(activityEvent);
            database.SaveChanges();

            return "ok";
        }
        #endregion

        #region Announcements
        public static string GetAnnouncements(Database database, int page, int per_page, Platform? platform)
        {
            if (page <= 0)
                page = 1;
            if (per_page <= 0)
                per_page = 1;

            var query = database.Announcements.AsQueryable();

            if (platform != null)
                query = query.Where(match => match.Platform == platform);

            var pageStart = PageCalculator.GetPageStart(page, per_page);

            var announcements = query.Skip(pageStart).Take(per_page).ToList();

            return JsonConvert.SerializeObject(new ModerationPageResponse<AnnouncementData>
            {
                Total = query.Count(),
                Page = announcements
            });
        }

        public static string CreateAnnouncement(Database database, string languageCode, string subject, string text, Platform platform)
        {
            database.Announcements.Add(new()
            {
                CreatedAt = TimeUtils.Now,
                LanguageCode = languageCode,
                Subject = subject,
                Text = text,
                Platform = platform
            });

            database.SaveChanges();

            return "ok";
        }

        public static string EditAnnouncement(Database database, int id, string languageCode, string subject, string text, Platform platform)
        {
            var announcement = database.Announcements.FirstOrDefault(match => match.Id == id);

            if (announcement == null)
                return null;

            announcement.LanguageCode = languageCode;
            announcement.Subject = subject;
            announcement.Text = text;
            announcement.Platform = platform;
            database.SaveChanges();

            return "ok";
        }

        public static string DeleteAnnouncement(Database database, int id)
        {
            var announcement = database.Announcements.FirstOrDefault(match => match.Id == id);

            if (announcement == null)
                return null;

            database.Announcements.Remove(announcement);
            database.SaveChanges();

            return "ok";
        }
        #endregion

        #region HotLap
        public static string GetHotLap(Database database)
        {
            HotLapData hotLap = ContentUpdates.ReadHotlapData();

            return hotLap == null ? "error_no_hotlap_file" : hotLap.TrackId.ToString();
        }
        
        public static string SetHotLap(Database database, int creationID)
        {
            var creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == creationID);
            
            if (creation == null)
                return "error_creation_not_found";
            else if (creation.Type != PlayerCreationType.TRACK && creation.Type != PlayerCreationType.STORY)
                return "error_not_a_track";
            else
            {
                var hotlap = ContentUpdates.ReadHotlapData();

                if (hotlap == null)
                {
                    ContentUpdates.GetNewHotLap(database);
                    hotlap = ContentUpdates.ReadHotlapData();
                    if (hotlap == null)
                        return "error_no_hotlap_file";
                }
                
                hotlap.TrackId = creationID;
                ContentUpdates.WriteHotlapData(hotlap);

                return "ok";
            }
        }
        
        public static string GetHotLapQueue(Database database, int page, int per_page)
        {
            HotLapData hotlap = ContentUpdates.ReadHotlapData();

            if (hotlap == null || hotlap.Queue == null)
                return "[]";

            var pageStart = PageCalculator.GetPageStart(page, per_page);
            
            var queuePage =  hotlap.Queue.Skip(pageStart).Take(per_page).ToList();

            var queue = database.PlayerCreations.Select(creation => new MinimalCreationInfo
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
            }).Where(match => queuePage.Contains(match.ID)).ToList();
            
            return JsonConvert.SerializeObject(new ModerationPageResponse<MinimalCreationInfo>
            {
                Total = hotlap.Queue.Count,
                Page = queue
            });
        }
        
        public static string AddToHotLapQueue(Database database, int creationID)
        {
            var creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == creationID);
            
            if (creation == null)
                return "error_creation_not_found";
            else if (creation.Type != PlayerCreationType.TRACK && creation.Type != PlayerCreationType.STORY)
                return "error_not_a_track";
            else
            {
                var hotlap = ContentUpdates.ReadHotlapData();

                if (hotlap == null)
                {
                    ContentUpdates.GetNewHotLap(database);
                    hotlap = ContentUpdates.ReadHotlapData();
                    if (hotlap == null)
                        return "error_no_hotlap_file";
                }

                if (hotlap.Queue == null)
                    hotlap.Queue = [creationID];
                else
                    hotlap.Queue.Add(creationID);
                
                ContentUpdates.WriteHotlapData(hotlap);

                return "ok";
            }
        }
        
        public static string RemoveFromHotLapQueue(Database database, int? index, int? creationID)
        {
            var hotlap = ContentUpdates.ReadHotlapData();

            if (hotlap == null)
            {
                ContentUpdates.GetNewHotLap(database);
                hotlap = ContentUpdates.ReadHotlapData();
                if (hotlap == null)
                    return "error_no_hotlap_file";
            }

            if (hotlap.Queue == null)
                hotlap.Queue = [];
            else
            {
                if (index != null)
                    hotlap.Queue.RemoveAt(index.Value);
                if (creationID != null)
                    hotlap.Queue.RemoveAll(match => match == creationID.Value);
            }
                
            ContentUpdates.WriteHotlapData(hotlap);

            return "ok";
        }
        #endregion

        #region ModeratorManagement
        public static string CreateModerator(Database database, string username, string password, ModeratorPermissions permissions)
        {
            var usernameIsTaken = database.Moderators.Any(match => match.Username == username);
            if (usernameIsTaken || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return usernameIsTaken ? "error_username_is_taken" : string.IsNullOrEmpty(username) ? "error_username_is_empty" : "error_password_is_empty";

            database.Moderators.Add(new()
            {
                Username = username,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                BanUsers = permissions.BanUsers,
                ChangeCreationStatus = permissions.ChangeCreationStatus,
                ChangeUserSettings = permissions.ChangeUserSettings,
                ChangeUserQuota = permissions.ChangeUserQuota,
                ManageModerators = permissions.ManageModerators,
                ViewGriefReports = permissions.ViewGriefReports,
                ViewPlayerComplaints = permissions.ViewPlayerComplaints,
                ViewPlayerCreationComplaints = permissions.ViewPlayerCreationComplaints,
                ManageHotlap = permissions.ManageHotlap,
                ManageAnnouncements = permissions.ManageAnnouncements,
                ManageSystemEvents = permissions.ManageSystemEvents
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
                ChangeUserQuota = true,
                ManageModerators = true,
                ManageAnnouncements = true,
                ManageSystemEvents = true,
                ManageHotlap = true,
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

            var query = database.Moderators.AsQueryable();

            var pageStart = PageCalculator.GetPageStart(page, per_page);

            var moderators = query.Skip(pageStart).Take(per_page).ToList();

            return JsonConvert.SerializeObject(new ModerationPageResponse<Moderator>
            {
                Total = query.Count(),
                Page = moderators
            });
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
            moderator.ChangeUserQuota = permissions.ChangeUserQuota;
            moderator.ManageModerators = permissions.ManageModerators;
            moderator.ViewGriefReports = permissions.ViewGriefReports;
            moderator.ViewPlayerComplaints = permissions.ViewPlayerComplaints;
            moderator.ViewPlayerCreationComplaints = permissions.ViewPlayerCreationComplaints;
            moderator.ManageAnnouncements = permissions.ManageAnnouncements;
            moderator.ManageHotlap = permissions.ManageHotlap;
            moderator.ManageSystemEvents = permissions.ManageSystemEvents;
            database.SaveChanges();

            return "ok";
        }
        #endregion
    }
}

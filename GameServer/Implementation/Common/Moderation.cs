using GameServer.Models;
using GameServer.Models.Config;
using GameServer.Models.GameBrowser;
using GameServer.Models.Moderation;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;

namespace GameServer.Implementation.Common
{
    public class Moderation
    {
        #region Game
        public static string GriefReport(Database database, User user, GriefReport grief_report)
        {
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

        public static string PlayerComplaints(Database database, User user, PlayerComplaint player_complaint)
        {
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

        public static string PlayerCreationComplaints(Database database, User user, PlayerCreationComplaint player_creation_complaint)
        {
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

        #region ModeratorSession

        private static void ClearSessions(Database database)
        {
            database.ModeratorSessions
                .Where(match => match.LastTokenRefresh < TimeUtils.Now.Subtract(JWTUtils.ExpirationTime))
                .ExecuteDelete();
        }
        
        private static ModeratorSession GetSession(Database database, SessionInfo sessionInfo)
        {
            ClearSessions(database);
            
            var session = database.ModeratorSessions
                .Include(s => s.User)
                .FirstOrDefault(match => match.SessionId == sessionInfo.SessionId
                                         && match.UserId == sessionInfo.UserId);

            if (session == null)
                return new ModeratorSession();
            else
                return session;
        }
        
        public static ModeratorSession GetSession(Database database, ClaimsPrincipal user)
        {
            return GetSession(database, JWTUtils.GetSessionInfo(user));
        }
        
        public static Moderator GetUser(Database database, ClaimsPrincipal user)
        {
            return GetSession(database, user).User;
        }
        #endregion
        
        #region ModeratorSelf
        public static string Login(Database database, string username, string password)
        {
            var moderator = database.Moderators.FirstOrDefault(match => match.Username == username);

            if (moderator == null || !BCrypt.Net.BCrypt.Verify(password, moderator.Password))
                return null;

            var session = new ModeratorSession
            {
                UserId = moderator.ID,
                LastTokenRefresh = TimeUtils.Now,
                SessionId = Guid.NewGuid()
            };
            database.ModeratorSessions.Add(session);
            database.SaveChanges();
            
            return JWTUtils.GenerateToken(moderator.ID, session.SessionId, true);
        }
        
        public static string RefreshToken(Database database, ModeratorSession session)
        {
            if (session?.User == null)
                return null;

            session.LastTokenRefresh = TimeUtils.Now;
            database.SaveChanges();
            
            return JWTUtils.GenerateToken(session.UserId, session.SessionId, true);
        }

        public static string GetSessions(Database database, int userID, int page, int per_page)
        {
            var query = database.ModeratorSessions.Where(match => match.UserId == userID);
            
            var pageStart = PageCalculator.GetPageStart(page, per_page);

            var sessions = query.Skip(pageStart).Take(per_page).ToList();
            
            return JsonConvert.SerializeObject(new ModerationPageResponse<ModeratorSession>
            {
                Total = query.Count(),
                Page = sessions
            });
        }

        public static string RemoveSession(Database database, int userID, Guid sessionID)
        {
            var session = database.ModeratorSessions.FirstOrDefault(match => match.UserId == userID && match.SessionId == sessionID);

            if (session == null)
                return null;

            database.ModeratorSessions.Remove(session);
            database.SaveChanges();
            
            return "ok";
        }
        
        public static string RemoveAllSessions(Database database, int userID)
        {
            if (!database.Moderators.Any(match => match.ID == userID))
                return null;
            
            database.ModeratorSessions.Where(match => match.UserId == userID).ExecuteDelete();
            
            return "ok";
        }
        
        public static string SetUsername(Database database, Moderator moderator, string username)
        {
            if (moderator == null || string.IsNullOrEmpty(username))
                return moderator == null ? "error_moderator_not_found" : "error_username_is_empty";

            moderator.Username = username;
            database.SaveChanges();

            return "ok";
        }

        public static string SetUsername(Database database, int id, string username)
        {
            return SetUsername(database, database.Moderators.FirstOrDefault(match => match.ID == id), username);
        }
        
        public static string SetPassword(Database database, Moderator moderator, string password)
        {
            if (moderator == null || string.IsNullOrEmpty(password))
                return moderator == null ? "error_moderator_not_found" : "error_password_is_empty";

            moderator.Password = BCrypt.Net.BCrypt.HashPassword(password);
            database.SaveChanges();

            return "ok";
        }
        
        public static string SetPassword(Database database, int id, string password)
        {
            return SetPassword(database, database.Moderators.FirstOrDefault(match => match.ID == id), password);
        }
        
        public static string GetPermissions(Database database, Moderator moderator)
        {
            if (moderator == null)
                return null;

            var permissions = moderator.GetPermissions();

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
            
            database.HeartedPlayerCreations
                .Where(h => h.HeartedPlayerCreationId == playerCreationID)
                .ExecuteDelete();
            
            database.PlayerCreationBookmarks
                .Where(b => b.BookmarkedPlayerCreationId == playerCreationID)
                .ExecuteDelete();
            
            database.ActivityLog
                .Where(match => match.PlayerCreationId == playerCreationID)
                .ExecuteDelete();

            return "ok";
        }
        
        public static string RemovePlayerCreation(Database database, int playerCreationID)
        {
            var Creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == playerCreationID);

            if (Creation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            Creation.Type = PlayerCreationType.DELETED;

            foreach (var item in database.PlayerCreations.Where(match => match.TrackId == Creation.PlayerCreationId).ToList())
            {
                var Photo = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == item.PlayerCreationId);
                Photo.TrackId = 4912;
            }

            database.ActivityLog.Where(match => match.PlayerCreationId == Creation.PlayerCreationId).ExecuteDelete();

            database.SaveChanges();

            if (ServerConfig.Instance.DeleteCreationData)
                UserGeneratedContentUtils.RemovePlayerCreation(playerCreationID);

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

        public static string ResetUserProfile(Database database, int targetUserId, bool removeCreations)
        {
            if (!database.Users.Any(u => u.UserId == targetUserId))
                return null;

            var user = database.Users.First(u => u.UserId == targetUserId);

            user.OnlineForfeit = 0;
            user.OnlineDisconnected = 0;
            user.WinStreak = 0;
            user.LongestWinStreak = 0;
            user.CharacterIdx = 0;
            user.KartIdx = 0;
            user.LongestDrift = 0;
            user.LongestHangTime = 0;
            user.ModMiles = 0;
            user.PlayedMNR = false;
            user.HasCheckedInBefore = false;
            user.LastLatitude = 0;
            user.LastLongitude = 0;
            user.Quote = null;

            database.PlayerExperiencePoints.Where(x => x.PlayerId == targetUserId).ExecuteDelete();
            database.PlayerPoints.Where(x => x.PlayerId == targetUserId).ExecuteDelete();
            database.TravelPoints.Where(x => x.PlayerId == targetUserId).ExecuteDelete();
            database.OnlineRacesStarted.Where(x => x.PlayerId == targetUserId).ExecuteDelete();
            database.OnlineRacesFinished.Where(x => x.PlayerId == targetUserId).ExecuteDelete();
            database.Scores.Where(x => x.PlayerId == targetUserId).ExecuteDelete();
            database.AwardUnlocks.Where(x => x.PlayerId == targetUserId).ExecuteDelete();
            database.HeartedPlayerCreations.Where(x => x.UserId == targetUserId).ExecuteDelete();
            database.PlayerCreationBookmarks.Where(x => x.UserId == targetUserId).ExecuteDelete();
            database.HeartedProfiles.Where(x => x.UserId == targetUserId || x.HeartedUserId == targetUserId).ExecuteDelete();

            database.ActivityLog.Where(match => match.AuthorId == user.UserId 
                || match.PlayerId == user.UserId).ExecuteDelete();
            
            var creationIds = database.PlayerCreations.Where(c => c.PlayerId == targetUserId)
                .Select(c => c.PlayerCreationId)
                .ToList();

            if (removeCreations)
            {
                foreach (var creationId in creationIds)
                {
                    var Creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == creationId && match.PlayerId == user.UserId);

                    if (Creation == null)
                        continue;

                    Creation.Type = PlayerCreationType.DELETED;

                    foreach (var item in database.PlayerCreations.Where(match => match.TrackId == creationId)
                                 .Select(item => item.PlayerCreationId).ToList())
                    {
                        var Photo = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == item);
                        Photo.TrackId = 4912;
                    }

                    database.ActivityLog.Where(match => match.PlayerCreationId == creationId).ExecuteDelete();

                    database.SaveChanges();

                    if (ServerConfig.Instance.DeleteCreationData)
                        UserGeneratedContentUtils.RemovePlayerCreation(creationId);
                }
            }
            else
            {
                if (creationIds.Count > 0)
                {
                    database.PlayerCreationDownloads.Where(x => creationIds.Contains(x.PlayerCreationId)).ExecuteDelete();
                    database.PlayerCreationViews.Where(x => creationIds.Contains(x.PlayerCreationId)).ExecuteDelete();
                    database.PlayerCreationRatings.Where(x => creationIds.Contains(x.PlayerCreationId)).ExecuteDelete();
                    database.PlayerCreationPoints.Where(x => creationIds.Contains(x.PlayerCreationId)).ExecuteDelete();
                    database.PlayerCreationComments.Where(x => creationIds.Contains(x.PlayerCreationId)).ExecuteDelete();
                    database.PlayerCreationReviews.Where(x => creationIds.Contains(x.PlayerCreationId)).ExecuteDelete();
                    database.HeartedPlayerCreations.Where(h => creationIds.Contains(h.HeartedPlayerCreationId)).ExecuteDelete();
                    database.PlayerCreationBookmarks.Where(b => creationIds.Contains(b.BookmarkedPlayerCreationId)).ExecuteDelete();
                    database.ActivityLog.Where(match => creationIds.Contains(match.PlayerCreationId)).ExecuteDelete();
                }
            }

            database.SaveChanges();
            return "ok";
        }
        
        public static string RemovePlayerCreations(Database database, int targetUserId)
        {
            if (!database.Users.Any(u => u.UserId == targetUserId))
                return null;
        
            var user = database.Users.First(u => u.UserId == targetUserId);
            
            var creationIds = database.PlayerCreations.Where(c => c.PlayerId == targetUserId)
                .Select(c => c.PlayerCreationId)
                .ToList();
        
            foreach (var creationId in creationIds)
            {
                var Creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == creationId && match.PlayerId == user.UserId);
        
                if (Creation == null)
                    continue;
        
                Creation.Type = PlayerCreationType.DELETED;
        
                foreach (var item in database.PlayerCreations.Where(match => match.TrackId == creationId)
                             .Select(item => item.PlayerCreationId).ToList())
                {
                    var Photo = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == item);
                    Photo.TrackId = 4912;
                }
        
                database.ActivityLog.Where(match => match.PlayerCreationId == creationId).ExecuteDelete();
        
                database.SaveChanges();
        
                if (ServerConfig.Instance.DeleteCreationData)
                    UserGeneratedContentUtils.RemovePlayerCreation(creationId);
            }
        
            database.SaveChanges();
            return "ok";
        }
        
        public static string RemoveUser(Database database, int targetUserId)
        {
            if (!database.Users.Any(u => u.UserId == targetUserId))
                return null;
        
            var user = database.Users.First(u => u.UserId == targetUserId);
            
            var creationIds = database.PlayerCreations.Where(c => c.PlayerId == targetUserId)
                .Select(c => c.PlayerCreationId)
                .ToList();
        
            
            foreach (var item in database.PlayerCreations.Where(match => creationIds.Contains(match.TrackId)).ToList())
            {
                var Photo = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == item.PlayerCreationId);
                Photo.TrackId = 4912;
            }

            database.ActivityLog.Where(match => creationIds.Contains(match.PlayerCreationId) 
                || match.AuthorId == user.UserId || match.PlayerId == user.UserId).ExecuteDelete();

            database.Users.Remove(user);
            
            database.SaveChanges();
            return "ok";
        }

        public static string GetUsers(Database database, int page, int per_page, bool? PlayedMNR, bool? IsPSNLinked, bool? IsRPCNLinked, bool? IsBanned)
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

            if (IsBanned != null)
                query = query.Where(u => u.IsBanned == IsBanned);

            var total = query.Count();

            var pageStart = PageCalculator.GetPageStart(page, per_page);

            var users = query.Select(user => new MinimalUserInfo
            {
                ID = user.UserId,
                Username = user.Username,
                PlayedMNR = user.PlayedMNR,
                IsPSNLinked = user.PSNID != 0,
                IsRPCNLinked = user.RPCNID != 0,
                IsBanned = user.IsBanned,
                AllowOppositePlatform = user.AllowOppositePlatform,
                ShowCreationsWithoutPreviews = user.ShowCreationsWithoutPreviews,
            }).Skip(pageStart).Take(per_page).ToList();

            return JsonConvert.SerializeObject(new ModerationPageResponse<MinimalUserInfo>
            {
                Total = total,
                Page = users
            });
        }
        
        public static string GetUserSessions(Database database, int userID, int page, int per_page)
        {
            var query = database.Sessions.Where(match => match.UserId == userID);
            
            var pageStart = PageCalculator.GetPageStart(page, per_page);

            var sessions = query.Skip(pageStart).Take(per_page).ToList();
            
            return JsonConvert.SerializeObject(new ModerationPageResponse<SessionData>
            {
                Total = query.Count(),
                Page = sessions
            });
        }

        public static string RemoveUserSession(Database database, int userID, Guid sessionID)
        {
            var session = database.Sessions.FirstOrDefault(match => match.UserId == userID && match.SessionId == sessionID);

            if (session == null)
                return null;

            database.Sessions.Remove(session);
            database.SaveChanges();
            ServerCommunication.NotifySessionDestroyed(sessionID);
            
            return "ok";
        }
        
        public static string RemoveAllUserSessions(Database database, int userID)
        {
            if (!database.Users.Any(match => match.UserId == userID))
                return null;
            
            var sessions = database.Sessions.Where(match => match.UserId == userID);
            
            foreach (var id in sessions.Select(s => s.SessionId).ToList())
                ServerCommunication.NotifySessionDestroyed(id);

            sessions.ExecuteDelete();
            
            return "ok";
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
            else if (!creation.IsMNR || creation.Platform != Platform.PS3)
                return "error_wrong_game_or_platform";
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
            else if (!creation.IsMNR || creation.Platform != Platform.PS3)
                return "error_wrong_game_or_platform";
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

        public static string RemoveHotLapScoreById(Database database, int scoreId)
        {
            HotLapData hotlap = ContentUpdates.ReadHotlapData();
            if (hotlap == null)
                return "error_no_hotlap_file";

            var score = database.Scores
                .FirstOrDefault(s => s.Id == scoreId
                    && s.IsMNR
                    && s.SubGroupId == 700
                    && s.SubKeyId == hotlap.TrackId);

            if (score == null)
                return null;

            database.Scores.Remove(score);
            database.SaveChanges();

            return "ok";
        }
        #endregion

        #region ScoreManagement
        public static string RemoveScoreById(Database database, int scoreId)
        {
            var score = database.Scores
                .FirstOrDefault(s => s.Id == scoreId
                    && (s.SubGroupId == 703 || s.SubGroupId == 702 || s.SubGroupId == 701));

            if (score == null)
                return null;

            if (score.IsMNR)
                UserGeneratedContentUtils.RemoveGhostCarData((GameType)(score.SubGroupId + 10), score.Platform, score.SubKeyId, score.PlayerId);

            database.Scores.Remove(score);
            database.SaveChanges();

            return "ok";
        }

        public static string RemoveAllScoresForTrack(Database database, int trackId)
        {
            var scores = database.Scores
                .Where(s => s.SubKeyId == trackId
                    && (s.SubGroupId == 703 || s.SubGroupId == 702 || s.SubGroupId == 701))
                .ToList();

            if (!scores.Any())
                return null;

            foreach (var score in scores.Where(s => s.IsMNR))
                UserGeneratedContentUtils.RemoveGhostCarData((GameType)(score.SubGroupId + 10), score.Platform, score.SubKeyId, score.PlayerId);

            database.Scores.RemoveRange(scores);
            database.SaveChanges();

            return "ok";
        }

        public static string RemoveAllScoresForPlayer(Database database, int playerId)
        {
            var scores = database.Scores
                .Where(s => s.PlayerId == playerId
                    && (s.SubGroupId == 703 || s.SubGroupId == 702 || s.SubGroupId == 701))
                .ToList();

            if (!scores.Any())
                return null;

            foreach (var score in scores.Where(s => s.IsMNR))
                UserGeneratedContentUtils.RemoveGhostCarData((GameType)(score.SubGroupId + 10), score.Platform, score.SubKeyId, score.PlayerId);

            database.Scores.RemoveRange(scores);
            database.SaveChanges();

            return "ok";
        }
        #endregion

        #region Whitelist
        public static string GetWhitelist(int page, int per_page)
        {
            var whitelist = Session.LoadWhitelist();

            if (whitelist == null)
                return "[]";

            var pageStart = PageCalculator.GetPageStart(page, per_page);
            
            var whitelistPage = whitelist.Skip(pageStart).Take(per_page).ToList();
            
            return JsonConvert.SerializeObject(new ModerationPageResponse<string>
            {
                Total = whitelist.Count,
                Page = whitelistPage
            });
        }
        
        public static string AddToWhitelist(string username)
        {
            var whitelist = Session.LoadWhitelist();

            if (whitelist == null || string.IsNullOrEmpty(username))
                return null;

            if (whitelist.Contains(username))
                return "error_already_exists";

            whitelist.Add(username);
            
            Session.WriteWhitelist(whitelist);
            
            return "ok";
        }
        
        public static string UpdateWhitelist(string oldUsername, string newUsername)
        {
            var whitelist = Session.LoadWhitelist();

            if (whitelist == null || string.IsNullOrEmpty(oldUsername) || string.IsNullOrEmpty(newUsername))
                return null;

            Session.UpdateWhitelist(oldUsername, newUsername);
            
            return "ok";
        }
        
        public static string RemoveFromWhitelist(string username)
        {
            var whitelist = Session.LoadWhitelist();

            if (whitelist == null || string.IsNullOrEmpty(username) || !whitelist.Contains(username))
                return null;

            whitelist.RemoveAll(match => match == username);
            
            Session.WriteWhitelist(whitelist);
            
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
                ManageSystemEvents = permissions.ManageSystemEvents,
                ManageWhitelist = permissions.ManageWhitelist,
                RemovePlayerCreations = permissions.RemovePlayerCreations,
                ResetCreationStats = permissions.ResetCreationStats,
                ResetUserStats = permissions.ResetUserStats,
                RemoveUsers = permissions.RemoveUsers
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
                ViewPlayerCreationComplaints = true,
                RemovePlayerCreations = true,
                ManageWhitelist = true,
                ResetCreationStats = true,
                ResetUserStats = true,
                RemoveUsers = true,
                RemoveScores = true
            });
        }

        public static string DeleteModerator(Database database, int userID, Moderator requestedBy)
        {
            var moderator = database.Moderators.FirstOrDefault(match => match.ID == userID);

            if (moderator == null) 
                return null;

            if (!requestedBy.CanDelete(moderator))
                return "error_insufficient_permissions";

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
            moderator.ManageWhitelist = permissions.ManageWhitelist;
            moderator.RemovePlayerCreations = permissions.RemovePlayerCreations;
            moderator.ResetCreationStats = permissions.ResetCreationStats;
            moderator.ResetUserStats = permissions.ResetUserStats;
            moderator.RemoveUsers = permissions.RemoveUsers;
            moderator.RemoveScores = permissions.RemoveScores;
            database.SaveChanges();

            return "ok";
        }
        #endregion
    }
}

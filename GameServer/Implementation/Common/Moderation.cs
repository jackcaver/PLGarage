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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace GameServer.Implementation.Common
{
    public class Moderation
    {
        #region Game
        public static string GriefReport(Database database, IUGCStorage storage, User user, GriefReport grief_report)
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

            storage.SaveGriefReportData(database.GriefReports.Count(),
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

            var complaint = database.PlayerComplaints.Any(match =>
                match.UserId == user.UserId &&
                match.PlayerId == player_complaint.player_id &&
                match.Reason == player_complaint.player_complaint_reason);

            if (complaint)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = 0, message = "The player complaint already exists" },
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

        public static string PlayerCreationComplaints(Database database, IUGCStorage storage, User user, PlayerCreationComplaint player_creation_complaint)
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

            var complaint = database.PlayerCreationComplaints.Any(match =>
                match.UserId == user.UserId &&
                match.PlayerCreationId == player_creation_complaint.player_creation_id &&
                match.Reason == player_creation_complaint.player_complaint_reason);

            if (complaint)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = 0, message = "The player creation complaint already exists" },
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
                storage.SavePlayerCreationComplaintPreview(database.PlayerCreationComplaints.Count(),
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
        public static string GetGriefReports(Database database, int page, int per_page, string context, int? from, SortOrder? sortOrder, ReportState? state, List<int> assignedModerators)
        {
            if (page <= 0)
                page = 1;
            if (per_page <= 0)
                per_page = 1;

            IQueryable<GriefReportData> query = database.GriefReports
                .AsNoTracking()
                .Include(r => r.ModeratorAssignments)
                .Where(match => from == null || match.UserId == from);

            if (!string.IsNullOrEmpty(context))
                query = query.Where(match => match.Context == context);
            if (state != null)
                query = query.Where(match => match.State == state);
            if (assignedModerators != null)
                foreach (var moderator in assignedModerators)
                    query = query.Where(match => match.IsAssigned(moderator));
            
            var total = query.Count();
            var pageStart = PageCalculator.GetPageStart(page, per_page);

            var reports = (sortOrder == SortOrder.desc || sortOrder == null
                    ? query.OrderByDescending(match => match.Id)
                    : query.OrderBy(match => match.Id))
                .Skip(pageStart)
                .Take(per_page)
                .ToList();

            return JsonConvert.SerializeObject(new ModerationPageResponse<GriefReportData>
            {
                Total = total,
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
        
        public static string SetGriefReportState(Database database, int id, ReportState state)
        {
            var report = database.GriefReports.FirstOrDefault(match => match.Id == id);

            if (report == null)
                return null;

            report.State = state;
            database.SaveChanges();
            
            return "ok";
        }
        #endregion

        #region CreationManagement
        public static string SetModerationStatus(Database database, int id, ModerationStatus status)
        {
            var creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == id);

            if (creation == null)
                return null;

            creation.ModerationStatus = status;

            if (status == ModerationStatus.BANNED || status == ModerationStatus.ILLEGAL)
            {
                ResetCreationStats(database, id);
                
                var hotlap = ContentUpdates.ReadHotlapData();
                if (hotlap != null && hotlap.TrackId == id)
                    ContentUpdates.GetNewHotLap(database);
            }

            database.SaveChanges();

            return "ok";
        }

        public static string GetPlayerCreationsWithStatus(Database database, int page, int per_page, ModerationStatus status, SortOrder? sortOrder)
        {
            if (page <= 0)
                page = 1;
            if (per_page <= 0)
                per_page = 1;

            var query = database.PlayerCreations
                .AsNoTracking()
                .Where(match => match.ModerationStatus == status);

            var pageStart = PageCalculator.GetPageStart(page, per_page);

            var creations = (sortOrder == SortOrder.asc
                ? query.OrderBy(match => match.PlayerCreationId)
                : query.OrderByDescending(match => match.PlayerCreationId))
            .Select(creation => new MinimalCreationInfo
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
            })
            .Skip(pageStart)
            .Take(per_page)
            .ToList();

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
        
        public static string RemovePlayerCreation(Database database, IUGCStorage storage, int playerCreationID)
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

            var hotlap = ContentUpdates.ReadHotlapData();
                if (hotlap != null && hotlap.TrackId == playerCreationID)
                    ContentUpdates.GetNewHotLap(database);

            database.PlayerCreationDownloads
                .Where(x => x.PlayerCreationId == Creation.PlayerCreationId)
                .ExecuteDelete();

            database.PlayerCreationViews
                .Where(x => x.PlayerCreationId == Creation.PlayerCreationId)
                .ExecuteDelete();

            database.PlayerCreationRatings
                .Where(x => x.PlayerCreationId == Creation.PlayerCreationId)
                .ExecuteDelete();

            database.PlayerCreationPoints
                .Where(x => x.PlayerCreationId == Creation.PlayerCreationId)
                .ExecuteDelete();

            database.PlayerCreationComments
                .Where(x => x.PlayerCreationId == Creation.PlayerCreationId)
                .ExecuteDelete();

            database.PlayerCreationReviews
                .Where(x => x.PlayerCreationId == Creation.PlayerCreationId)
                .ExecuteDelete();
            
            database.HeartedPlayerCreations
                .Where(h => h.HeartedPlayerCreationId == Creation.PlayerCreationId)
                .ExecuteDelete();
            
            database.PlayerCreationBookmarks
                .Where(b => b.BookmarkedPlayerCreationId == Creation.PlayerCreationId)
                .ExecuteDelete();
        
            database.ActivityLog
                .Where(match => match.PlayerCreationId == Creation.PlayerCreationId)
                .ExecuteDelete();

            database.SaveChanges();

            if (ServerConfig.Instance.DeleteCreationData)
                storage.RemovePlayerCreation(playerCreationID);

            return "ok";
        }

        public static string RemovePlayerCreationComment(Database database, int playerCreationID, int commentID)
        {
            var creation = database.PlayerCreations
                .AsNoTracking()
                .FirstOrDefault(match => match.PlayerCreationId == playerCreationID);

            if (creation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (creation.IsMNR)
            {
                var mnrComment = database.PlayerCreationRatings
                    .FirstOrDefault(match => match.Id == commentID && match.PlayerCreationId == playerCreationID);

                if (mnrComment == null || string.IsNullOrEmpty(mnrComment.Comment))
                {
                    var errorResp = new Response<EmptyResponse>
                    {
                        status = new ResponseStatus { id = -630, message = "No player creation comment exists for the given ID" },
                        response = new EmptyResponse { }
                    };
                    return errorResp.Serialize();
                }

                database.PlayerCreationRatings
                    .Where(match => match.Id == commentID && match.PlayerCreationId == playerCreationID && !string.IsNullOrEmpty(match.Comment))
                    .ExecuteDelete();

                return "ok";
            }

            var comment = database.PlayerCreationComments
                .FirstOrDefault(match => match.Id == commentID && match.PlayerCreationId == playerCreationID);

            if (comment == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -630, message = "No player creation comment exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.PlayerCreationComments.Remove(comment);
            database.SaveChanges();

            return "ok";
        }

        public static string RemoveAllPlayerCreationComments(Database database, int playerCreationID)
        {
            var creation = database.PlayerCreations
                .AsNoTracking()
                .FirstOrDefault(match => match.PlayerCreationId == playerCreationID);

            if (creation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (creation.IsMNR)
            {
                database.PlayerCreationRatings
                    .Where(match => match.PlayerCreationId == playerCreationID &&
                                    !string.IsNullOrEmpty(match.Comment))
                    .ExecuteDelete();

                return "ok";
            }

            var commentIds = database.PlayerCreationComments
                .Where(match => match.PlayerCreationId == playerCreationID)
                .Select(match => match.Id)
                .ToList();

            database.PlayerCreationComments
                .Where(match => match.PlayerCreationId == playerCreationID)
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

            if (isBanned)
                RemoveAllUserSessions(database, id);

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

        public static string ResetUserProfile(Database database, IUGCStorage storage, int targetUserId, bool removeCreations)
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
                RemovePlayerCreations(database, storage, targetUserId);
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
                    database.ActivityLog.Where(match => match.PlayerCreationId.HasValue && creationIds.Contains(match.PlayerCreationId.Value)).ExecuteDelete();
                }
            }

            database.SaveChanges();
            return "ok";
        }
        
        public static string RemovePlayerCreations(Database database, IUGCStorage storage, int targetUserId)
        {
            if (!database.Users.Any(u => u.UserId == targetUserId))
                return null;
            
            var creationIds = database.PlayerCreations.Where(c => c.PlayerId == targetUserId)
                .Select(c => c.PlayerCreationId)
                .ToList();
        
            foreach (var creationId in creationIds)
                RemovePlayerCreation(database, storage, creationId);
        
            database.SaveChanges();
            return "ok";
        }
        
        public static string RemoveUser(Database database, IUGCStorage storage, int targetUserId)
        {
            if (!database.Users.Any(u => u.UserId == targetUserId))
                return null;
        
            var user = database.Users.First(u => u.UserId == targetUserId);
            
            ResetUserProfile(database, storage, targetUserId, true);

            database.Users.Remove(user);
            
            database.SaveChanges();
            return "ok";
        }

        public static string GetUsers(Database database, 
            int page, int per_page, 
            bool? PlayedMNR, bool? IsPSNLinked, bool? IsRPCNLinked, 
            bool? IsBanned, SortOrder? sortOrder
        )
        {
            if (page <= 0)
                page = 1;
            if (per_page <= 0)
                per_page = 1;

            var query = database.Users.AsQueryable().AsNoTracking();

            if (PlayedMNR != null)
                query = query.Where(match => match.PlayedMNR == PlayedMNR);

            if (IsPSNLinked != null)
                query = query.Where(match => match.PSNID != 0);

            if (IsRPCNLinked != null)
                query = query.Where(match => match.RPCNID != 0);

            if (IsBanned != null)
                query = query.Where(u => u.IsBanned == IsBanned);

            var total = query.Count();
            var orderedQuery = sortOrder == SortOrder.asc
                ? query.OrderBy(user => user.UserId)
                : query.OrderByDescending(user => user.UserId);

            var pageStart = PageCalculator.GetPageStart(page, per_page);

            var users = orderedQuery.Select(user => new MinimalUserInfo
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
            var query = database.Sessions.AsNoTracking().Where(match => match.UserId == userID);
            
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

        public static string RemoveProfileComments(Database database, int targetUserId)
        {
            if (!database.Users.Any(u => u.UserId == targetUserId))
                return null;

            database.PlayerComments.Where(x => x.PlayerId == targetUserId).ExecuteDelete();

            return "ok";
        }

        public static string RemoveProfileComment(Database database, int commentId)
        {
            var comment = database.PlayerComments.FirstOrDefault(x => x.Id == commentId);

            if (comment == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -640, message = "No profile comment exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.PlayerComments.Remove(comment);
            database.SaveChanges();

            return "ok";
        }

        public static string RemoveProfileAvatars(Database database, IUGCStorage storage, int targetUserId, bool isMNR = false)
        {
            if (!database.Users.Any(u => u.UserId == targetUserId))
                return null;

            if (ServerConfig.Instance.DeleteCreationData)
                storage.RemoveProfileAvatars(targetUserId, isMNR);

            return "ok";
        }
        #endregion

        #region PlayerComplaints
        public static string GetPlayerComplaints(Database database, int page, int per_page, int? from, int? playerID, SortOrder? sortOrder, ReportState? state, List<int> assignedModerators)
        {
            if (page <= 0)
                page = 1;
            if (per_page <= 0)
                per_page = 1;

            IQueryable<PlayerComplaintData> query = database.PlayerComplaints
                .AsNoTracking()
                .Include(c => c.ModeratorAssignments)
                .Where(match => from == null || match.UserId == from);

            if (playerID != null)
                query = query.Where(match => match.PlayerId == playerID);
            if (state != null)
                query = query.Where(match => match.State == state);
            if (assignedModerators != null)
                foreach (var moderator in assignedModerators)
                    query = query.Where(match => match.IsAssigned(moderator));
            
            var pageStart = PageCalculator.GetPageStart(page, per_page);

            var reports = (sortOrder == SortOrder.desc || sortOrder == null 
                    ? query.OrderByDescending(x => x.Id) 
                    : query.OrderBy(x => x.Id))
                .Skip(pageStart)
                .Take(per_page)
                .ToList();

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
        
        public static string SetPlayerComplaintState(Database database, int id, ReportState state)
        {
            var complaint = database.PlayerComplaints.FirstOrDefault(match => match.Id == id);

            if (complaint == null)
                return null;
            
            complaint.State = state;
            database.SaveChanges();

            return "ok";
        }
        #endregion

        #region PlayerCreationComplaints
        public static string GetPlayerCreationComplaints(Database database, int page, int per_page, int? from, int? playerID, int? playerCreationID, SortOrder? sortOrder, ReportState? state, List<int> assignedModerators)
        {
            if (page <= 0)
                page = 1;
            if (per_page <= 0)
                per_page = 1;

            IQueryable<PlayerCreationComplaintData> query = database.PlayerCreationComplaints
                .AsNoTracking()
                .Include(c => c.ModeratorAssignments)
                .Where(match => from == null || match.UserId == from);

            if (playerID != null)
                query = query.Where(match => match.PlayerId == playerID);
            if (playerCreationID != null)
                query = query.Where(match => match.PlayerCreationId == playerCreationID);
            if (state != null)
                query = query.Where(match => match.State == state);
            if (assignedModerators != null)
                foreach (var moderator in assignedModerators)
                    query = query.Where(match => match.IsAssigned(moderator));
            
            var pageStart = PageCalculator.GetPageStart(page, per_page);

            var reports = (sortOrder == SortOrder.desc || sortOrder == null ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id))
                .Skip(pageStart)
                .Take(per_page)
                .ToList();

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
        
        public static string SetPlayerCreationComplaintState(Database database, int id, ReportState state)
        {
            var complaint = database.PlayerCreationComplaints.FirstOrDefault(match => match.Id == id);

            if (complaint == null)
                return null;

            complaint.State = state;
            database.SaveChanges();

            return "ok";
        }
        #endregion

        #region SystemEvents
        public static string GetSystemEvents(Database database, int page, int per_page, SortOrder? sortOrder)
        {
            if (page <= 0)
                page = 1;
            if (per_page <= 0)
                per_page = 1;

            var query = database.ActivityLog.Where(match => match.Type == ActivityType.system_event);

            var total = query.Count();
            var pageStart = PageCalculator.GetPageStart(page, per_page);

            var events = (sortOrder == SortOrder.desc || sortOrder == null
                    ? query.OrderByDescending(systemEvent => systemEvent.CreatedAt)
                    : query.OrderBy(systemEvent => systemEvent.CreatedAt))
                .Select(systemEvent => new MinimalSystemEventInfo
                {
                   CreatedAt = systemEvent.CreatedAt,
                   Topic = systemEvent.Topic,
                   Description = systemEvent.Description,
                   ImageURL = systemEvent.ImageURL
                })
                .Skip(pageStart)
                .Take(per_page)
                .ToList();

            return JsonConvert.SerializeObject(new ModerationPageResponse<MinimalSystemEventInfo>
            {
                Total = total,
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
                List = ActivityList.news_feed,
                AuthorId = null,
                PlayerId = null,
                PlayerCreationId = null
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
        public static string GetAnnouncements(Database database, int page, int per_page, Platform? platform, SortOrder? sortOrder)
        {
            if (page <= 0)
                page = 1;
            if (per_page <= 0)
                per_page = 1;

            var query = database.Announcements.AsQueryable();

            if (platform != null)
                query = query.Where(match => match.Platform == platform);

            var total = query.Count();
            var pageStart = PageCalculator.GetPageStart(page, per_page);

            var announcements = (sortOrder == SortOrder.desc || sortOrder == null
                    ? query.OrderByDescending(match => match.CreatedAt)
                    : query.OrderBy(match => match.CreatedAt))
                .Skip(pageStart)
                .Take(per_page)
                .ToList();

            return JsonConvert.SerializeObject(new ModerationPageResponse<AnnouncementData>
            {
                Total = total,
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
            else if (!creation.AutoReset)
                return "error_track_no_autoreset";
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
            else if (!creation.AutoReset)
                return "error_track_no_autoreset";
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

        public static string RemoveHotLapScore(Database database, int scoreId)
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
        public static string RemoveScore(Database database, IUGCStorage storage, int scoreId)
        {
            var score = database.Scores
                .FirstOrDefault(s => s.Id == scoreId
                    && (s.SubGroupId == 703 || s.SubGroupId == 702 || s.SubGroupId == 701));

            if (score == null)
                return null;

            if (score.IsMNR)
                storage.RemoveGhostCarData((GameType)(score.SubGroupId + 10), score.Platform, score.SubKeyId, score.PlayerId);

            database.Scores.Remove(score);
            database.SaveChanges();

            return "ok";
        }

        public static string RemoveAllScoresForTrack(Database database, IUGCStorage storage, int trackId)
        {
            var scores = database.Scores
                .Where(s => s.SubKeyId == trackId
                    && (s.SubGroupId == 703 || s.SubGroupId == 702 || s.SubGroupId == 701))
                .ToList();

            if (!scores.Any())
                return null;

            foreach (var score in scores.Where(s => s.IsMNR))
                storage.RemoveGhostCarData((GameType)(score.SubGroupId + 10), score.Platform, score.SubKeyId, score.PlayerId);

            database.Scores.RemoveRange(scores);
            database.SaveChanges();

            return "ok";
        }

        public static string RemoveAllScoresForPlayer(Database database, IUGCStorage storage, int playerId)
        {
            var scores = database.Scores
                .Where(s => s.PlayerId == playerId
                    && (s.SubGroupId == 703 || s.SubGroupId == 702 || s.SubGroupId == 701))
                .ToList();

            if (!scores.Any())
                return null;

            foreach (var score in scores.Where(s => s.IsMNR))
                storage.RemoveGhostCarData((GameType)(score.SubGroupId + 10), score.Platform, score.SubKeyId, score.PlayerId);

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

        #region IPManagement
        public static string GetBannedIPs(int page, int perPage)
        {
            var bannedIPs = Session.LoadBannedIPs();

            if (bannedIPs == null)
                return "[]";

            var pageStart = PageCalculator.GetPageStart(page, perPage);
            
            var bannedIPsPage = bannedIPs.Skip(pageStart).Take(perPage).ToList();
            
            return JsonConvert.SerializeObject(new ModerationPageResponse<string>
            {
                Total = bannedIPs.Count,
                Page = bannedIPsPage.Select(ip => ip?.ToString()).Where(ip => ip != null).ToList()
            });
        }

        public static string AddBannedIP(IPAddress ip)
        {
            var bannedIPs = Session.LoadBannedIPs();

            if (bannedIPs == null || ip == null)
                return null;

            if (bannedIPs.Any(match => Equals(match, ip)))
                return "error_already_exists";

            bannedIPs.Add(ip);
            Session.WriteBannedIPs(bannedIPs);

            return "ok";
        }

        public static string RemoveBannedIP(IPAddress ip)
        {
            var bannedIPs = Session.LoadBannedIPs();

            if (bannedIPs == null || ip == null)
                return null;

            var removed = bannedIPs.RemoveAll(match => Equals(match, ip)) > 0;

            if (!removed)
                return null;

            Session.WriteBannedIPs(bannedIPs);

            return "ok";
        }
        #endregion

        #region ConsoleIdManagement
        public static string GetBannedConsoleIds(int page, int perPage)
        {
            var bannedConsoleIds = Session.LoadBannedConsoleIds();

            if (bannedConsoleIds == null)
                return "[]";

            var pageStart = PageCalculator.GetPageStart(page, perPage);
            
            var bannedConsoleIdsPage = bannedConsoleIds.Skip(pageStart).Take(perPage).ToList();
            
            return JsonConvert.SerializeObject(new ModerationPageResponse<string>
            {
                Total = bannedConsoleIds.Count,
                Page = bannedConsoleIdsPage
            });
        }

        public static string AddBannedConsoleId(string consoleId)
        {
            var bannedConsoleIds = Session.LoadBannedConsoleIds();

            if (bannedConsoleIds == null || string.IsNullOrEmpty(consoleId))
                return null;

            if (bannedConsoleIds.Contains(consoleId))
                return "error_already_exists";

            bannedConsoleIds.Add(consoleId);
            Session.WriteBannedConsoleIds(bannedConsoleIds);

            return "ok";
        }

        public static string RemoveBannedConsoleId(string consoleId)
        {
            var bannedConsoleIds = Session.LoadBannedConsoleIds();

            if (bannedConsoleIds == null || string.IsNullOrEmpty(consoleId))
                return null;

            var removed = bannedConsoleIds.RemoveAll(match => match == consoleId) > 0;

            if (!removed)
                return null;

            Session.WriteBannedConsoleIds(bannedConsoleIds);

            return "ok";
        }
        public static string AddConsoleIdByPlayerSession(Database database, int userId)
        {
            var session = database.Sessions.FirstOrDefault(match => match.UserId == userId);

            if (session == null)
                return "no_active_session";

            if (string.IsNullOrWhiteSpace(session.ConsoleId))
                return "no_console_id_found";

            return AddBannedConsoleId(session.ConsoleId);
        }
        #endregion

        #region TeamPicksManagement
        public static string AddToTeamPicks(Database database, int creationID)
        {
            var creation = database.PlayerCreations.FirstOrDefault(x => x.PlayerCreationId == creationID);

            if (creation == null)
                return "error_creation_not_found";

            if (creation.Type == PlayerCreationType.DELETED 
                || !(creation.Type == PlayerCreationType.STORY && creation.IsMNR && creation.Platform == Platform.PS3)
                || creation.Type == PlayerCreationType.ITEM
                || creation.Type == PlayerCreationType.PHOTO
                || creation.Type == PlayerCreationType.PLANET)
                return "error_invalid_creation_type";

            if (creation.ModerationStatus == ModerationStatus.BANNED || creation.ModerationStatus == ModerationStatus.ILLEGAL)
                return "error_creation_banned_or_illegal";

            if (creation.IsTeamPick)
                return "error_already_team_pick";

            creation.IsTeamPick = true;
            creation.UpdatedAt = TimeUtils.Now;
            database.SaveChanges();

            return "ok";
        }

        public static string RemoveFromTeamPicks(Database database, int creationID)
        {
            var creation = database.PlayerCreations.FirstOrDefault(x => x.PlayerCreationId == creationID);

            if (creation == null)
                return "error_creation_not_found";

            if (!creation.IsTeamPick)
                return "error_not_team_pick";

            creation.IsTeamPick = false;
            creation.UpdatedAt = TimeUtils.Now;
            database.SaveChanges();

            return "ok";
        }

        public static string ClearTeamPicks(Database database)
        {
            database.PlayerCreations
                .Where(x => x.IsTeamPick)
                .ExecuteUpdate(setter => setter.SetProperty(x => x.IsTeamPick, false)
                                              .SetProperty(x => x.UpdatedAt, TimeUtils.Now));

            return "ok";
        }
        #endregion

        #region ModeratorAssgnment
        public static string GetModeratorAssignments(Database database, int page, int per_page, int userId, AssignmentType? type)
        {
            if (page <= 0)
                page = 1;
            if (per_page <= 0)
                per_page = 1;

            var query = database.ModeratorAssignments
                .AsNoTracking()
                .Where(match => match.UserId == userId);
            
            if (type != null)
                query = query.Where(match => match.Type == type);
            
            var pageStart = PageCalculator.GetPageStart(page, per_page);

            var assignments = query.Skip(pageStart).Take(per_page).ToList();

            return JsonConvert.SerializeObject(new ModerationPageResponse<ModeratorAssignment>
            {
                Total = query.Count(),
                Page = assignments
            });
        }
        
        public static string AssignModerator(Database database, int userId, AssignmentType type, int? griefReportId, int? playerComplaintId, int? playerCreationComplaintId)
        {
            if (!database.Moderators.Any(match => match.ID == userId))
                return "error_moderator_not_found";
            
            if (!database.Moderators.Any(match => match.ID == userId && (type == AssignmentType.GriefReport ? match.ViewGriefReports : 
                                                                          type == AssignmentType.PlayerComplaint ? match.ViewPlayerComplaints : 
                                                                          match.ViewPlayerCreationComplaints)))
                return "error_insufficient_permissions";
            
            var assignment = new ModeratorAssignment
            {
                UserId = userId,
                Type = type
            };

            switch (type)
            {
                case AssignmentType.GriefReport:
                    if (griefReportId.HasValue && database.GriefReports.Any(match => match.Id == griefReportId))
                        assignment.GriefReportId = griefReportId;
                    else
                        return "error_report_not_found";
                    break;
                
                case AssignmentType.PlayerComplaint:
                    if (playerComplaintId.HasValue && database.GriefReports.Any(match => match.Id == playerComplaintId))
                        assignment.PlayerComplaintId = playerComplaintId;
                    else
                        return "error_report_not_found";
                    break;
                
                case AssignmentType.PlayerCreationComplaint:
                    if (playerCreationComplaintId.HasValue && database.GriefReports.Any(match => match.Id == playerCreationComplaintId))
                        assignment.PlayerCreationComplaintId = playerCreationComplaintId;
                    else
                        return "error_report_not_found";
                    break;
                
                default:
                    return "error_unknown_type";
            }

            database.ModeratorAssignments.Add(assignment);
            database.SaveChanges();

            return "ok";
        }
        
        public static string RemoveAssignment(Database database, Moderator user, int id)
        {
            var assignment = database.ModeratorAssignments.FirstOrDefault(match => match.Id == id);

            if (assignment == null)
                return null;

            if (assignment.UserId != user.ID && !user.ManageModerators)
                return "error_insufficient_permissions";
            
            database.ModeratorAssignments.Remove(assignment);
            database.SaveChanges();
            
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
                ManageBannedIPs = permissions.ManageBannedIPs,
                ManageBannedConsoleIDs = permissions.ManageBannedConsoleIDs,
                ManageTeamPicks = permissions.ManageTeamPicks,
                ManageUserSessions = permissions.ManageUserSessions,
                RemovePlayerCreations = permissions.RemovePlayerCreations,
                RemovePlayerCreationComments = permissions.RemovePlayerCreationComments,
                RemoveProfileComments = permissions.RemoveProfileComments,
                RemoveProfileAvatars = permissions.RemoveProfileAvatars,
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
                ManageTeamPicks = true,
                ViewGriefReports = true,
                ViewPlayerComplaints = true,
                ViewPlayerCreationComplaints = true,
                RemovePlayerCreations = true,
                RemovePlayerCreationComments = true,
                RemoveProfileComments = true,
                ManageWhitelist = true,
                ResetCreationStats = true,
                ResetUserStats = true,
                RemoveUsers = true,
                RemoveScores = true,
                ManageUserSessions = true,
                ManageBannedIPs = true,
                ManageBannedConsoleIDs = true,
                RemoveProfileAvatars = true
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

        public static string GetModerators(Database database, int page, int per_page, SortOrder? sortOrder)
        {
            if (page <= 0)
                page = 1;
            if (per_page <= 0)
                per_page = 1;

            var query = database.Moderators.AsQueryable();

            var pageStart = PageCalculator.GetPageStart(page, per_page);

            var moderators = (sortOrder == SortOrder.desc || sortOrder == null 
                    ? query.OrderByDescending(x => x.ID) 
                    : query.OrderBy(x => x.ID))
                .Skip(pageStart)
                .Take(per_page)
                .ToList();

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

        public static string GetModeratorId(Database database, string username)
        {
            var moderator = database.Moderators.FirstOrDefault(match => match.Username == username);

            if (moderator == null)
                return "error_moderator_not_found";

            return moderator.ID.ToString();
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
            moderator.ManageTeamPicks = permissions.ManageTeamPicks;
            moderator.ManageUserSessions = permissions.ManageUserSessions;
            moderator.ManageBannedIPs = permissions.ManageBannedIPs;
            moderator.ManageBannedConsoleIDs = permissions.ManageBannedConsoleIDs;
            moderator.RemovePlayerCreations = permissions.RemovePlayerCreations;
            moderator.RemovePlayerCreationComments = permissions.RemovePlayerCreationComments;
            moderator.RemoveProfileComments = permissions.RemoveProfileComments;
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
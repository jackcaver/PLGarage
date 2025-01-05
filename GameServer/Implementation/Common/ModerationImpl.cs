using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GameServer.Implementation.Common
{
    public class ModerationImpl
    {
        private static Dictionary<Guid, ModerationSessionInfo> Sessions = []; 

        public static string GriefReport(Database database, Guid SessionID, GriefReport grief_report)
        {
            var session = SessionImpl.GetSession(SessionID);
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
            var session = SessionImpl.GetSession(SessionID);
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
            var session = SessionImpl.GetSession(SessionID);
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

        public static string Login(Database database, Guid sessionID, string login, string password)
        {
            var moderator = database.Moderators.FirstOrDefault(match => match.Username == login);

            if (moderator == null)
                return "error";

            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(password));

            if (moderator.Password != Convert.ToBase64String(hash))
                return "error";

            foreach (var session in Sessions.Where(match => match.Value.ModeratorID == moderator.ID && match.Key != sessionID))
            {
                Sessions.Remove(session.Key);
            }

            Sessions.Add(sessionID, new()
            {
                ModeratorID = moderator.ID,
                ExpiresAt = DateTime.Now.AddDays(1)
            });

            return "ok";
        }

        private static void ClearSessions()
        {
            foreach (var session in Sessions.Where(match => DateTime.Now > match.Value.ExpiresAt))
            {
                Sessions.Remove(session.Key);
            }
        }

        public static bool IsLoggedIn(Guid sessionID)
        {
            ClearSessions();
            return Sessions.ContainsKey(sessionID);
        }

        public static string GetGriefReports(Database database, string context, int? from)
        {
            List<int> reports = [];
            IQueryable<GriefReportData> baseQuery = database.GriefReports.Where(match => from == null || match.UserId == from);

            if (string.IsNullOrEmpty(context))
            {
                foreach (var report in baseQuery)
                    reports.Add(report.Id);
            }
            else
            {
                foreach (var report in baseQuery.Where(match => match.Context == context))
                    reports.Add(report.Id);
            }

            return JsonConvert.SerializeObject(reports);
        }

        public static string GetGriefReport(Database database, int id)
        {
            var report = database.GriefReports.FirstOrDefault(match => match.Id == id);

            if (report == null)
                return null;

            return JsonConvert.SerializeObject(report);
        }

        public static string SetModerationStatus(Database database, int id, ModerationStatus status)
        {
            var creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == id);

            if (creation == null)
                return null;

            creation.ModerationStatus = status;
            database.SaveChanges();

            return "ok";
        }

        public static string SetBan(Database database, int id, bool isBanned)
        {
            var user = database.Users.FirstOrDefault(match => match.UserId == id);

            if (user == null)
                return null;

            user.IsBanned = isBanned;
            database.SaveChanges();

            return "ok";
        }

        public static string GetPlayerComplaints(Database database, int? from, int? playerID)
        {
            List<int> reports = [];
            IQueryable<PlayerComplaintData> baseQuery = database.PlayerComplaints.Where(match => from == null || match.UserId == from);

            if (playerID == null)
            {
                foreach (var report in baseQuery)
                    reports.Add(report.Id);
            }
            else
            {
                foreach (var report in baseQuery.Where(match => match.PlayerId == playerID))
                    reports.Add(report.Id);
            }

            return JsonConvert.SerializeObject(reports);
        }

        public static string GetPlayerComplaint(Database database, int id)
        {
            var complaint = database.PlayerComplaints.FirstOrDefault(match => match.Id == id);

            if (complaint == null)
                return null;

            return JsonConvert.SerializeObject(complaint);
        }

        public static string GetPlayerCreationComplaints(Database database, int? from, int? playerID, int? playerCreationID)
        {
            List<int> reports = [];
            IQueryable<PlayerCreationComplaintData> baseQuery = database.PlayerCreationComplaints.Where(match => from == null || match.UserId == from);

            if (playerID != null)
                baseQuery = baseQuery.Where(match => match.PlayerId == playerID);

            if (playerCreationID == null)
            {
                foreach (var report in baseQuery)
                    reports.Add(report.Id);
            }
            else
            {
                foreach (var report in baseQuery.Where(match => match.PlayerCreationId == playerCreationID))
                    reports.Add(report.Id);
            }

            return JsonConvert.SerializeObject(reports);
        }

        public static string GetPlayerCreationComplaint(Database database, int id)
        {
            var complaint = database.PlayerCreationComplaints.FirstOrDefault(match => match.Id == id);

            if (complaint == null)
                return null;

            return JsonConvert.SerializeObject(complaint);
        }
    }
}

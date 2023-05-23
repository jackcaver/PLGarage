using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.Response;
using System.Collections.Generic;
using System;
using GameServer.Utils;
using System.Linq;
using NPTicket;
using Serilog;
using GameServer.Models.Config;
using System.IO;
using Newtonsoft.Json;

namespace GameServer.Implementation.Common
{
    public class Session //TODO: proper session handling...
    {
        private static Dictionary<Guid, SessionInfo> Sessions = new();

        public static string Login(Database database, string ip, Platform platform, string ticket, string hmac, string console_id, Guid SessionID)
        {
            ClearSessions();
            List<string> whitelist = new();
            if (ServerConfig.Instance.Whitelist)
                whitelist = LoadWhitelist();
            
            Ticket NPTicket;
            try
            {
                NPTicket = Ticket.FromBytes(Convert.FromBase64String(ticket.Split("\n\0")[0]));
            }
            catch (Exception exception)
            {
                Log.Error($"Unable to parse ticket: {exception}");
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }
            if (NPTicket.Username == "ufg")
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            User user;

            if (NPTicket.IssuerId == 0x100)
                user = database.Users.FirstOrDefault(match => match.PSNID == NPTicket.UserId);
            else if (NPTicket.IssuerId == 0x33333333)
                user = database.Users.FirstOrDefault(match => match.RPCNID == NPTicket.UserId);
            else
                user = null;

            if (user != null && user.Username != NPTicket.Username)
            {
                if (ServerConfig.Instance.Whitelist)
                    whitelist = UpdateWhitelist(user.Username, NPTicket.Username);
                user.Username = NPTicket.Username;
                database.SaveChanges();
            }

            if (database.Users.FirstOrDefault(match => match.Username == NPTicket.Username) != null && user == null)
            {
                var userByUsername = database.Users.FirstOrDefault(match => match.Username == NPTicket.Username);
                if (NPTicket.IssuerId == 0x100 && userByUsername.PSNID == 0 && userByUsername.RPCNID == 0)
                {
                    userByUsername.PSNID = NPTicket.UserId;
                    user = userByUsername;
                    database.SaveChanges();
                }
                else if (NPTicket.IssuerId == 0x33333333 && userByUsername.PSNID == 0 && userByUsername.RPCNID == 0)
                {
                    userByUsername.RPCNID = NPTicket.UserId;
                    user = userByUsername;
                    database.SaveChanges();
                }
            }

            if (user == null && (!ServerConfig.Instance.Whitelist || whitelist.Contains(NPTicket.Username)) 
                && database.Users.FirstOrDefault(match => match.Username == NPTicket.Username) != null)
            {
                var newUser = new User
                {
                    UserId = database.Users.Count(match => match.Username != "ufg") + 11,
                    Username = NPTicket.Username,
                    Quota = 30,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    PolicyAccepted = Sessions[SessionID].PolicyAccepted,
                };
                if (NPTicket.IssuerId == 0x100)
                    newUser.PSNID = NPTicket.UserId;
                else if (NPTicket.IssuerId == 0x33333333)
                    newUser.RPCNID = NPTicket.UserId;
                
                database.Users.Add(newUser);
                database.SaveChanges();
                user = database.Users.FirstOrDefault(match => match.Username == NPTicket.Username);
            }

            if (user == null || !Sessions.ContainsKey(SessionID) || (ServerConfig.Instance.Whitelist && !whitelist.Contains(user.Username)))
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            foreach (var Session in Sessions.Where(match => match.Value.Username == user.Username))
            {
                Sessions.Remove(Session.Key);
            }

            Sessions[SessionID].Ticket = NPTicket;
            Sessions[SessionID].LastPing = DateTime.UtcNow;

            var resp = new Response<List<login_data>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<login_data> {
                    new login_data {
                        ip_address = ip,
                        login_time = DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        platform = Platform.PS3.ToString(),
                        player_id = user.UserId,
                        player_name = user.Username,
                        presence = user.Presence.ToString()
                    }
                }
            };
            return resp.Serialize();
        }

        public static string SetPresence(string presence, Guid SessionID)
        {
            ClearSessions();
            int id = -130;
            string message = "The player doesn't exist";

            if (Sessions.ContainsKey(SessionID))
            {
                id = 0;
                message = "Successful completion";
                Presence userPresence = Presence.OFFLINE;
                Enum.TryParse(presence, out userPresence);
                Sessions[SessionID].Presence = userPresence;
            }

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = id, message = message },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static Presence GetPresence(string Username)
        {
            ClearSessions();
            var Session = Sessions.FirstOrDefault(match => match.Value.Username == Username).Value;
            if (Session == null) 
            {
                return Presence.OFFLINE;
            }
            return Session.Presence;
        }

        public static string Ping(Guid SessionID)
        {
            ClearSessions();

            if (!Sessions.ContainsKey(SessionID))
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            Sessions[SessionID].LastPing = DateTime.UtcNow;

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static void StartSession(Guid SessionID) 
        {
            Sessions.Add(SessionID, new SessionInfo
            {
                LastPing = DateTime.UtcNow,
                Presence = Presence.OFFLINE
            });
        }

        private static void ClearSessions()
        {
            foreach (var Session in Sessions.Where(match => match.Value.Authenticated
                && DateTime.UtcNow > match.Value.LastPing.AddMinutes(30) && DateTime.UtcNow > match.Value.ExpiryDate))
            {
                Sessions.Remove(Session.Key);
            }

            foreach (var Session in Sessions.Where(match => !match.Value.Authenticated
                && DateTime.UtcNow > match.Value.LastPing.AddHours(3)))
            {
                Sessions.Remove(Session.Key);
            }
        }

        public static SessionInfo GetSession(Guid SessionID)
        {
            ClearSessions();

            if (!Sessions.ContainsKey(SessionID))
            {
                return new SessionInfo {};
            }

            return Sessions[SessionID];
        }

        public static void AcceptPolicy(Guid SessionID) 
        {
            ClearSessions();
            if (!Sessions.ContainsKey(SessionID))
                return;
            Sessions[SessionID].PolicyAccepted = true;
        }

        public static List<string> LoadWhitelist()
        {
            if (!File.Exists("./whitelist.json"))
            {
                File.WriteAllText("./whitelist.json", JsonConvert.SerializeObject(new List<string>()));
                return new List<string>();
            }
            return JsonConvert.DeserializeObject<List<string>>(File.ReadAllText("./whitelist.json"));
        }

        private static List<string> UpdateWhitelist(string OldUsername, string NewUsername)
        {
            if (!File.Exists("./whitelist.json"))
            {
                Log.Error("Cannot update whitelist if it doesn't exist");
                return new List<string>();
            }
            List<string> whitelist = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText("./whitelist.json"));
            if (!whitelist.Contains(OldUsername))
                return whitelist;
            int EntryIndex = whitelist.FindIndex(match => match == OldUsername);
            if (EntryIndex != -1)
                whitelist[EntryIndex] = NewUsername;
            File.WriteAllText("./whitelist.json", JsonConvert.SerializeObject(whitelist));

            return whitelist;
        }
    }
}

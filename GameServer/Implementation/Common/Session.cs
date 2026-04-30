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
using Microsoft.EntityFrameworkCore;
using System.IO;
using Newtonsoft.Json;
using NPTicket.Verification;
using NPTicket.Verification.Keys;
using System.Security.Claims;

namespace GameServer.Implementation.Common
{
    public class Session
    {
        public static string Login(Database database, string ip, Platform platform, string ticket, string hmac, string console_id, bool policyAccepted, out string token)
        {
            byte[] ticketData = Convert.FromBase64String(ticket.Trim('\n').Trim('\0'));
            List<string> whitelist = [];
            if (ServerConfig.Instance.Whitelist)
                whitelist = LoadWhitelist();
            token = null;

            Ticket NPTicket;
            try
            {
                NPTicket = Ticket.ReadFromBytes(ticketData);
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

            bool IsPSN = false;
            bool IsRPCN = false;

            TicketVerifier verifier;
            switch (NPTicket.SignatureIdentifier)
            {
                case "q�\u001dJ":
                    verifier = new(ticketData, NPTicket, new LbpkSigningKey());
                    IsPSN = true;
                    break;

                case "RPCN":
                    verifier = new(ticketData, NPTicket, RpcnSigningKey.Instance);
                    IsRPCN = true;
                    break;

                default:
                    verifier = null;
                    break;
            }

            if (NPTicket.Username == "ufg" || verifier == null || !verifier.IsTicketValid())
            {
                Log.Error($"Invalid ticket from {NPTicket.Username}");
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            User user;

            if (IsPSN)
                user = database.Users.FirstOrDefault(match => match.PSNID == NPTicket.UserId);
            else if (IsRPCN)
                user = database.Users.FirstOrDefault(match => match.RPCNID == NPTicket.UserId);
            else
                user = null;

            if (user != null && user.Username != NPTicket.Username && IsPSN)
            {
                if (ServerConfig.Instance.Whitelist)
                    whitelist = UpdateWhitelist(user.Username, NPTicket.Username);
                user.Username = NPTicket.Username;
                database.SaveChanges();
            }

            var userByUsername = database.Users.FirstOrDefault(match => match.Username == NPTicket.Username);
            if (userByUsername != null && user == null)
            {
                if (IsPSN && userByUsername.PSNID == 0
                    && (userByUsername.RPCNID == 0 || userByUsername.AllowOppositePlatform))
                {
                    userByUsername.PSNID = NPTicket.UserId;
                    user = userByUsername;
                    database.SaveChanges();
                }
                else if (IsRPCN && (userByUsername.PSNID == 0 || userByUsername.AllowOppositePlatform)
                    && userByUsername.RPCNID == 0)
                {
                    userByUsername.RPCNID = NPTicket.UserId;
                    user = userByUsername;
                    database.SaveChanges();
                }
                if (userByUsername.AllowOppositePlatform)
                {
                    userByUsername.AllowOppositePlatform = false;
                    database.SaveChanges();
                }
            }

            if (user == null && (!ServerConfig.Instance.Whitelist || whitelist.Contains(NPTicket.Username)) 
                && !database.Users.Any(match => match.Username == NPTicket.Username))
            {
                var newUser = new User
                {
                    UserId = database.Users.Count(match => match.Username != "ufg") + 11,
                    Username = NPTicket.Username,
                    Quota = 30,
                    CreatedAt = TimeUtils.Now,
                    UpdatedAt = TimeUtils.Now,
                    PolicyAccepted = policyAccepted,
                };
                if (IsPSN)
                    newUser.PSNID = NPTicket.UserId;
                else if (IsRPCN)
                    newUser.RPCNID = NPTicket.UserId;
                
                database.Users.Add(newUser);
                database.SaveChanges();
                user = database.Users.FirstOrDefault(match => match.Username == NPTicket.Username);
            }

            if (user == null || user.IsBanned
                || (ServerConfig.Instance.Whitelist && !whitelist.Contains(user.Username)))
            {
                if (user == null)
                    Log.Warning($"Unable find or create user for {NPTicket.Username}");

                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var sessions = database.Sessions.Where(match => match.UserId == user.UserId && match.Platform == platform);
            
            foreach (var id in sessions.Select(s => s.SessionId).ToList())
                ServerCommunication.NotifySessionDestroyed(id);

            sessions.ExecuteDelete();

            SessionData session = new()
            {
                UserId = user.UserId,
                SessionId = Guid.NewGuid(),
                Platform = platform,
                LastPing = TimeUtils.Now,
                Presence = Presence.ONLINE
            };

            List<string> MNR_IDs = [ "BCUS98167", "BCES00701", "BCES00764", "BCJS30041", "BCAS20105", 
                "BCKS10122", "NPEA00291", "NPUA80535", "BCET70020", "NPUA70074", "NPEA90062", "NPUA70096", "NPJA90132" ];

            if (MNR_IDs.Contains(NPTicket.TitleId) || platform != Platform.PS3)
                session.IsMNR = true;

            if (session.IsMNR && !user.PlayedMNR)
            {
                user.PlayedMNR = true;
                database.SaveChanges();
            }

            if ((ServerConfig.Instance.BlockMNRPS3 && session.IsMNR && session.Platform == Platform.PS3)
                || (ServerConfig.Instance.BlockMNRPSP && session.Platform == Platform.PSP)
                || (ServerConfig.Instance.BlockMNRRT && session.Platform == Platform.PSV)
                || (ServerConfig.Instance.BlockLBPK && !session.IsMNR))
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }
            
            database.Sessions.Add(session);
            database.SaveChanges();
            token = JWTUtils.GenerateToken(user.UserId, session.SessionId);
            
            ServerCommunication.NotifySessionCreated(session.SessionId, user.UserId, user.Username, (int)NPTicket.IssuerId, platform);

            var resp = new Response<List<login_data>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [
                    new login_data {
                        ip_address = ip ?? "127.0.0.1",
                        login_time = TimeUtils.Now.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        platform = platform.ToString(),
                        player_id = user.UserId,
                        player_name = user.Username,
                        presence = user.Presence(database, platform).ToString()
                    }
                ]
            };
            return resp.Serialize();
        }

        public static string SetPresence(Database database, string presenceString, SessionInfo sessionInfo)
        {
            Ping(database, sessionInfo);
            int id = -130;
            string message = "The player doesn't exist";

            var session = database.Sessions.FirstOrDefault(match => match.UserId == sessionInfo.UserId
                                                                    && match.SessionId == sessionInfo.SessionId);
            
            if (session != null && Enum.TryParse(presenceString, out Presence presence))
            {
                id = 0;
                message = "Successful completion";

                session.Presence = presence;
                database.SaveChanges();
            }

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = id, message = message },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static Presence GetPresence(Database database, string username, Platform platform)
        {
            ClearSessions(database);
            
            var session = database.Sessions.FirstOrDefault(match => match.Username == username && match.Platform == platform);
            if (session == null) 
            {
                return Presence.OFFLINE;
            }
            return session.Presence;
        }

        public static string Ping(Database database, SessionInfo sessionInfo)
        {
            ClearSessions(database);
            int id = -130;
            string message = "The player doesn't exist";

            var session = database.Sessions.FirstOrDefault(match => match.UserId == sessionInfo.UserId
                                                                    && match.SessionId == sessionInfo.SessionId);
            
            if (session != null)
            {
                id = 0;
                message = "Successful completion";
                
                session.LastPing = TimeUtils.Now;
                database.SaveChanges();
            }
            
            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = id, message = message },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        private static void ClearSessions(Database database)
        {
            var sessions = database.Sessions.Where(match => TimeUtils.Now > match.LastPing.AddHours(1));
            
            foreach (var id in sessions.Select(s => s.SessionId).ToList())
                ServerCommunication.NotifySessionDestroyed(id);

            sessions.ExecuteDelete();
        }

        private static SessionData GetSession(Database database, SessionInfo sessionInfo)
        {
            Ping(database, sessionInfo);

            var session = database.Sessions
                .Include(s => s.User)
                .FirstOrDefault(match => match.SessionId == sessionInfo.SessionId
                                         && match.UserId == sessionInfo.UserId);
            
            if (session == null)
                return new SessionData();
            else
                return session;
        }
        
        public static SessionData GetSession(Database database, ClaimsPrincipal user)
        {
            return GetSession(database, JWTUtils.GetSessionInfo(user));
        }
        
        public static User GetUser(Database database, ClaimsPrincipal user)
        {
            return GetSession(database, user).User;
        }

        public static void WriteWhitelist(List<string> whitelist)
        {
            File.WriteAllText("./whitelist.json", JsonConvert.SerializeObject(whitelist));
        }
        
        public static List<string> LoadWhitelist()
        {
            if (!File.Exists("./whitelist.json"))
            {
                List<string> whitelist = [];
                WriteWhitelist(whitelist);
                return whitelist;
            }
            return JsonConvert.DeserializeObject<List<string>>(File.ReadAllText("./whitelist.json"));
        }

        public static List<string> UpdateWhitelist(string oldUsername, string newUsername)
        {
            List<string> whitelist = LoadWhitelist();
            if (!whitelist.Contains(oldUsername))
                return whitelist;
            int entryIndex = whitelist.FindIndex(match => match == oldUsername);
            if (entryIndex != -1)
                whitelist[entryIndex] = newUsername;
            WriteWhitelist(whitelist);

            return whitelist;
        }
    }
}

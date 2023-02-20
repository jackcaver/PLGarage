using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.Response;
using System.Collections.Generic;
using System;
using GameServer.Utils;
using System.Linq;

namespace GameServer.Implementation.Common
{
    public class Session //TODO: proper session handling...
    {
        public static string Login(Database database, string username, string ip, Platform platform, string ticket, string hmac, string console_id)
        {
            var user = database.Users.FirstOrDefault(match => match.Username == username);

            if (user == null || user.Username == "ufg")
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

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

        public static string SetPresence(Database database, string username, string presence)
        {
            int id = -130;
            string message = "The player doesn't exist";

            var user = database.Users.FirstOrDefault(match => match.Username == username);

            if (user != null)
            {
                id = 0;
                message = "Successful completion";
                Presence userPresence = Presence.OFFLINE;
                Enum.TryParse(presence, out userPresence);
                user.Presence = userPresence;
                database.SaveChanges();
            }

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = id, message = message },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static string Ping()
        {
            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }
    }
}

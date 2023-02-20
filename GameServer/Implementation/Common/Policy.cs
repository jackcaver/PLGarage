using GameServer.Models.Config;
using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Implementation.Common
{
    public class Policy
    {
        public static string View(Database database, PolicyType policy_type, Platform platform, string username)
        {
            bool is_accepted = false;
            string text = ServerConfig.Instance.NotRegisteredText.Replace("%username", username).Replace("%platform", platform.ToString());
            var user = database.Users.FirstOrDefault(match => match.Username == username);

            if (user != null || username != "ufg")
            {
                is_accepted = user.PolicyAccepted;
                text = ServerConfig.Instance.EulaText.Replace("%username", username).Replace("%platform", platform.ToString());
            }

            var resp = new Response<List<policy>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<policy> { new policy { id = (int)policy_type, is_accepted = is_accepted, name = "Online User Agreement", text = text } }
            };
            return resp.Serialize();
        }

        public static string Accept(Database database, int id, string username)
        {
            var user = database.Users.FirstOrDefault(match => match.Username == username);

            if (user != null || username != "ufg")
            {
                user.PolicyAccepted = true;
                database.SaveChanges();
            }

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }
    }
}

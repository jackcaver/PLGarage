using GameServer.Models.Config;
using GameServer.Models;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using System.Collections.Generic;
using System.Linq;
using System;
using GameServer.Models.Common;

namespace GameServer.Implementation.Common
{
    public class PolicyImpl
    {
        private static readonly string PSVitaWarning = @"WARNING: This game sends your exact location to the server you're trying to connect to.
Before continuing please make sure that you could trust the owner of this server
Your GPS data will be stored and processed by PLGarage for certain features to work
PLGarage itself does not share that data to third parties, but it is possible for the server owner to use that data to their own advantage.
PLGarage and it's developers are not reliable for any possible data leaks or blackmailing.";

        public static string View(Database database, PolicyType policy_type, Platform platform, string username)
        {
            List<string> whitelist = [];
            if (ServerConfig.Instance.Whitelist)
                whitelist = SessionImpl.LoadWhitelist();
            bool is_accepted = false;
            string text = ServerConfig.Instance.NotWhitelistedText.Replace("%username", username).Replace("%platform", platform.ToString());
            var user = database.Users.FirstOrDefault(match => match.Username == username);

            if (user != null && username != "ufg")
                is_accepted = user.PolicyAccepted;
            if ((user != null || (!ServerConfig.Instance.Whitelist || whitelist.Contains(username))) && username != "ufg")
            {
                text = ServerConfig.Instance.EulaText.Replace("%username", username).Replace("%platform", platform.ToString());
                if (platform == Platform.PSV)
                    text = text.Insert(0, PSVitaWarning+'\n');
            }

            var resp = new Response<List<PolicyResponse>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ 
                    new PolicyResponse { 
                        Id = (int)policy_type, 
                        IsAccepted = is_accepted, 
                        Name = "Online User Agreement", 
                        Text = text 
                    } 
                ]
            };
            return resp.Serialize();
        }

        public static string Accept(Database database, Guid SessionID, int id, string username)
        {
            var user = database.Users.FirstOrDefault(match => match.Username == username);

            if (user != null && username != "ufg")
            {
                user.PolicyAccepted = true;
                database.SaveChanges();
            }

            if (user == null && username != "ufg")
                SessionImpl.AcceptPolicy(SessionID);

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }
    }
}

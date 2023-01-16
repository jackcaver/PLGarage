using System.Collections.Generic;
using System.Linq;
using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace GameServer.Controllers
{
    public class PoliciesController : Controller
    {
        private readonly Database database;

        public PoliciesController(Database database)
        {
            this.database = database;
        }

        [Route("/policies/view.xml")]
        public IActionResult ViewPolicy(PolicyType policy_type, Platform platform, string username)
        {
            bool is_accepted = false;
            if (Request.Cookies["username"] != null)
                username = Request.Cookies["username"];
            else
                Response.Cookies.Append("username", username);
            string text = $"user \"{username}\" is not registered on this instance";

            var user = this.database.Users.FirstOrDefault(match => match.Username == username);

            if (user != null || user.Username == "ufg")
            {
                is_accepted = user.PolicyAccepted;
                text = $"Welcome {username}! You have successfully logged in from {platform}";
            }

            var resp = new Response<List<policy>> {
                status = new ResponseStatus { id = 0, message = "Successful completion"},
                response = new List<policy> { new policy { id = (int)policy_type, is_accepted = is_accepted, name = "Online User Agreement", text = text}}
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("policies/{id}/accept.xml")]
        public IActionResult AcceptPolicy(int id, string username)
        {
            var user = this.database.Users.FirstOrDefault(match => match.Username == username);

            if (user != null || user.Username == "ufg")
            {
                user.PolicyAccepted = true;
                this.database.SaveChanges();
            }

            var resp = new Response<EmptyResponse> {
                status = new ResponseStatus { id = 0, message = "Successful completion"},
                response = new EmptyResponse {}
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
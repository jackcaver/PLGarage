using System.Collections.Generic;
using GameServer.Models;
using GameServer.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace GameServer.Controllers
{
    public class PoliciesController : Controller
    {
        [Route("/policies/view.xml")]
        public IActionResult ViewPolicy(string policy_type, string platform, string username)
        {
            Log.Information($"{username} is trying to get {policy_type} for {platform}");
            Response.Cookies.Append("username", username);
            var resp = new Response<List<policy>> {
                status = new ResponseStatus { id = 0, message = "Successful completion"},
                response = new List<policy> { new policy { id = 1, is_accepted = false, name = "Online User Agreement", text = $"your username is {username} and you're a {platform} user, right?"}}
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("policies/{id}/accept.xml")]
        public IActionResult AcceptPolicy(int id, string username)
        {
            Log.Information($"{username} has accepted policy with id {id}");
            var resp = new Response<EmptyResponse> {
                status = new ResponseStatus { id = 0, message = "Successful completion"},
                response = new EmptyResponse {}
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
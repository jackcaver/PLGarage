using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Implementation.Common;
using GameServer.Models;
using GameServer.Models.Config;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace GameServer.Controllers.Common
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
            if (Request.Cookies["username"] != null)
                username = Request.Cookies["username"];
            else
                Response.Cookies.Append("username", username);

            return Content(Policy.View(database, policy_type, platform, username), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("policies/{id}/accept.xml")]
        public IActionResult AcceptPolicy(int id, string username)
        {
            return Content(Policy.Accept(database, id, username.Split("\0")[0]), "application/xml;charset=utf-8");
        }
    }
}
using System;
using GameServer.Implementation.Common;
using GameServer.Models.Common;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;

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
            if (Request.Cookies.ContainsKey("session_id") 
                && Session.GetSession(Guid.Parse(Request.Cookies["session_id"])).Username != null 
                && (username == null || (platform == Platform.PSV && username == "X")))
                username = Session.GetSession(Guid.Parse(Request.Cookies["session_id"])).Username;

            return Content(Policy.View(database, policy_type, platform, username), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("policies/{id}/accept.xml")]
        public IActionResult AcceptPolicy(int id, string username)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(Policy.Accept(database, SessionID, id, username.Split("\0")[0]), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
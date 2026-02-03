using GameServer.Implementation.Common;
using GameServer.Models.PlayerData;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GameServer.Controllers.Common
{
    public class SessionController : Controller
    {
        private readonly Database database;

        public SessionController(Database database)
        {
            this.database = database;
        }

        [HttpPost]
        [Route("session/login_np.xml")]
        public IActionResult Login(Platform platform, string ticket, string hmac, string console_id)
        {
            Guid SessionID = Guid.Empty;
            if (!(Request.Cookies.ContainsKey("session_id") 
                 && Guid.TryParse(Request.Cookies["session_id"], out SessionID) 
                 && Session.SessionExists(SessionID)))
            {
                SessionID = Session.StartSession();
                Response.Cookies.Append("session_id", SessionID.ToString());
            }
            return Content(Session.Login(database, HttpContext.Connection.RemoteIpAddress.ToString(),
                platform, ticket, hmac, console_id, SessionID), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("session/set_presence.xml")]
        public IActionResult SetPresence(string presence)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(Session.SetPresence(presence.Split("\0")[0], SessionID), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("session/ping.xml")]
        public IActionResult Ping()
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            
            return Content(Session.Ping(SessionID), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
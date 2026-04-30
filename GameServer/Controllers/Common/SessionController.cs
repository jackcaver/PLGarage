using GameServer.Implementation.Common;
using GameServer.Models.PlayerData;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace GameServer.Controllers.Common
{
    public class SessionController(Database database) : Controller
    {
        [HttpPost]
        [Route("session/login_np.xml")]
        public IActionResult Login(Platform platform, string ticket, string hmac, string console_id)
        {
            bool policyAccepted = Request.Cookies.TryGetValue("PolicyAccepted", out string policyAcceptedCookie)
                && bool.TryParse(policyAcceptedCookie.ToLower(), out bool accepted) && accepted;

            var result = Session.Login(database, HttpContext.Connection.RemoteIpAddress?.ToString(),
                platform, ticket, hmac, console_id, policyAccepted, out string token);
            
            if (token != null)
                Response.Cookies.Append("Token", token);
            
            return Content(result, "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("session/set_presence.xml")]
        public IActionResult SetPresence(string presence)
        {
            return Content(Session.SetPresence(database, presence.TrimEnd('\0'), JWTUtils.GetSessionInfo(User)), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("session/ping.xml")]
        public IActionResult Ping()
        {
            var iatString = User.FindFirstValue("iat");
            if (int.TryParse(iatString, out int issuedAt)
                && DateTimeOffset.FromUnixTimeSeconds(issuedAt)
                    .DateTime.Add(JWTUtils.ExpirationTime) > TimeUtils.Now.ToUniversalTime()
                && DateTimeOffset.FromUnixTimeSeconds(issuedAt)
                    .DateTime.Add(JWTUtils.RefreshWindowStart) < TimeUtils.Now.ToUniversalTime())
                Response.Cookies.Append("Token", JWTUtils.GenerateToken(JWTUtils.GetSessionInfo(User)));
            
            return Content(Session.Ping(database, JWTUtils.GetSessionInfo(User)), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
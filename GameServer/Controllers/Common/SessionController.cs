using GameServer.Implementation.Common;
using GameServer.Models.PlayerData;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;

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
            return Content(Session.Login(database, Request.Cookies["username"], HttpContext.Connection.RemoteIpAddress.ToString(),
                platform, ticket, hmac, console_id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("session/set_presence.xml")]
        public IActionResult SetPresence(string presence)
        {
            return Content(Session.SetPresence(database, Request.Cookies["username"], presence.Split("\0")[0]), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("session/ping.xml")]
        public IActionResult Ping()
        {
            return Content(Session.Ping(), "application/xml;charset=utf-8");
        }
    }
}
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GameServer.Implementation.Common;
using GameServer.Utils;

namespace GameServer.Controllers.Api
{
    public class PlayerCountApiController : Controller
    {
        private readonly Database database;

        public PlayerCountApiController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("/api/playercounts/sessioncount")]
        public IActionResult GetSessionCount(bool? isMnr = null)
        {
            return Content($"{Session.GetSessions()
                .Where(x =>
                    x != null &&    // TODO: Why are session objects becoming null?
                    (isMnr != null ? x.IsMNR == isMnr : true) &&
                    x.Authenticated &&
                    x.LastPing.AddMinutes(1) > DateTime.Now)
                .Count()}");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}

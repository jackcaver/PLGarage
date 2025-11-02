using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameServer.Implementation.Common;
using GameServer.Models.Config;
using GameServer.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;

namespace GameServer.Controllers.Api
{
    public class SessionApiController : Controller
    {
        private readonly Database database;

        public SessionApiController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("api/get_player_count")]
        public IActionResult GetPlayerCount(bool? isMnr = null)
        {
            return Content($"{Session.GetSessions()
                .Where(x => 
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

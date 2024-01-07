using System;
using System.Collections.Generic;
using GameServer.Implementation.Common;
using GameServer.Models;
using GameServer.Models.Request;
using GameServer.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player
{
    [Route("preferences.xml")]
    public class PrefferencesController : Controller
    {
        [HttpPut]
        [HttpPost]
        public IActionResult UpdatePreferences(ClientPreferences preference)
        {
            if (Request.Cookies["session_id"] == null)
            {
                Guid SessionID = Guid.NewGuid();
                Response.Cookies.Append("session_id", SessionID.ToString());
                Session.StartSession(SessionID);
            }

            var resp = new Response<List<Preference>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<Preference> { new Preference {
                    domain = preference.domain, ip_address = HttpContext.Connection.RemoteIpAddress.ToString(),
                    language_code = preference.language_code, region_code = preference.region_code, timezone = preference.timezone
                } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
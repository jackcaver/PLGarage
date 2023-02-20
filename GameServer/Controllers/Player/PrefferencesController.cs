using System.Collections.Generic;
using GameServer.Models;
using GameServer.Models.Request;
using GameServer.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player
{
    [Route("preferences.xml")]
    public class PrefferencesController : Controller
    {
        [HttpPost]
        public IActionResult UpdatePreferences(ClientPreferences preference)
        {
            var resp = new Response<List<preference>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<preference> { new preference {
                    domain = preference.domain, ip_address = HttpContext.Connection.RemoteIpAddress.ToString(),
                    language_code = preference.language_code, region_code = preference.region_code, timezone = preference.timezone
                } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
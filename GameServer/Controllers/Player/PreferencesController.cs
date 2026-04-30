using System.Collections.Generic;
using GameServer.Models;
using GameServer.Models.Request;
using GameServer.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GameServer.Controllers.Player
{
    [Route("preferences.xml")]
    public class PreferencesController : Controller
    {
        [HttpPut]
        [HttpPost]
        public IActionResult UpdatePreferences(ClientPreferences preference)
        {
            Response.Cookies.Append("ClientPreferences", JsonConvert.SerializeObject(preference));
            
            var resp = new Response<List<Preference>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new Preference
                {
                    domain = preference.domain,
                    ip_address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1",
                    language_code = preference.language_code,
                    region_code = preference.region_code,
                    timezone = preference.timezone
                } ]
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
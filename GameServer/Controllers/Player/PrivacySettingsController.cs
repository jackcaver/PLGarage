using GameServer.Models;
using GameServer.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player
{
    public class PrivacySettingsController : Controller
    {
        [HttpGet]
        [Route("privacy_setting.xml")]
        public IActionResult Get()
        {
            var resp = new Response<privacy_settings>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new privacy_settings
                {
                    profile_acls = new profile_acls
                    {
                        allow_all = false,
                        allow_psn = false,
                        deny_all = true
                    },
                    player_creation_acls = new player_creation_acls
                    {
                        allow_all = false,
                        allow_psn = false,
                        deny_all = true
                    }
                }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
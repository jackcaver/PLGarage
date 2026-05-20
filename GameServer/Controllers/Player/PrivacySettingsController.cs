using GameServer.Implementation.Common;
using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GameServer.Controllers.Player
{
    public class PrivacySettingsController(Database database) : Controller
    {
        [HttpGet]
        [Authorize]
        [Route("privacy_setting.xml")]
        public IActionResult Show()
        {
            var user = Session.GetUser(database, User);
            
            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }
            
            var resp = new Response<List<privacy_settings>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [new privacy_settings
                {
                    profile_acls = new profile_acls
                    {
                        allow_all = user.ProfilePrivacy == PrivacyType.AllowAll,
                        allow_psn = user.ProfilePrivacy == PrivacyType.AllowPSN,
                        deny_all = user.ProfilePrivacy == PrivacyType.DenyAll
                    },
                    player_creation_acls = new player_creation_acls
                    {
                        allow_all = user.PlayerCreationsPrivacy == PrivacyType.AllowAll,
                        allow_psn = user.PlayerCreationsPrivacy == PrivacyType.AllowPSN,
                        deny_all = user.PlayerCreationsPrivacy == PrivacyType.DenyAll
                    }
                }]
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
        
        [HttpPut]
        [HttpPost]
        [Authorize]
        [Route("privacy_setting.xml")]
        public IActionResult Update(PrivacySettings privacy_settings)
        {
            var user = Session.GetUser(database, User);
            
            if (user == null 
                || privacy_settings.profile_acl < 0 || privacy_settings.profile_acl > 2 
                || privacy_settings.player_creation_acl < 0 || privacy_settings.player_creation_acl > 2)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            user.ProfilePrivacy = (PrivacyType)privacy_settings.profile_acl;
            user.PlayerCreationsPrivacy = (PrivacyType)privacy_settings.player_creation_acl;
            database.SaveChanges();
            
            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse()
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
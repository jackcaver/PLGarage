using System.Collections.Generic;
using GameServer.Models;
using GameServer.Models.Request;
using GameServer.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers
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
                response = new List<preference> { new preference { domain = preference.domain, ip_address = HttpContext.Connection.RemoteIpAddress.ToString(), language_code = preference.language_code, region_code = preference.region_code, timezone = preference.timezone } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }

    public class AchievementsController : Controller
    {
        [Route("achievements.xml")]
        public IActionResult Get() 
        {
            var resp = new Response<List<achievements>> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<achievements> { new achievements { total = 0, AchievementList = new List<achievement> {} } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }

    public class FavoritePlayersController : Controller
    {
        [HttpPost]
        [Route("favorite_players.xml")]
        public IActionResult Create() 
        {
            var resp = new Response<EmptyResponse> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse {}
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpDelete]
        [Route("favorite_players/remove.xml")]
        public IActionResult Remove() 
        {
            var resp = new Response<EmptyResponse> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse {}
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("favorite_players.xml")]
        public IActionResult Get() 
        {
            var resp = new Response<List<favorite_players>> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<favorite_players> { new favorite_players { total = 0 } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }

    public class BuddiesController : Controller
    {
        [HttpPost]
        [Route("buddies/replicate.xml")]
        public IActionResult Replicate(List<string> usernames, List<string> blocked_usernames) 
        {
            var resp = new Response<EmptyResponse> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse {}
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }

    public class PlayerGlickosController : Controller
    {
        [HttpGet]
        [Route("player_glickos/bulk_fetch.xml")]
        public IActionResult BulkFetch(List<string> usernames, List<string> blocked_usernames) 
        {
            var resp = new Response<List<player_metrics>> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_metrics> { new player_metrics { total = 0 } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }

    public class PlayerController : Controller
    {
        [Route("players/to_id.xml")]
        public IActionResult ToID(string username) 
        {
            var resp = new Response<PlayerIDResponse> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new PlayerIDResponse { player_id = 1 }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [Route("players/{id}/info.xml")]
        public IActionResult GetPlayerInfo(int id, string platfom) 
        {
            var resp = new Response<List<player>> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player> {}
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("planet.xml")]
        public IActionResult GetPlanet(int player_id, bool is_counted) 
        {
            var resp = new Response<List<player>> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player> {}
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
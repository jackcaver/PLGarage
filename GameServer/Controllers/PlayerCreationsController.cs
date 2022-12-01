using System.Collections.Generic;
using GameServer.Models;
using GameServer.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace GameServer.Controllers
{
    public class PlayerCreationBookmarksController : Controller
    {
        [HttpPost]
        [Route("player_creation_bookmarks.xml")]
        public IActionResult Create(int player_creation_id) 
        {
            var resp = new Response<EmptyResponse> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse {}
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpDelete]
        [Route("player_creation_bookmarks/remove.xml")]
        public IActionResult Remove(int player_creation_id) 
        {
            var resp = new Response<EmptyResponse> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse {}
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [Route("player_creation_bookmarks/tally.xml")]
        public IActionResult Get() 
        {
            var resp = new Response<List<PlayerCreationBookmarks>> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<PlayerCreationBookmarks> { new PlayerCreationBookmarks { total = 0 } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }

    public class PlayerCreations : Controller
    {
        [HttpPost]
        [Route("player_creations/verify.xml")]
        public IActionResult Verify(List<int> id, List<int> offline_id) 
        {
            foreach (int item in id)
            {
                Log.Information($"id:{item}");
            }
            foreach (int item in offline_id)
            {
                Log.Information($"offline_id:{item}");
            }
            var resp = new Response<EmptyResponse> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse {}
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
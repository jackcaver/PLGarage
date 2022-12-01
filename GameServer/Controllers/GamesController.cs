using System.Collections.Generic;
using GameServer.Models;
using GameServer.Models.Request;
using GameServer.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers
{
    public class GamesController : Controller
    {
        [HttpPost]
        [Route("single_player_games/create_finish_and_post_stats.xml")]
        public IActionResult PostSinglePlayerGameStats(Game game, GamePlayer game_player, GamePlayerStats game_player_stats) 
        {
            var resp = new Response<EmptyResponse> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse {}
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
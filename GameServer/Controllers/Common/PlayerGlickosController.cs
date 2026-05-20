using GameServer.Implementation.Common;
using System.Collections.Generic;
using GameServer.Models;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;

namespace GameServer.Controllers.Common
{
    public class PlayerGlickosController(Database database) : Controller
    {
        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("player_glickos/bulk_fetch.xml")]
        public IActionResult BulkFetch(int[] player_ids)
        {
            var session = Session.GetSession(database, User);
            var query = database.Users
                .AsNoTracking()
                .Include(u => u.PlayerPoints)
                .Include(u => u.RacesStarted)
                .Where(match => player_ids.Contains(match.UserId));
            
            var total = query.Count();

            if (total < 1)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var metrics = query.Select(user => new player_metric
            {
                points = user.Points(session.Platform).ToString("0.0", CultureInfo.InvariantCulture),
                player_id = user.UserId,
                deviation = "350.0",
                volatility = "0.06",
                num_games = user.OnlineRaces.ToString()
            }).ToList();
            
            var resp = new Response<List<player_metrics>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [new player_metrics { total = total, Metrics = metrics}]
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
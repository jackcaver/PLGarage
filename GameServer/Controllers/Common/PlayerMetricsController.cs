using GameServer.Implementation.Common;
using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GameServer.Controllers.Common
{
    public class PlayerMetricsController(Database database) : Controller
    {
        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("player_metrics.xml")]
        public IActionResult Show(PlayerMetricType player_metric_type, int player_id, string username)
        {
            var session = Session.GetSession(database, User);
            var user = database.Users.FirstOrDefault(match => match.Username == username || match.UserId == player_id);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse() { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var metric = new player_metric
            {
                player_metric_type = player_metric_type.ToString(),
                player_id = user.UserId,
                username = user.Username
            };

            if (player_metric_type == PlayerMetricType.GLICKO)
            {
                metric.points = user.Points(session.Platform).ToString("0.0", CultureInfo.InvariantCulture);
                metric.deviation = "350.0";
                metric.volatility = "0.06";
                metric.num_games = user.OnlineRaces.ToString();
            }
            else
            {
                var analytics = database.Analytics.FirstOrDefault(match => match.UserId == user.UserId);
                
                if (analytics == null)
                {
                    var errorResp = new Response<EmptyResponse>
                    {
                        status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                        response = new EmptyResponse() { }
                    };
                    return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
                }
                
                metric.id = analytics.Id.ToString();
                metric.data = analytics.Data;
                metric.created_at = analytics.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz");
                metric.updated_at = analytics.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz");
            }
            
            var resp = new Response<List<player_metric>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [metric]
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
        
        [HttpPut]
        [Authorize]
        [Route("player_metrics.xml")]
        public IActionResult Update(string username, PlayerMetricType player_metric_type, PlayerMetric player_metric)
        {
            var user = Session.GetUser(database, User);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse() { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            if (user.Username == username && player_metric_type == PlayerMetricType.ANALYTICS)
            {
                var analytics = database.Analytics.FirstOrDefault(match => match.UserId == user.UserId);

                if (analytics == null)
                {
                    database.Analytics.Add(new Analytics
                    {
                        UserId = user.UserId,
                        CreatedAt = TimeUtils.Now,
                        UpdatedAt = TimeUtils.Now,
                        Data = player_metric.data
                    });
                }
                else
                {
                    analytics.Data = player_metric.data;
                    analytics.UpdatedAt = TimeUtils.Now;
                }
                
                database.SaveChanges();
            }
            
            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse()
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}

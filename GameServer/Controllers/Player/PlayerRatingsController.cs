using GameServer.Implementation.Common;
using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GameServer.Controllers.Player
{
    public class PlayerRatingsController(Database database) : Controller
    {
        [HttpPost]
        [Authorize]
        [Route("player_ratings.xml")]
        public IActionResult Create(PlayerRating player_rating)
        {
            var requestedBy = Session.GetUser(database, User);
            var user = database.Users.FirstOrDefault(match => match.UserId == player_rating.player_id);

            if (user == null || requestedBy == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var rating = database.PlayerRatings.FirstOrDefault(match => match.PlayerId == player_rating.player_id && match.AuthorId == requestedBy.UserId);

            if (rating == null)
            {
                database.PlayerRatings.Add(new PlayerRatingData {
                    AuthorId = requestedBy.UserId,
                    PlayerId = player_rating.player_id,
                    Rating = player_rating.rating,
                    Comment = player_rating.comments,
                    RatedAt = TimeUtils.Now
                });
                database.SaveChanges();
            }

            if (rating != null)
            {
                rating.Rating = player_rating.rating;
                rating.Comment = player_rating.comments;
                database.SaveChanges();
            }

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}

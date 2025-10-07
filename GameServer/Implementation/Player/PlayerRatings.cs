using GameServer.Implementation.Common;
using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using System;
using System.Linq;

namespace GameServer.Implementation.Player
{
    public class PlayerRatings
    {
        public static string Create(Database database, Guid SessionID, PlayerRating player_rating)
        {
            var session = Session.GetSession(SessionID);
            var requestedBy = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var user = database.Users.FirstOrDefault(match => match.UserId == player_rating.player_id);

            if (user == null || requestedBy == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
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
            return resp.Serialize();
        }
    }
}

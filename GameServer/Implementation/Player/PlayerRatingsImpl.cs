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
    public class PlayerRatingsImpl
    {
        public static string Create(Database database, Guid SessionID, PlayerRating player_rating)
        {
            var session = SessionImpl.GetSession(SessionID);
            var requestedBy = database.Users.FirstOrDefault(match => match.Username == session.Username);

            var user = database.Users
                .FirstOrDefault(match => match.UserId == player_rating.player_id);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var rating = database.PlayerRatings
                .FirstOrDefault(match => match.Player.UserId == player_rating.player_id && match.Author.UserId == requestedBy.UserId);

            if (rating == null)
            {
                database.PlayerRatings.Add(new PlayerRatingData {
                    Author = requestedBy,
                    Player = user,
                    Rating = player_rating.rating,
                    Comment = player_rating.comments,
                    RatedAt = DateTime.UtcNow
                });
                database.SaveChanges();
            }
            else
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

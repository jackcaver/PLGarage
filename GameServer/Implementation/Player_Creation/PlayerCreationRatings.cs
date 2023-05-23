using GameServer.Models.Response;
using GameServer.Models;
using System.Collections.Generic;
using GameServer.Utils;
using System.Linq;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.PlayerData;
using System;
using GameServer.Models.Request;
using GameServer.Implementation.Common;

namespace GameServer.Implementation.Player_Creation
{
    public class PlayerCreationRatings
    {
        public static string View(Database database, Guid SessionID, int player_creation_id, int player_id)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var rating = database.PlayerCreationRatings.FirstOrDefault(match => match.PlayerCreationId == player_creation_id && match.PlayerId == user.UserId);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var resp = new Response<List<player_creation_rating>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_creation_rating> { new player_creation_rating {
                    comments = rating != null ? rating.Comment : null,
                    rating = rating != null
                } }
            };
            return resp.Serialize();
        }

        public static string Create(Database database, Guid SessionID, PlayerCreationRating player_creation_rating)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var Creation = database.PlayerCreationComments.FirstOrDefault(match => match.Id == player_creation_rating.player_creation_id);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var rating = database.PlayerCreationRatings.FirstOrDefault(match => match.PlayerCreationId == player_creation_rating.player_creation_id && match.PlayerId == user.UserId);

            if (rating == null)
            {
                database.PlayerCreationRatings.Add(new PlayerCreationRatingData
                {
                    PlayerCreationId = player_creation_rating.player_creation_id,
                    PlayerId = user.UserId,
                    Type = RatingType.YAY,
                    RatedAt = DateTime.UtcNow
                });
                database.SaveChanges();
            }

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static string Clear(Database database, Guid SessionID, int player_creation_id)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var rating = database.PlayerCreationRatings.FirstOrDefault(match => match.PlayerId == user.UserId && match.PlayerCreationId == player_creation_id);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.PlayerCreationRatings.Remove(rating);
            database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }
    }
}

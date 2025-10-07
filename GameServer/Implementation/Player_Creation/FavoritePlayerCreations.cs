using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Response;
using GameServer.Models;
using System;
using GameServer.Utils;
using System.Linq;
using System.Collections.Generic;
using GameServer.Implementation.Common;
using GameServer.Models.PlayerData;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Implementation.Player_Creation
{
    public class FavoritePlayerCreations
    {
        public static string AddToFavorites(Database database, Guid SessionID, int id)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var Creation = database.PlayerCreations
                .Include(x => x.Hearts)
                .FirstOrDefault(match => match.PlayerCreationId == id);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (Creation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (!Creation.IsHeartedByMe(user.UserId))
            {
                database.HeartedPlayerCreations.Add(new HeartedPlayerCreation
                {
                    HeartedPlayerCreationId = Creation.PlayerCreationId,
                    UserId = user.UserId,
                    HeartedAt = TimeUtils.Now,
                });
                if (!session.IsMNR)
                {
                    database.ActivityLog.Add(new ActivityEvent
                    {
                        AuthorId = user.UserId,
                        Type = ActivityType.player_creation_event,
                        List = ActivityList.activity_log,
                        Topic = "player_creation_hearted",
                        Description = "",
                        PlayerId = 0,
                        PlayerCreationId = Creation.PlayerCreationId,
                        CreatedAt = TimeUtils.Now,
                        AllusionId = Creation.PlayerCreationId,
                        AllusionType = "PlayerCreation::Track"
                    });
                }
                database.SaveChanges();
            }

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static string RemoveFromFavorites(Database database, Guid SessionID, int id)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var HeartedCreation = database.HeartedPlayerCreations.FirstOrDefault(match => match.HeartedPlayerCreationId == id && match.UserId == user.UserId);

            if (HeartedCreation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.HeartedPlayerCreations.Remove(HeartedCreation);
            database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static string ListFavorites(Database database, Guid SessionID, string player_id_or_username)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == player_id_or_username || match.UserId.ToString() == player_id_or_username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var favoriteCrations = database.HeartedPlayerCreations
                .Include(h => h.HeartedCreation)
                .Where(match => match.UserId == user.UserId && match.HeartedCreation.IsMNR == session.IsMNR).ToList();
            List<favorite_player_creation> favoriteCreationsList = [];

            foreach (var creation in favoriteCrations)
            {
                favoriteCreationsList.Add(new favorite_player_creation
                {
                    player_creation_id = creation.HeartedPlayerCreationId,
                    player_creation_name = creation.Name
                });
            }

            var resp = new Response<List<favorite_player_creations>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new favorite_player_creations {
                    total = favoriteCreationsList.Count,
                    PlayerCreations = favoriteCreationsList
                } ]
            };
            return resp.Serialize();
        }
    }
}

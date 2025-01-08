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
using AutoMapper.QueryableExtensions;

namespace GameServer.Implementation.Player_Creation
{
    public class FavoritePlayerCreationsImpl
    {
        public static string AddToFavorites(Database database, Guid SessionID, int id)
        {
            var session = SessionImpl.GetSession(SessionID);
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

            var creation = database.PlayerCreations
                .Include(x => x.Hearts)
                .ThenInclude(x => x.User)
                .FirstOrDefault(match => match.Id == id);

            if (creation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (!creation.Hearts.Any(x => x.User.UserId == user.UserId))
            {
                database.HeartedPlayerCreations.Add(new HeartedPlayerCreation
                {
                    HeartedCreation = creation,
                    User = user,
                    HeartedAt = DateTime.UtcNow,
                });
                if (!session.IsMNR)
                {
                    database.ActivityLog.Add(new ActivityEvent
                    {
                        Author = user,
                        Type = ActivityType.player_creation_event,
                        List = ActivityList.activity_log,
                        Topic = "player_creation_hearted",
                        Description = "",
                        Creation = creation,
                        CreatedAt = DateTime.UtcNow,
                        AllusionId = creation.Id,
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
            var session = SessionImpl.GetSession(SessionID);
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

            var heartedCreation = database.HeartedPlayerCreations
                .Include(x => x.HeartedCreation)
                .Include(x => x.User)
                .FirstOrDefault(match => match.HeartedCreation.Id == id && match.User.UserId == user.UserId);

            if (heartedCreation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.HeartedPlayerCreations.Remove(heartedCreation);
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
            var session = SessionImpl.GetSession(SessionID);
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

            var favoriteCreations = database.HeartedPlayerCreations
                .Include(x => x.User)
                .Where(match => match.User.UserId == user.UserId && match.HeartedCreation.IsMNR == session.IsMNR)
                .ProjectTo<FavoritePlayerCreation>(database.MapperConfig)
                .ToList();

            var resp = new Response<List<FavoritePlayerCreations>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new FavoritePlayerCreations {
                    Total = favoriteCreations.Count,
                    PlayerCreations = favoriteCreations
                } ]
            };
            return resp.Serialize();
        }
    }
}

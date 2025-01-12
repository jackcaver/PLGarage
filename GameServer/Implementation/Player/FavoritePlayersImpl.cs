using GameServer.Models.PlayerData;
using GameServer.Models.Response;
using GameServer.Models;
using System;
using GameServer.Utils;
using System.Linq;
using GameServer.Models.Request;
using System.Collections.Generic;
using GameServer.Implementation.Common;
using AutoMapper.QueryableExtensions;

namespace GameServer.Implementation.Player
{
    public class FavoritePlayersImpl
    {
        public static string AddToFavorites(Database database, Guid SessionID, Models.Request.FavoritePlayer favorite_player)
        {
            var session = SessionImpl.GetSession(SessionID);
            var requestedBy = database.Users.FirstOrDefault(match => match.Username == session.Username);

            var user = database.Users
                .FirstOrDefault(match => match.Username == favorite_player.username.Trim('\0'));

            if (user == null || requestedBy == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (!user.HeartedProfileFromOthers.Any(match => match.User.UserId == requestedBy.UserId && match.IsMNR == session.IsMNR))
            {
                database.HeartedProfiles.Add(new HeartedProfile
                {
                    HeartedUser = user,
                    User = requestedBy,
                    HeartedAt = DateTime.UtcNow,
                    IsMNR = session.IsMNR
                });
                if (!session.IsMNR)
                {
                    database.ActivityLog.Add(new ActivityEvent
                    {
                        Author = requestedBy,
                        Type = ActivityType.player_event,
                        List = ActivityList.activity_log,
                        Topic = "player_hearted",
                        Description = "",
                        Player = user,
                        CreatedAt = DateTime.UtcNow,
                        AllusionId = user.UserId,
                        AllusionType = "Player"
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

        public static string RemoveFromFavorites(Database database, Guid SessionID, Models.Request.FavoritePlayer favorite_player)
        {
            var session = SessionImpl.GetSession(SessionID);
            var requestedBy = database.Users.FirstOrDefault(match => match.Username == session.Username);
            
            var user = database.Users
                .FirstOrDefault(match => match.Username == favorite_player.username.Trim('\0'));

            if (user == null || requestedBy == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var heartedUser = database.HeartedProfiles.FirstOrDefault(match => match.HeartedUser.UserId == user.UserId && match.User.UserId == requestedBy.UserId 
                && match.IsMNR == session.IsMNR);

            if (heartedUser == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.HeartedProfiles.Remove(heartedUser);
            database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        // TODO: Particular point to bug check, this was slightly confusing to convert
        public static string ListFavorites(Database database, Guid SessionID, string player_id_or_username)
        {
            var session = SessionImpl.GetSession(SessionID);
            var requestedBy = database.Users.FirstOrDefault(match => match.Username == session.Username);

            var user = database.Users
                .FirstOrDefault(match => match.Username == player_id_or_username || match.UserId.ToString() == player_id_or_username);

            if (user == null || requestedBy == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var heartedUsers = database.HeartedProfiles
                .Where(match => match.User.UserId == user.UserId && match.IsMNR == session.IsMNR)
                .OrderByDescending(match => match.HeartedAt)
                .ProjectTo<Models.Response.FavoritePlayer>(database.MapperConfig, new { requestedBy })
                .ToList();

            var resp = new Response<List<FavoritePlayers>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [new FavoritePlayers { Total = heartedUsers.Count, Players = heartedUsers }]
            };
            return resp.Serialize();
        }
    }
}

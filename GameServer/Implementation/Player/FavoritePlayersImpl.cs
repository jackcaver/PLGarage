using GameServer.Models.PlayerData;
using GameServer.Models.Response;
using GameServer.Models;
using System;
using GameServer.Utils;
using System.Linq;
using GameServer.Models.Request;
using System.Collections.Generic;
using GameServer.Implementation.Common;

namespace GameServer.Implementation.Player
{
    public class FavoritePlayersImpl
    {
        public static string AddToFavorites(Database database, Guid SessionID, FavoritePlayer favorite_player)
        {
            var session = SessionImpl.GetSession(SessionID);
            var requestedBy = database.Users.FirstOrDefault(match => match.Username == session.Username);
            if (favorite_player.username.Contains("\0"))
                favorite_player.username = favorite_player.username.Split("\0")[0];
            var user = database.Users.FirstOrDefault(match => match.Username == favorite_player.username);

            if (user == null || requestedBy == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (!user.IsHeartedByMe(requestedBy.UserId, session.IsMNR))
            {
                database.HeartedProfiles.Add(new HeartedProfile
                {
                    HeartedUserId = user.UserId,
                    UserId = requestedBy.UserId,
                    HeartedAt = DateTime.UtcNow,
                    IsMNR = session.IsMNR
                });
                if (!session.IsMNR)
                {
                    database.ActivityLog.Add(new ActivityEvent
                    {
                        AuthorId = requestedBy.UserId,
                        Type = ActivityType.player_event,
                        List = ActivityList.activity_log,
                        Topic = "player_hearted",
                        Description = "",
                        PlayerId = user.UserId,
                        PlayerCreationId = 0,
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

        public static string RemoveFromFavorites(Database database, Guid SessionID, FavoritePlayer favorite_player)
        {
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            if (favorite_player.username.Contains("\0"))
                favorite_player.username = favorite_player.username.Split("\0")[0];
            int id = database.Users.FirstOrDefault(match => match.Username == favorite_player.username).UserId;

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var HeartedUser = database.HeartedProfiles.FirstOrDefault(match => match.HeartedUserId == id && match.UserId == user.UserId 
                && match.IsMNR == session.IsMNR);

            if (HeartedUser == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.HeartedProfiles.Remove(HeartedUser);
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
            var requestedBy = database.Users.FirstOrDefault(match => match.Username == session.Username);
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

            var Players = new List<favorite_player> { };
            var HeartedUsers = database.HeartedProfiles.Where(match => match.UserId == user.UserId && match.IsMNR == session.IsMNR).ToList();

            HeartedUsers.Sort((curr, prev) => prev.HeartedAt.CompareTo(curr.HeartedAt));

            foreach (var profile in HeartedUsers)
            {
                var heartedUser = database.Users.FirstOrDefault(match => match.UserId == profile.HeartedUserId);
                if (heartedUser != null)
                {
                    Players.Add(new favorite_player
                    {
                        favorite_player_id = profile.HeartedUserId,
                        hearted_by_me = requestedBy != null && heartedUser.IsHeartedByMe(requestedBy.UserId, session.IsMNR) ? 1 : 0,
                        hearts = heartedUser.Hearts,
                        id = Players.Count + 1,
                        quote = heartedUser.Quote,
                        total_tracks = heartedUser.TotalTracks,
                        username = heartedUser.Username
                    });
                }
            }

            var resp = new Response<List<favorite_players>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [new favorite_players { total = Players.Count, Players = Players }]
            };
            return resp.Serialize();
        }
    }
}

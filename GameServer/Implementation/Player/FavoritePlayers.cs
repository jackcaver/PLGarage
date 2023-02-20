using GameServer.Models.PlayerData;
using GameServer.Models.Response;
using GameServer.Models;
using System;
using GameServer.Utils;
using System.Linq;
using GameServer.Models.Request;
using System.Collections.Generic;

namespace GameServer.Implementation.Player
{
    public class FavoritePlayers
    {
        public static string AddToFavorites(Database database, string username, FavoritePlayer favorite_player)
        {
            var requestedBy = database.Users.FirstOrDefault(match => match.Username == username);
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

            if (!user.IsHeartedByMe(requestedBy.UserId))
            {
                database.HeartedProfiles.Add(new HeartedProfile
                {
                    HeartedUserId = user.UserId,
                    UserId = requestedBy.UserId,
                    HeartedAt = DateTime.UtcNow,
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

        public static string RemoveFromFavorites(Database database, string username, FavoritePlayer favorite_player)
        {
            var user = database.Users.FirstOrDefault(match => match.Username == username);
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

            var HeartedUser = database.HeartedProfiles.FirstOrDefault(match => match.HeartedUserId == id && match.UserId == user.UserId);

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

        public static string ListFavorites(Database database, string username, string player_id_or_username)
        {
            var requestedBy = database.Users.FirstOrDefault(match => match.Username == username);
            var user = database.Users.FirstOrDefault(match => match.Username == player_id_or_username || match.UserId.ToString() == player_id_or_username);

            if (user == null || requestedBy == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var Players = new List<favorite_player> { };
            var HeartedUsers = database.HeartedProfiles.Where(match => match.UserId == user.UserId).ToList();

            HeartedUsers.Sort((curr, prev) => prev.HeartedAt.CompareTo(curr.HeartedAt));

            foreach (var profile in HeartedUsers)
            {
                var heartedUser = database.Users.FirstOrDefault(match => match.UserId == profile.HeartedUserId);
                if (heartedUser != null)
                {
                    Players.Add(new favorite_player
                    {
                        favorite_player_id = profile.HeartedUserId,
                        hearted_by_me = heartedUser.IsHeartedByMe(requestedBy.UserId) ? 1 : 0,
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
                response = new List<favorite_players> { new favorite_players { total = Players.Count, Players = Players } }
            };
            return resp.Serialize();
        }
    }
}

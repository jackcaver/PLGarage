using GameServer.Models.PlayerData;
using GameServer.Models.Response;
using GameServer.Models;
using GameServer.Utils;
using System.Linq;
using GameServer.Models.Request;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Implementation.Player
{
    public class FavoritePlayers
    {
        public static string AddToFavorites(Database database, SessionData session, FavoritePlayer favorite_player)
        {
            favorite_player.username = favorite_player.username.TrimEnd('\0');
            var requestedBy = session.User;
            var user = database.Users
                .Include(u => u.HeartedByProfiles)
                .FirstOrDefault(match => match.Username == favorite_player.username);

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
                    HeartedAt = TimeUtils.Now,
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
                        CreatedAt = TimeUtils.Now,
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

        public static string RemoveFromFavorites(Database database, SessionData session, FavoritePlayer favorite_player)
        {
            favorite_player.username = favorite_player.username.TrimEnd('\0');
            var user = session.User;

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var heartedUser = database.HeartedProfiles.FirstOrDefault(match => match.Username == favorite_player.username 
                                                                               && match.UserId == user.UserId && match.IsMNR == session.IsMNR);

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

        public static string ListFavorites(Database database, SessionData session, string player_id_or_username)
        {
            var requestedBy = session.User;
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
            
            var heartedUsers = database.HeartedProfiles
                .AsSplitQuery()
                .Include(u => u.HeartedUser.PlayerCreations)
                .Include(u => u.HeartedUser.HeartedByProfiles)
                .OrderBy(h => h.HeartedAt)
                .Where(match => match.UserId == user.UserId && match.IsMNR == session.IsMNR);
            
            var players = heartedUsers.Select(profile => new favorite_player
            {
                favorite_player_id = profile.HeartedUserId,
                hearted_by_me = requestedBy != null && profile.IsHeartedByMe(requestedBy.UserId, session.IsMNR) ? 1 : 0,
                hearts = profile.Hearts,
                id = profile.Id,
                quote = profile.Quote,
                total_tracks = profile.TotalTracks,
                username = profile.Username
            }).ToList();

            var resp = new Response<List<favorite_players>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [new favorite_players { total = heartedUsers.Count(), Players = players }]
            };
            return resp.Serialize();
        }
    }
}

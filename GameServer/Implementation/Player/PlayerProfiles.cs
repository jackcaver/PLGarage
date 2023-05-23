using GameServer.Implementation.Common;
using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Implementation.Player
{
    public class PlayerProfiles
    {
        public static string ViewProfile(Database database, int player_id, Platform platform)
        {
            var user = database.Users.FirstOrDefault(match => match.UserId == player_id);
            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var resp = new Response<List<player_profile>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_profile> { new player_profile { player_id = user.UserId, quote = user.Quote, username = user.Username } }
            };
            return resp.Serialize();
        }

        public static string UpdateProfile(Database database, Guid SessionID, PlayerProfile player_profile)
        {
            int id = -130;
            string message = "The player doesn't exist";
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            if (user != null)
            {
                id = 0;
                message = "Successful completion";
                user.Quote = player_profile.quote;
                database.SaveChanges();
            }

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = id, message = message },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static string GetPlayerID(Database database, string username)
        {
            var user = database.Users.FirstOrDefault(match => match.Username == username);
            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var resp = new Response<PlayerIDResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new PlayerIDResponse { player_id = user.UserId }
            };

            return resp.Serialize();
        }

        public static string GetPlayerInfo(Database database, int id, Guid SessionID)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.UserId == id);
            var requestedBy = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (user == null || requestedBy == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var resp = new Response<List<player>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player> {
                    new player {
                        id = user.UserId,
                        city = "",
                        state = "",
                        province = "",
                        country = "",
                        created_at = user.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        hearted_by_me = user.IsHeartedByMe(requestedBy.UserId),
                        hearts = user.Hearts,
                        longest_win_streak = user.LongestWinStreak,
                        online_disconnected = user.OnlineDisconnected,
                        online_finished = user.OnlineFinished,
                        online_finished_last_week = user.OnlineFinishedLastWeek,
                        online_finished_this_week = user.OnlineFinishedThisWeek,
                        online_forfeit = user.OnlineForfeit,
                        online_races = user.OnlineRaces,
                        online_races_last_week = user.OnlineRacesLastWeek,
                        online_races_this_week = user.OnlineRacesThisWeek,
                        online_wins = user.OnlineWins,
                        online_wins_last_week = user.OnlineWinsLastWeek,
                        online_wins_this_week = user.OnlineWinsThisWeek,
                        player_creation_quota = user.Quota,
                        points = user.Points,
                        presence = user.Presence.ToString(),
                        quote = user.Quote,
                        rank = user.Rank,
                        total_tracks = user.TotalTracks,
                        updated_at = user.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        username = user.Username,
                        win_streak = user.WinStreak
                    }
                }
            };
            return resp.Serialize();
        }
    }
}

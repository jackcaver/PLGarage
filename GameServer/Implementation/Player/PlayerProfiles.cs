using GameServer.Implementation.Common;
using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
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
                response = [new player_profile { player_id = user.UserId, quote = user.Quote, username = user.Username }]
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
                user.Quote = player_profile.quote.Replace("\0", "");
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
            var user = database.Users
                .AsSplitQuery()
                .Include(u => u.HeartedByProfiles)
                .Include(u => u.RacesStarted)
                .Include(u => u.RacesFinished)
                .Include(u => u.PlayerRatings)
                .Include(u => u.PlayerPoints)
                .Include(u => u.PlayerExperiencePoints)
                .Include(u => u.PlayerCreations)
                .Include(u => u.PlayerCreationPoints)
                .FirstOrDefault(match => match.UserId == id);
            var requestedBy = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var resp = new Response<List<PlayerProfileResponse>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [
                    new PlayerProfileResponse {
                        id = user.UserId,
                        player_id = user.UserId,
                        city = "",
                        state = "",
                        province = "",
                        country = "",
                        created_at = user.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        hearted_by_me = requestedBy != null ? user.IsHeartedByMe(requestedBy.UserId, session.IsMNR) : false,
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
                        quote = user.Quote != null ? user.Quote.Replace("\0", "") : "",
                        rank = user.Rank,
                        updated_at = user.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        username = user.Username,
                        win_streak = user.WinStreak,
                        //MNR
                        total_characters = user.TotalCharacters(session.Platform),
                        total_karts = user.TotalKarts(session.Platform),
                        total_player_creations = user.TotalPlayerCreations(session.Platform),
                        total_tracks = (session != null && session.IsMNR) ? user.TotalMNRTracks(session.Platform) : user.TotalTracks,
                        skill_level = user.SkillLevelName(session.Platform),
                        skill_level_id = user.SkillLevelId(session.Platform),
                        skill_level_name = user.SkillLevelName(session.Platform),
                        rating = user.Rating.ToString("0.00", CultureInfo.InvariantCulture),
                        star_rating = user.StarRating,
                        creator_points = user.CreatorPoints(session.Platform),
                        creator_points_last_week = user.CreatorPointsLastWeek(session.Platform), 
                        creator_points_this_week = user.CreatorPointsThisWeek(session.Platform),
                        experience_points = user.ExperiencePoints,
                        experience_points_last_week = user.ExperiencePointsLastWeek,
                        experience_points_this_week = user.ExperiencePointsThisWeek,
                        longest_drift = user.LongestDrift,
                        longest_hang_time = user.LongestHangTime.ToString()
                    }
                ]
            };
            return resp.Serialize();
        }

        public static string GetSkillLevel(Database database, Guid SessionID, int[] id)
        {
            var session = Session.GetSession(SessionID);
            var users = database.Users
                    .AsSplitQuery()
                    .Include(u => u.PlayerCreationPoints)
                    .Include(u => u.PlayerExperiencePoints)
                    .Where(match => id.Contains(match.UserId)).ToList();

            if (users.Count == 0)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var skillLevelPlayers = new List<SkillLevelPlayer>();

            foreach (var user in users)
            {
                skillLevelPlayers.Add(new SkillLevelPlayer {
                    id = user.UserId,
                    username = user.Username,
                    skill_level_id = user.SkillLevelId(session.Platform),
                    skill_level_name = user.SkillLevelName(session.Platform)
                });
            }

            var resp = new Response<List<SkillLevelResponse>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response =
                [
                    new SkillLevelResponse {
                        total = users.Count,
                        playersList = skillLevelPlayers
                    }
                ]
            };

            return resp.Serialize();
        }

        public static string IncrementRaceXP(Database database, Guid SessionID, int delta)
        {
            int id = -130;
            string message = "The player doesn't exist";
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (user != null)
            {
                id = 0;
                message = "Successful completion";
                database.PlayerExperiencePoints.Add(new PlayerExperiencePoint
                {
                    CreatedAt = TimeUtils.Now,
                    PlayerId = user.UserId,
                    Amount = delta
                });
                database.SaveChanges();
            }

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = id, message = message },
                response = new EmptyResponse()
            };
            return resp.Serialize();
        }
    }
}

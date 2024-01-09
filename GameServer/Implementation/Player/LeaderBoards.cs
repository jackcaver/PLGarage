using GameServer.Implementation.Common;
using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.Games;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Implementation.Player
{
    public class LeaderBoards
    {
        public static string ViewSubLeaderBoard(Database database, Guid SessionID, int sub_group_id, int sub_key_id, LeaderboardType type, Platform platform,
            int page, int per_page, int column_page, int cols_per_page, SortColumn sort_column, SortOrder sort_order, int? num_above_below, int limit, int playgroup_size,
            float? latitude, float? longitude, string usernameFilter = null, bool FriendsView = false)
        {
            var scores = new List<Score> { };
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            UserGeneratedContentUtils.AddStoryLevel(database, sub_key_id);

            if (usernameFilter == null)
                scores = database.Scores.Where(match => match.SubKeyId == sub_key_id && match.SubGroupId == sub_group_id && match.PlaygroupSize == playgroup_size && match.IsMNR == session.IsMNR && match.Platform == platform).ToList();

            if (usernameFilter != null)
            {
                foreach (string name in usernameFilter.Split(','))
                {
                    var friend = database.Users.FirstOrDefault(match => match.Username == name);
                    if (friend != null)
                    {
                        var score = database.Scores.FirstOrDefault(match => match.PlayerId == friend.UserId && match.SubKeyId == sub_key_id
                            && match.SubGroupId == sub_group_id && match.PlaygroupSize == playgroup_size && match.IsMNR == session.IsMNR && match.Platform == platform);
                        if (score != null)
                            scores.Add(score);
                    }
                }
            }

            if (latitude != null && longitude != null)
                scores.RemoveAll(match => !(match.Latitude >= latitude-0.24 && match.Latitude <= latitude+0.24 
                    && match.Longitude >= longitude-0.24 && match.Longitude <= longitude+0.24));

            if (sort_column == SortColumn.finish_time)
                scores.Sort((curr, prev) => curr.FinishTime.CompareTo(prev.FinishTime));
            if (sort_column == SortColumn.score)
                scores.Sort((curr, prev) => prev.Points.CompareTo(curr.Points));
            if (sort_column == SortColumn.best_lap_time)
                scores.Sort((curr, prev) => curr.BestLapTime.CompareTo(prev.BestLapTime));

            var MyStats = user != null ? database.Scores.FirstOrDefault(match => match.PlayerId == user.UserId
                && match.SubKeyId == sub_key_id && match.SubGroupId == sub_group_id && match.PlaygroupSize == playgroup_size && match.IsMNR == session.IsMNR && match.Platform == platform) : null;
            var mystats = new SubLeaderboardPlayer { };

            if (MyStats != null)
            {
                mystats.created_at = MyStats.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz");
                mystats.finish_time = MyStats.FinishTime;
                mystats.id = MyStats.Id;
                mystats.platform = MyStats.Platform.ToString();
                mystats.player_id = MyStats.PlayerId;
                mystats.playgroup_size = MyStats.PlaygroupSize;
                mystats.rank = MyStats.GetRank(sort_column);
                mystats.score = MyStats.Points;
                mystats.sub_group_id = MyStats.SubGroupId;
                mystats.sub_key_id = MyStats.SubKeyId;
                mystats.updated_at = MyStats.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz");
                mystats.username = MyStats.Username;
                //MNR
                mystats.best_lap_time = MyStats.BestLapTime;
                mystats.character_idx = MyStats.CharacterIdx;
                mystats.ghost_car_data_md5 = MyStats.GhostCarDataMD5;
                mystats.kart_idx = MyStats.KartIdx;
                mystats.skill_level_id = user.SkillLevelId(platform);
                mystats.skill_level_name = user.SkillLevelName(platform);
            }

            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, scores.Count);

            if (pageEnd > scores.Count)
                pageEnd = scores.Count;

            var leaderboardPlayers = new List<SubLeaderboardPlayer> { };

            for (int i = pageStart; i < pageEnd; i++)
            {
                var score = scores[i];
                if (score != null)
                {
                    leaderboardPlayers.Add(new SubLeaderboardPlayer
                    {
                        id = score.Id,
                        created_at = score.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        finish_time = score.FinishTime,
                        platform = score.Platform.ToString(),
                        player_id = score.PlayerId,
                        username = score.Username,
                        playgroup_size = score.PlaygroupSize,
                        rank = score.GetRank(sort_column),
                        score = score.Points,
                        sub_group_id = score.SubGroupId,
                        sub_key_id = score.SubKeyId,
                        updated_at = score.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        //MNR
                        best_lap_time = score.BestLapTime,
                        character_idx = score.CharacterIdx,
                        ghost_car_data_md5 = score.GhostCarDataMD5,
                        kart_idx = score.KartIdx,
                        skill_level_id = score.User.SkillLevelId(platform),
                        skill_level_name = score.User.SkillLevelName(platform)
                    });
                }
            }

            if (FriendsView)
            {
                var friendsViewResp = new Response<SubLeaderboardFriendsViewResponse>
                {
                    status = new ResponseStatus { id = 0, message = "Successful completion" },
                    response = new SubLeaderboardFriendsViewResponse
                    {
                        my_stats = mystats,
                        friends_leaderboard = new SubLeaderboard
                        {
                            page = page,
                            playgroup_size = playgroup_size,
                            row_end = pageEnd,
                            row_start = pageStart,
                            sub_group_id = sub_group_id,
                            sub_key_id = sub_key_id,
                            total = scores.Count,
                            total_pages = totalPages,
                            type = type.ToString(),
                            LeaderboardPlayersList = leaderboardPlayers
                        }
                    }
                };
                return friendsViewResp.Serialize();
            }

            var resp = new Response<SubLeaderboardViewResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new SubLeaderboardViewResponse
                {
                    my_stats = mystats,
                    leaderboard = new SubLeaderboard
                    {
                        page = page,
                        playgroup_size = playgroup_size,
                        row_end = pageEnd,
                        row_start = pageStart,
                        sub_group_id = sub_group_id,
                        sub_key_id = sub_key_id,
                        total = scores.Count,
                        total_pages = totalPages,
                        type = type.ToString(),
                        LeaderboardPlayersList = leaderboardPlayers
                    }
                }
            };
            return resp.Serialize();
        }

        public static string ViewSubLeaderBoardAroundMe(Database database, Guid SessionID, int sub_group_id, int sub_key_id, LeaderboardType type, Platform platform,
            int column_page, int cols_per_page, SortColumn sort_column, SortOrder sort_order, int num_above_below, int playgroup_size, int limit)
        {
            var scores = database.Scores.Where(match => match.SubKeyId == sub_key_id && match.SubGroupId == sub_group_id
                && match.PlaygroupSize == playgroup_size && match.Platform == platform).ToList();
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (sort_column == SortColumn.finish_time)
                scores.Sort((curr, prev) => curr.FinishTime.CompareTo(prev.FinishTime));
            if (sort_column == SortColumn.score)
                scores.Sort((curr, prev) => prev.Points.CompareTo(curr.Points));

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            int MyStatsIndex = scores.FindIndex(match => match.PlayerId == user.UserId);

            var leaderboardPlayers = new List<SubLeaderboardPlayer> { };
            int maxIndex = MyStatsIndex + num_above_below,
                minIndex = MyStatsIndex - num_above_below;

            if (maxIndex > scores.Count)
                maxIndex = scores.Count;

            if (minIndex < 0)
                minIndex = 0;

            for (int i = minIndex; i < maxIndex; i++)
            {
                var score = scores[i];
                if (score != null)
                {
                    leaderboardPlayers.Add(new SubLeaderboardPlayer
                    {
                        id = score.Id,
                        created_at = score.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        finish_time = score.FinishTime,
                        platform = score.Platform.ToString(),
                        player_id = score.PlayerId,
                        username = score.Username,
                        playgroup_size = score.PlaygroupSize,
                        rank = score.GetRank(sort_column),
                        score = score.Points,
                        sub_group_id = score.SubGroupId,
                        sub_key_id = score.SubKeyId,
                        updated_at = score.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")
                    });
                }
            }

            var resp = new Response<List<SubLeaderboard>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<SubLeaderboard> { new SubLeaderboard {
                    playgroup_size = playgroup_size,
                    sub_group_id = sub_group_id,
                    sub_key_id = sub_key_id,
                    total = leaderboardPlayers.Count,
                    type = type.ToString(),
                    LeaderboardPlayersList = leaderboardPlayers
                }}
            };
            return resp.Serialize();
        }

        public static string ViewLeaderBoard(Database database, Guid SessionID, LeaderboardType type, GameType game_type, Platform platform, int page, 
            int per_page, int column_page, int cols_per_page, SortColumn sort_column, SortOrder sort_order, int limit, 
            string usernameFilter = null, bool FriendsView = false)
        {
            var session = Session.GetSession(SessionID);
            var requestedBy = database.Users.FirstOrDefault(match => match.Username == session.Username);
            List<User> users = database.Users.Where(match => match.Username != "ufg" && match.PlayedMNR).ToList();
            List<Score> scores = database.Scores.Where(match => match.IsMNR && match.SubGroupId == (int)game_type-10).ToList();

            if (usernameFilter != null)
            {
                var usernames = usernameFilter.Split(',');
                var unfilteredUsers = new List<User>(users);
                foreach (var user in unfilteredUsers)
                {
                    if (!usernames.Contains(user.Username))
                    {
                        scores.RemoveAll(match => match.Username == user.Username);
                        users.Remove(user);
                    }
                }
            }

            foreach (var score in scores)
            {
                if (FriendsView)Log.Information(score.Username);
            }

            int Total = 0;

            //creator points
            if (game_type == GameType.OVERALL_CREATORS && type == LeaderboardType.LIFETIME)
                users.Sort((curr, prev) => prev.CreatorPoints(platform).CompareTo(curr.CreatorPoints(platform)));
            if (game_type == GameType.OVERALL_CREATORS && type == LeaderboardType.WEEKLY)
                users.Sort((curr, prev) => prev.CreatorPointsThisWeek(platform).CompareTo(curr.CreatorPointsThisWeek(platform)));
            if (game_type == GameType.OVERALL_CREATORS && type == LeaderboardType.LAST_WEEK)
                users.Sort((curr, prev) => prev.CreatorPointsLastWeek(platform).CompareTo(curr.CreatorPointsLastWeek(platform)));

            //creator points for characters
            if (game_type == GameType.CHARACTER_CREATORS && type == LeaderboardType.LIFETIME)
                users.Sort((curr, prev) => prev.CreatorPoints(platform, PlayerCreationType.CHARACTER).CompareTo(curr.CreatorPoints(platform, PlayerCreationType.CHARACTER)));
            if (game_type == GameType.CHARACTER_CREATORS && type == LeaderboardType.WEEKLY)
                users.Sort((curr, prev) => prev.CreatorPointsThisWeek(platform, PlayerCreationType.CHARACTER).CompareTo(curr.CreatorPointsThisWeek(platform, PlayerCreationType.CHARACTER)));
            if (game_type == GameType.CHARACTER_CREATORS && type == LeaderboardType.LAST_WEEK)
                users.Sort((curr, prev) =>  prev.CreatorPointsLastWeek(platform, PlayerCreationType.CHARACTER).CompareTo(curr.CreatorPointsLastWeek(platform, PlayerCreationType.CHARACTER)));

            //creator points for karts
            if (game_type == GameType.KART_CREATORS && type == LeaderboardType.LIFETIME)
                users.Sort((curr, prev) => prev.CreatorPoints(platform, PlayerCreationType.KART).CompareTo(curr.CreatorPoints(platform, PlayerCreationType.KART)));
            if (game_type == GameType.KART_CREATORS && type == LeaderboardType.WEEKLY)
                users.Sort((curr, prev) => prev.CreatorPointsThisWeek(platform, PlayerCreationType.KART).CompareTo(curr.CreatorPointsThisWeek(platform, PlayerCreationType.KART)));
            if (game_type == GameType.KART_CREATORS && type == LeaderboardType.LAST_WEEK)
                users.Sort((curr, prev) => prev.CreatorPointsLastWeek(platform, PlayerCreationType.KART).CompareTo(curr.CreatorPointsLastWeek(platform, PlayerCreationType.KART)));

            //creator points for tracks
            if (game_type == GameType.TRACK_CREATORS && type == LeaderboardType.LIFETIME)
                users.Sort((curr, prev) => prev.CreatorPoints(platform, PlayerCreationType.TRACK).CompareTo(curr.CreatorPoints(platform, PlayerCreationType.TRACK)));
            if (game_type == GameType.TRACK_CREATORS && type == LeaderboardType.WEEKLY)
                users.Sort((curr, prev) => prev.CreatorPointsThisWeek(platform, PlayerCreationType.TRACK).CompareTo(curr.CreatorPointsThisWeek(platform, PlayerCreationType.TRACK)));
            if (game_type == GameType.TRACK_CREATORS && type == LeaderboardType.LAST_WEEK)
                users.Sort((curr, prev) => prev.CreatorPointsLastWeek(platform, PlayerCreationType.TRACK).CompareTo(curr.CreatorPointsLastWeek(platform, PlayerCreationType.TRACK)));

            //Experience points
            if (game_type == GameType.OVERALL && type == LeaderboardType.LIFETIME)
                users.Sort((curr, prev) => prev.TotalXP(platform).CompareTo(curr.TotalXP(platform)));
            if (game_type == GameType.OVERALL && type == LeaderboardType.WEEKLY)
                users.Sort((curr, prev) => prev.TotalXPThisWeek(platform).CompareTo(curr.TotalXPThisWeek(platform)));
            if (game_type == GameType.OVERALL && type == LeaderboardType.LAST_WEEK)
                users.Sort((curr, prev) => prev.TotalXPLastWeek(platform).CompareTo(curr.TotalXPLastWeek(platform)));

            if (game_type == GameType.OVERALL_RACE && sort_column == SortColumn.experience_points)
            {
                switch (sort_column)
                {
                    case SortColumn.experience_points:
                        if (type == LeaderboardType.LIFETIME)
                            users.Sort((curr, prev) => prev.ExperiencePoints.CompareTo(curr.ExperiencePoints));
                        if (type == LeaderboardType.WEEKLY)
                            users.Sort((curr, prev) => prev.ExperiencePointsThisWeek.CompareTo(curr.ExperiencePointsThisWeek));
                        if (type == LeaderboardType.LAST_WEEK)
                            users.Sort((curr, prev) => prev.ExperiencePointsLastWeek.CompareTo(curr.ExperiencePointsLastWeek));
                        break;

                    case SortColumn.online_races:
                        users.Sort((curr, prev) => prev.OnlineRaces.CompareTo(curr.OnlineRaces));
                        break;

                    case SortColumn.online_wins:
                        users.Sort((curr, prev) => prev.OnlineWins.CompareTo(curr.OnlineWins));
                        break;

                    case SortColumn.longest_win_streak:
                        users.Sort((curr, prev) => prev.LongestWinStreak.CompareTo(curr.LongestWinStreak));
                        break;

                    case SortColumn.win_streak:
                        users.Sort((curr, prev) => prev.WinStreak.CompareTo(curr.WinStreak));
                        break;

                    case SortColumn.longest_hang_time:
                        users.Sort((curr, prev) => prev.LongestHangTime.CompareTo(curr.LongestHangTime));
                        break;

                    case SortColumn.longest_drift:
                        users.Sort((curr, prev) => prev.LongestDrift.CompareTo(curr.LongestDrift));
                        break;

                    default:
                        break;
                }
            }

            if (sort_column == SortColumn.finish_time)
                scores.Sort((curr, prev) => curr.FinishTime.CompareTo(prev.FinishTime));
            if (sort_column == SortColumn.best_lap_time)
                scores.Sort((curr, prev) => curr.BestLapTime.CompareTo(prev.BestLapTime));

            if (game_type == GameType.OVERALL_CREATORS || game_type == GameType.CHARACTER_CREATORS 
                || game_type == GameType.TRACK_CREATORS || game_type == GameType.KART_CREATORS 
                || game_type == GameType.OVERALL || game_type == GameType.OVERALL_RACE) Total = users.Count();

            if (game_type == GameType.ONLINE_HOT_SEAT_RACE) Total = scores.Count();

            var MyStats = requestedBy != null ? scores.FirstOrDefault(match => match.PlayerId == requestedBy.UserId) : null;
            var mystats = new LeaderboardPlayer { };

            if (MyStats != null || ((game_type == GameType.OVERALL_CREATORS || game_type == GameType.CHARACTER_CREATORS
                || game_type == GameType.TRACK_CREATORS || game_type == GameType.KART_CREATORS
                || game_type == GameType.OVERALL || game_type == GameType.OVERALL_RACE) && requestedBy != null))
            {
                if (game_type == GameType.OVERALL_CREATORS || game_type == GameType.CHARACTER_CREATORS
                    || game_type == GameType.TRACK_CREATORS || game_type == GameType.KART_CREATORS || game_type == GameType.OVERALL)
                {
                    mystats.created_at = requestedBy.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz");
                    mystats.id = requestedBy.UserId;
                    mystats.player_id = requestedBy.UserId;
                    mystats.updated_at = requestedBy.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz");
                    mystats.username = requestedBy.Username;
                    mystats.rank = requestedBy.GetRank(game_type, type, platform, sort_column);
                    mystats.skill_level_id = requestedBy.SkillLevelId(platform);
                    mystats.skill_level_name = requestedBy.SkillLevelName(platform);

                    switch(game_type)
                    {
                        case GameType.OVERALL:
                            if (type == LeaderboardType.LIFETIME)
                                mystats.points = requestedBy.TotalXP(platform);
                            if (type == LeaderboardType.WEEKLY)
                                mystats.points = requestedBy.TotalXPThisWeek(platform);
                            if (type == LeaderboardType.LAST_WEEK)
                                mystats.points = requestedBy.TotalXPLastWeek(platform);
                            break;

                        case GameType.OVERALL_CREATORS:
                            if (type == LeaderboardType.LIFETIME)
                                mystats.points = requestedBy.CreatorPoints(platform);
                            if (type == LeaderboardType.WEEKLY)
                                mystats.points = requestedBy.CreatorPointsThisWeek(platform);
                            if (type == LeaderboardType.LAST_WEEK)
                                mystats.points = requestedBy.CreatorPointsLastWeek(platform);
                            break;

                        case GameType.KART_CREATORS:
                            if (type == LeaderboardType.LIFETIME)
                                mystats.points = requestedBy.CreatorPoints(platform, PlayerCreationType.KART);
                            if (type == LeaderboardType.WEEKLY)
                                mystats.points = requestedBy.CreatorPointsThisWeek(platform, PlayerCreationType.KART);
                            if (type == LeaderboardType.LAST_WEEK)
                                mystats.points = requestedBy.CreatorPointsLastWeek(platform, PlayerCreationType.KART);
                            break;

                        case GameType.TRACK_CREATORS:
                            if (type == LeaderboardType.LIFETIME)
                                mystats.points = requestedBy.CreatorPoints(platform, PlayerCreationType.TRACK);
                            if (type == LeaderboardType.WEEKLY)
                                mystats.points = requestedBy.CreatorPointsThisWeek(platform, PlayerCreationType.TRACK);
                            if (type == LeaderboardType.LAST_WEEK)
                                mystats.points = requestedBy.CreatorPointsLastWeek(platform, PlayerCreationType.TRACK);
                            break;

                        case GameType.CHARACTER_CREATORS:
                            if (type == LeaderboardType.LIFETIME)
                                mystats.points = requestedBy.CreatorPoints(platform, PlayerCreationType.CHARACTER);
                            if (type == LeaderboardType.WEEKLY)
                                mystats.points = requestedBy.CreatorPointsThisWeek(platform, PlayerCreationType.CHARACTER);
                            if (type == LeaderboardType.LAST_WEEK)
                                mystats.points = requestedBy.CreatorPointsLastWeek(platform, PlayerCreationType.CHARACTER);
                            break;
                    }
                }
                if (game_type == GameType.OVERALL_RACE)
                {
                    mystats.created_at = requestedBy.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz");
                    mystats.experience_points = requestedBy.ExperiencePoints;
                    mystats.id = requestedBy.UserId;
                    mystats.longest_drift = requestedBy.LongestDrift;
                    mystats.longest_hang_time = requestedBy.LongestHangTime;
                    mystats.longest_win_streak = requestedBy.LongestWinStreak;
                    mystats.online_disconnected = requestedBy.OnlineDisconnected;
                    mystats.online_finished = requestedBy.OnlineFinished;
                    mystats.online_forfeit = requestedBy.OnlineForfeit;
                    mystats.online_quits = requestedBy.OnlineQuits;
                    mystats.online_races = requestedBy.OnlineRaces;
                    mystats.online_wins = requestedBy.OnlineWins;
                    mystats.player_id = requestedBy.UserId;
                    mystats.points = requestedBy.Points;
                    mystats.updated_at = requestedBy.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz");
                    mystats.username = requestedBy.Username;
                    mystats.win_streak = requestedBy.WinStreak;
                    mystats.rank = requestedBy.GetRank(game_type, type, platform, sort_column);
                    mystats.skill_level_id = requestedBy.SkillLevelId(platform);
                    mystats.skill_level_name = requestedBy.SkillLevelName(platform);
                    mystats.character_idx = requestedBy.CharacterIdx;
                    mystats.kart_idx = requestedBy.KartIdx;
                }
                if (game_type == GameType.ONLINE_HOT_SEAT_RACE)
                {
                    mystats.best_lap_time = MyStats.BestLapTime;
                    mystats.character_idx = MyStats.CharacterIdx;
                    mystats.created_at = MyStats.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz");
                    mystats.ghost_car_data_md5 = MyStats.GhostCarDataMD5;
                    mystats.id = MyStats.Id;
                    mystats.kart_idx = MyStats.KartIdx;
                    mystats.player_id = MyStats.PlayerId;
                    mystats.points = MyStats.Points;
                    mystats.track_idx = MyStats.SubKeyId;
                    mystats.updated_at = MyStats.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz");
                    mystats.username = MyStats.Username;
                    mystats.rank = MyStats.GetRank(sort_column);
                    mystats.skill_level_id = requestedBy.SkillLevelId(platform);
                    mystats.skill_level_name = requestedBy.SkillLevelName(platform);
                }
            }

            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, Total);

            if (pageEnd > Total)
                pageEnd = Total;

            var leaderboardPlayers = new List<LeaderboardPlayer>();
            for (int i = pageStart; i < pageEnd; i++)
            {
                if (game_type == GameType.OVERALL_CREATORS || game_type == GameType.CHARACTER_CREATORS 
                    || game_type == GameType.TRACK_CREATORS || game_type == GameType.KART_CREATORS || game_type == GameType.OVERALL)
                {
                    var user = users[i];
                    if (user != null)
                    {
                        float points = 0;
                        switch (game_type)
                        {
                            case GameType.OVERALL:
                                if (type == LeaderboardType.LIFETIME)
                                    points = user.TotalXP(platform);
                                if (type == LeaderboardType.WEEKLY)
                                    points = user.TotalXPThisWeek(platform);
                                if (type == LeaderboardType.LAST_WEEK)
                                    points = user.TotalXPLastWeek(platform);
                                break;

                            case GameType.OVERALL_CREATORS:
                                if (type == LeaderboardType.LIFETIME)
                                    points = user.CreatorPoints(platform);
                                if (type == LeaderboardType.WEEKLY)
                                    points = user.CreatorPointsThisWeek(platform);
                                if (type == LeaderboardType.LAST_WEEK)
                                    points = user.CreatorPointsLastWeek(platform);
                                break;

                            case GameType.KART_CREATORS:
                                if (type == LeaderboardType.LIFETIME)
                                    points = user.CreatorPoints(platform, PlayerCreationType.KART);
                                if (type == LeaderboardType.WEEKLY)
                                    points = user.CreatorPointsThisWeek(platform, PlayerCreationType.KART);
                                if (type == LeaderboardType.LAST_WEEK)
                                    points = user.CreatorPointsLastWeek(platform, PlayerCreationType.KART);
                                break;

                            case GameType.TRACK_CREATORS:
                                if (type == LeaderboardType.LIFETIME)
                                    mystats.points = user.CreatorPoints(platform, PlayerCreationType.TRACK);
                                if (type == LeaderboardType.WEEKLY)
                                    mystats.points = user.CreatorPointsThisWeek(platform, PlayerCreationType.TRACK);
                                if (type == LeaderboardType.LAST_WEEK)
                                    points = user.CreatorPointsLastWeek(platform, PlayerCreationType.TRACK);
                                break;

                            case GameType.CHARACTER_CREATORS:
                                if (type == LeaderboardType.LIFETIME)
                                    points = user.CreatorPoints(platform, PlayerCreationType.CHARACTER);
                                if (type == LeaderboardType.WEEKLY)
                                    points = user.CreatorPointsThisWeek(platform, PlayerCreationType.CHARACTER);
                                if (type == LeaderboardType.LAST_WEEK)
                                    points = user.CreatorPointsLastWeek(platform, PlayerCreationType.CHARACTER);
                                break;
                        }

                        leaderboardPlayers.Add(new LeaderboardPlayer
                        {
                            created_at = user.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                            id = user.UserId,
                            player_id = user.UserId,
                            points = points,
                            updated_at = user.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                            username = user.Username,
                            rank = user.GetRank(game_type, type, platform, sort_column),
                            skill_level_id = user.SkillLevelId(platform),
                            skill_level_name = user.SkillLevelName(platform)
                        });
                    }
                }
                if (game_type == GameType.OVERALL_RACE)
                {
                    var user = users[i];
                    if (user != null)
                    {
                        leaderboardPlayers.Add(new LeaderboardPlayer
                        {
                            created_at = user.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                            experience_points = user.ExperiencePoints,
                            id = user.UserId,
                            longest_drift = user.LongestDrift,
                            longest_hang_time = user.LongestHangTime,
                            longest_win_streak = user.LongestWinStreak,
                            online_disconnected = user.OnlineDisconnected,
                            online_finished = user.OnlineFinished,
                            online_forfeit = user.OnlineForfeit,
                            online_quits = user.OnlineQuits,
                            online_races = user.OnlineRaces,
                            online_wins = user.OnlineWins,
                            player_id = user.UserId,
                            points = user.Points,
                            updated_at = user.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                            username = user.Username,
                            win_streak = user.WinStreak,
                            rank = user.GetRank(game_type, type, platform, sort_column),
                            skill_level_id = user.SkillLevelId(platform),
                            skill_level_name = user.SkillLevelName(platform),
                            character_idx = user.CharacterIdx,
                            kart_idx = user.KartIdx
                        });
                    }
                }
                if (game_type == GameType.ONLINE_HOT_SEAT_RACE)
                {
                    var score = scores[i];
                    if (score != null)
                    {
                        leaderboardPlayers.Add(new LeaderboardPlayer
                        {
                            best_lap_time = score.BestLapTime,
                            character_idx = score.CharacterIdx,
                            created_at = score.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                            ghost_car_data_md5 = score.GhostCarDataMD5,
                            id = score.Id,
                            kart_idx = score.KartIdx,
                            player_id = score.PlayerId,
                            points = score.Points,
                            track_idx = score.SubKeyId,
                            updated_at = score.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                            username = score.Username,
                            rank = score.GetRank(sort_column),
                            skill_level_id = score.User.SkillLevelId(platform),
                            skill_level_name = score.User.SkillLevelName(platform)
                        });
                    }
                }
            }

            var leaderboardColumns = new LeaderboardColumns();

            if (game_type == GameType.OVERALL_RACE)
            {
                leaderboardColumns.Columns = new List<LeaderboardColumn> {
                    new LeaderboardColumn
                    {
                        name = "online_races",
                        display_name = "Online Races Started"
                    },
                    new LeaderboardColumn
                    {
                        name = "online_finished",
                        display_name = "Online Races Finished"
                    },
                    new LeaderboardColumn
                    {
                        name = "online_wins",
                        display_name = "Online Wins"
                    },
                    new LeaderboardColumn
                    {
                        name = "online_forfeit",
                        display_name = "Online Races DNF"
                    },
                    new LeaderboardColumn
                    {
                        name = "online_disconnected",
                        display_name = "Online Disconnects"
                    },
                    new LeaderboardColumn
                    {
                        name = "online_quits",
                        display_name = "Online Quits"
                    },
                    new LeaderboardColumn
                    {
                        name = "win_streak",
                        display_name = "Current Win Streak"
                    },
                    new LeaderboardColumn
                    {
                        name = "longest_win_streak",
                        display_name = "Longest Win Streak"
                    },
                    new LeaderboardColumn
                    {
                        name = "longest_drift",
                        display_name = "Longest Drift"
                    }
                };
            }

            if (game_type == GameType.ONLINE_HOT_SEAT_RACE)
            {
                leaderboardColumns.Columns = new List<LeaderboardColumn> {
                    new LeaderboardColumn
                    {
                        name = "best_lap_time",
                        display_name = "Best Lap Time"
                    },
                    new LeaderboardColumn
                    {
                        name = "character_idx",
                        display_name = "Character ID"
                    },
                    new LeaderboardColumn
                    {
                        name = "kart_idx",
                        display_name = "Kart ID"
                    },
                    new LeaderboardColumn
                    {
                        name = "track_idx",
                        display_name = "Track ID"
                    },
                    new LeaderboardColumn
                    {
                        name = "ghost_car_data_md5",
                        display_name = "Ghost Car MD5"
                    }
                };
            }

            if (FriendsView)
            {
                var friendsViewResp = new Response<LeaderboardFriendsViewResponse>
                {
                    status = new ResponseStatus { id = 0, message = "Successful completion" },
                    response = new LeaderboardFriendsViewResponse
                    {
                        my_stats = mystats,
                        friends_leaderboard = new Leaderboard
                        {
                            total = Total,
                            total_pages = totalPages,
                            row_start = pageStart,
                            row_end = pageEnd,
                            page = page,
                            game_type = game_type.ToString(),
                            type = type.ToString(),
                            LeaderboardPlayersList = leaderboardPlayers,
                        },
                        leaderboard_columns = leaderboardColumns
                    }
                };
                return friendsViewResp.Serialize();
            }

            var resp = new Response<LeaderboardViewResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new LeaderboardViewResponse
                {
                    my_stats = mystats,
                    leaderboard = new Leaderboard
                    {
                        total = Total,
                        total_pages = totalPages,
                        row_start = pageStart,
                        row_end = pageEnd,
                        page = page,
                        game_type = game_type.ToString(),
                        type = type.ToString(),
                        LeaderboardPlayersList = leaderboardPlayers,
                    },
                    leaderboard_columns = leaderboardColumns
                }
            };
            return resp.Serialize();
        }
    }
}

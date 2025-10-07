using GameServer.Implementation.Common;
using GameServer.Models;
using GameServer.Models.GameBrowser;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.EntityFrameworkCore;
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
            var session = Session.GetSession(SessionID);
            var scoresQuery = database.Scores
                .AsSplitQuery()
                .Include(s => s.User)
                .ThenInclude(u => u.PlayerExperiencePoints)
                .Include(s => s.User)
                .ThenInclude(u => u.PlayerCreationPoints)
                .Where(match => match.SubKeyId == sub_key_id && match.SubGroupId == sub_group_id 
                    && match.PlaygroupSize == playgroup_size && match.IsMNR == session.IsMNR 
                    && match.Platform == platform);
            var user = database.Users
                .Include(u => u.PlayerExperiencePoints)
                .Include(u => u.PlayerCreationPoints)
                .FirstOrDefault(match => match.Username == session.Username);

            UserGeneratedContentUtils.AddStoryLevel(database, sub_key_id);

            if (usernameFilter != null)
            {
                var usernames = usernameFilter.Split(',');
                scoresQuery = scoresQuery.Where(s => usernames.Contains(s.User.Username));
            }
            
            if (latitude != null && longitude != null)
                scoresQuery = scoresQuery.Where(match => match.Latitude >= latitude-0.24 
                    && match.Latitude <= latitude+0.24 
                    && match.Longitude >= longitude-0.24 && match.Longitude <= longitude+0.24);

            if (sort_column == SortColumn.finish_time)
                scoresQuery = scoresQuery.OrderBy(s => s.FinishTime);
            if (sort_column == SortColumn.score)
                scoresQuery = scoresQuery.OrderByDescending(s => s.Points);
            if (sort_column == SortColumn.best_lap_time)
                scoresQuery = scoresQuery.OrderBy(s => s.BestLapTime);

            if (type == LeaderboardType.WEEKLY)
                scoresQuery = scoresQuery.Where(match => match.UpdatedAt >= TimeUtils.ThisWeekStart);
            if (type == LeaderboardType.LAST_WEEK)
                scoresQuery = scoresQuery.Where(match => match.UpdatedAt >= TimeUtils.LastWeekStart && match.UpdatedAt < TimeUtils.ThisWeekStart);

            Score MyStats = null;

            if (user != null)
                MyStats = scoresQuery.FirstOrDefault(match => match.PlayerId == user.UserId);

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

            if (platform == Platform.PSV)
            {
                List<int> players = [];
                var duplicateFilter = scoresQuery;
                foreach (var score in scoresQuery)
                {
                    if (!players.Contains(score.PlayerId))
                        players.Add(score.PlayerId);
                }
                foreach (var player in players)
                {
                    var best = scoresQuery.FirstOrDefault(match => match.PlayerId == player);
                    duplicateFilter = scoresQuery.Where(s => s.Id == best.Id);
                }
                scoresQuery = duplicateFilter;
            }

            var total = scoresQuery.Count();

            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, total);

            if (pageEnd > total)
                pageEnd = total;

            var leaderboardPlayers = new List<SubLeaderboardPlayer> { };

            var scores = scoresQuery.Skip(pageStart).Take(per_page).ToList();

            foreach (var score in scores)
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
                            total = total,
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
                        total = total,
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
            var session = Session.GetSession(SessionID);
            var scoresQuery = database.Scores
                .AsSplitQuery()
                .Include(s => s.User)
                .ThenInclude(u => u.PlayerExperiencePoints)
                .Include(s => s.User)
                .ThenInclude(u => u.PlayerCreationPoints)
                .Where(match => match.SubKeyId == sub_key_id 
                    && match.SubGroupId == sub_group_id && match.PlaygroupSize == playgroup_size 
                    && match.Platform == platform && match.IsMNR == session.IsMNR);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (sort_column == SortColumn.finish_time)
                scoresQuery = scoresQuery.OrderBy(s => s.FinishTime);
            if (sort_column == SortColumn.score)
                scoresQuery = scoresQuery.OrderByDescending(s => s.Points);
            if (sort_column == SortColumn.best_lap_time)
                scoresQuery = scoresQuery.OrderBy(s => s.BestLapTime);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var MyStatsIndex = scoresQuery.Select(s => s.PlayerId).ToList().FindIndex(match => match == user.UserId);

            var leaderboardPlayers = new List<SubLeaderboardPlayer> { };
            int minIndex = MyStatsIndex - num_above_below;

            var total = scoresQuery.Count();

            if (minIndex < 0)
                minIndex = 0;

            var scores = scoresQuery.Skip(minIndex).Take((num_above_below * 2) + 1).ToList();

            foreach (var score in scores)
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

            var resp = new Response<List<SubLeaderboard>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new SubLeaderboard {
                    playgroup_size = playgroup_size,
                    sub_group_id = sub_group_id,
                    sub_key_id = sub_key_id,
                    total = total,
                    type = type.ToString(),
                    LeaderboardPlayersList = leaderboardPlayers
                }]
            };
            return resp.Serialize();
        }

        public static string ViewPersonalSubLeaderBoard(Database database, Guid SessionID, int limit, int page, 
            int per_page, LeaderboardType type, int sub_group_id, int sub_key_id, Platform platform, 
            Platform track_platform, SortOrder sort_order, SortColumn sort_column, float longitude, float latitude)
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

            var scoresQuery = database.Scores
                .AsSplitQuery()
                .Include(s => s.User)
                .ThenInclude(u => u.PlayerExperiencePoints)
                .Include(s => s.User)
                .ThenInclude(u => u.PlayerCreationPoints)
                .Where(match => match.PlayerId == user.UserId 
                    && match.SubGroupId == sub_group_id && match.SubKeyId == sub_key_id 
                    && match.Platform == platform && match.IsMNR);

            if (sort_column == SortColumn.finish_time)
                scoresQuery = scoresQuery.OrderBy(s => s.FinishTime);
            if (sort_column == SortColumn.score)
                scoresQuery = scoresQuery.OrderByDescending(s => s.Points);
            if (sort_column == SortColumn.best_lap_time)
                scoresQuery = scoresQuery.OrderBy(s => s.BestLapTime);

            var total = scoresQuery.Count();

            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, total);

            if (pageEnd > total)
                pageEnd = total;

            var scores = scoresQuery.Skip(pageStart).Take(per_page).ToList();

            var LeaderboardScores = new List<PersonalSubLeaderboardPlayer>();

            //for some reason my game skips the first record, so here is my weird way to fix it ._.
            if (scores.Count > 0)
            {
                LeaderboardScores.Add(new PersonalSubLeaderboardPlayer
                {
                    player_id = scores[0].PlayerId,
                    username = scores[0].Username,
                    best_lap_time = scores[0].BestLapTime,
                    character_idx = scores[0].CharacterIdx,
                    kart_idx = scores[0].KartIdx,
                    rank = scores[0].GetRank(sort_column),
                    sub_key_id = scores[0].SubKeyId,
                    track_idx = scores[0].SubKeyId,
                    skill_level_id = scores[0].User.SkillLevelId(platform),
                    latitude = scores[0].Latitude,
                    longitude = scores[0].Longitude,
                    location_tag = scores[0].LocationTag != null ? scores[0].LocationTag : "Unnamed location"
                });
            }

            foreach (var score in scores)
            {
                LeaderboardScores.Add(new PersonalSubLeaderboardPlayer
                {
                    player_id = score.PlayerId,
                    username = score.Username,
                    best_lap_time = score.BestLapTime,
                    character_idx = score.CharacterIdx,
                    kart_idx = score.KartIdx,
                    rank = score.GetRank(sort_column),
                    sub_key_id = score.SubKeyId,
                    track_idx = score.SubKeyId,
                    skill_level_id = score.User.SkillLevelId(platform),
                    latitude = score.Latitude,
                    longitude = score.Longitude,
                    location_tag = score.LocationTag != null ? score.LocationTag : "Unnamed location"
                });
            }

            var MyStats = scoresQuery.FirstOrDefault();
            var mystats = new PersonalSubLeaderboardPlayer { };

            if (MyStats != null)
            {
                mystats.player_id = MyStats.PlayerId;
                mystats.username = MyStats.Username;
                mystats.best_lap_time = MyStats.BestLapTime;
                mystats.character_idx = MyStats.CharacterIdx;
                mystats.kart_idx = MyStats.KartIdx;
                mystats.rank = MyStats.GetRank(sort_column);
                mystats.sub_key_id = MyStats.SubKeyId;
                mystats.track_idx = MyStats.SubKeyId;
                mystats.skill_level_id = MyStats.User.SkillLevelId(platform);
                mystats.latitude = MyStats.Latitude;
                mystats.longitude = MyStats.Longitude;
                mystats.location_tag = MyStats.LocationTag != null ? MyStats.LocationTag : "Unnamed location";
            }

            var resp = new Response<SubLeaderboardPersonalViewResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new SubLeaderboardPersonalViewResponse
                {
                    my_stats = mystats,
                    leaderboard = new PersonalSubLeaderboard
                    {
                        page = page,
                        total_pages = totalPages,
                        total = total,
                        Scores = LeaderboardScores
                    }
                }
            };
            return resp.Serialize();
        }

        public static string ViewLeaderBoard(Database database, Guid SessionID, LeaderboardType type, GameType game_type, Platform platform, int page, 
            int per_page, int column_page, int cols_per_page, SortColumn sort_column, SortOrder sort_order, int limit, 
            string usernameFilter = null, bool FriendsView = false)
        {
            var session = Session.GetSession(SessionID);
            var requestedBy = database.Users
                .AsSplitQuery()
                .Include(x => x.PlayerPoints)
                .Include(x => x.RacesStarted)
                .Include(x => x.RacesFinished)
                .Include(x => x.PlayerCreationPoints)
                .Include(x => x.PlayerExperiencePoints)
                .FirstOrDefault(match => match.Username == session.Username);

            var usersQuery = database.Users
                .AsSplitQuery()
                .Include(x => x.PlayerPoints)
                .Include(x => x.RacesStarted)
                .Include(x => x.RacesFinished)
                .Include(x => x.PlayerCreationPoints)
                .Include(x => x.PlayerExperiencePoints)
                .Where(match => match.Username != "ufg" && match.PlayedMNR);
            var scoresQuery = database.Scores
                .AsSplitQuery()
                .Include(s => s.User)
                .ThenInclude(u => u.PlayerExperiencePoints)
                .Include(s => s.User)
                .ThenInclude(u => u.PlayerCreationPoints)
                .Where(match => match.IsMNR && match.SubGroupId == (int)game_type-10);

            if (usernameFilter != null)
            {
                var usernames = usernameFilter.Split(',');
                usersQuery = usersQuery.Where(u => usernames.Contains(u.Username));
                scoresQuery = scoresQuery.Where(s => usernames.Contains(s.User.Username));
            }

            int Total = 0;

            //creator points
            if (game_type == GameType.OVERALL_CREATORS && type == LeaderboardType.LIFETIME)
                usersQuery = usersQuery.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform).Sum(p => p.Amount));
            if (game_type == GameType.OVERALL_CREATORS && type == LeaderboardType.WEEKLY)
                usersQuery = usersQuery.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform && match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount));
            if (game_type == GameType.OVERALL_CREATORS && type == LeaderboardType.LAST_WEEK)
                usersQuery = usersQuery.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform && match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount));

            //creator points for characters
            if (game_type == GameType.CHARACTER_CREATORS && type == LeaderboardType.LIFETIME)
                usersQuery = usersQuery.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == PlayerCreationType.CHARACTER).Sum(p => p.Amount));
            if (game_type == GameType.CHARACTER_CREATORS && type == LeaderboardType.WEEKLY)
                usersQuery = usersQuery.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == PlayerCreationType.CHARACTER && match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount));
            if (game_type == GameType.CHARACTER_CREATORS && type == LeaderboardType.LAST_WEEK)
                usersQuery = usersQuery.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == PlayerCreationType.CHARACTER && match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount));

            //creator points for karts
            if (game_type == GameType.CHARACTER_CREATORS && type == LeaderboardType.LIFETIME)
                usersQuery = usersQuery.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == PlayerCreationType.KART).Sum(p => p.Amount));
            if (game_type == GameType.CHARACTER_CREATORS && type == LeaderboardType.WEEKLY)
                usersQuery = usersQuery.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == PlayerCreationType.KART && match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount));
            if (game_type == GameType.CHARACTER_CREATORS && type == LeaderboardType.LAST_WEEK)
                usersQuery = usersQuery.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == PlayerCreationType.KART && match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount));

            //creator points for tracks
            if (game_type == GameType.CHARACTER_CREATORS && type == LeaderboardType.LIFETIME)
                usersQuery = usersQuery.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == PlayerCreationType.TRACK).Sum(p => p.Amount));
            if (game_type == GameType.CHARACTER_CREATORS && type == LeaderboardType.WEEKLY)
                usersQuery = usersQuery.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == PlayerCreationType.TRACK && match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount));
            if (game_type == GameType.CHARACTER_CREATORS && type == LeaderboardType.LAST_WEEK)
                usersQuery = usersQuery.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == PlayerCreationType.TRACK && match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount));

            //Experience points
            if (game_type == GameType.OVERALL && type == LeaderboardType.LIFETIME)
                usersQuery = usersQuery.OrderByDescending(u => (platform == Platform.PSV ? 0 : u.PlayerExperiencePoints.Sum(p => p.Amount)) + u.PlayerCreationPoints.Where(match => match.Platform == platform).Sum(p => p.Amount));
            if (game_type == GameType.OVERALL && type == LeaderboardType.WEEKLY)
                usersQuery = usersQuery.OrderByDescending(u => (platform == Platform.PSV ? 0 : u.PlayerExperiencePoints.Where(match => match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount)) + u.PlayerCreationPoints.Where(match => match.Platform == platform && match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount));
            if (game_type == GameType.OVERALL && type == LeaderboardType.LAST_WEEK)
                usersQuery = usersQuery.OrderByDescending(u => (platform == Platform.PSV ? 0 : u.PlayerExperiencePoints.Where(match => match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount)) + u.PlayerCreationPoints.Where(match => match.Platform == platform && match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount));

            if (game_type == GameType.OVERALL_RACE)
            {
                switch (sort_column)
                {
                    case SortColumn.experience_points:
                        if (type == LeaderboardType.LIFETIME)
                            usersQuery = usersQuery.OrderByDescending(u => u.PlayerExperiencePoints.Sum(p => p.Amount));
                        if (type == LeaderboardType.WEEKLY)
                            usersQuery = usersQuery.OrderByDescending(u => u.PlayerExperiencePoints.Where(match => match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount));
                        if (type == LeaderboardType.LAST_WEEK)
                            usersQuery = usersQuery.OrderByDescending(u => u.PlayerExperiencePoints.Where(match => match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount));
                        break;

                    case SortColumn.online_races:
                        usersQuery = usersQuery.OrderByDescending(u => u.RacesStarted.Count);
                        break;

                    case SortColumn.online_wins:
                        usersQuery = usersQuery.OrderByDescending(u => u.RacesFinished.Count(match => match.IsWinner));
                        break;

                    case SortColumn.longest_win_streak:
                        usersQuery = usersQuery.OrderByDescending(u => u.LongestWinStreak);
                        break;

                    case SortColumn.win_streak:
                        usersQuery = usersQuery.OrderByDescending(u => u.WinStreak);
                        break;

                    case SortColumn.longest_hang_time:
                        usersQuery = usersQuery.OrderByDescending(u => u.LongestHangTime);
                        break;

                    case SortColumn.longest_drift:
                        usersQuery = usersQuery.OrderByDescending(u => u.LongestDrift);
                        break;

                    default:
                        break;
                }
            }

            if (sort_column == SortColumn.finish_time)
                scoresQuery = scoresQuery.OrderBy(s => s.FinishTime);
            if (sort_column == SortColumn.score)
                scoresQuery = scoresQuery.OrderByDescending(s => s.Points);
            if (sort_column == SortColumn.best_lap_time)
                scoresQuery = scoresQuery.OrderBy(s => s.BestLapTime);

            if (game_type == GameType.OVERALL_CREATORS || game_type == GameType.CHARACTER_CREATORS 
                || game_type == GameType.TRACK_CREATORS || game_type == GameType.KART_CREATORS 
                || game_type == GameType.OVERALL || game_type == GameType.OVERALL_RACE) Total = usersQuery.Count();

            if (game_type == GameType.ONLINE_HOT_SEAT_RACE) Total = scoresQuery.Count();

            var MyStats = requestedBy != null ? scoresQuery.FirstOrDefault(match => match.PlayerId == requestedBy.UserId) : null;
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

            if (game_type == GameType.OVERALL_CREATORS || game_type == GameType.CHARACTER_CREATORS
                    || game_type == GameType.TRACK_CREATORS || game_type == GameType.KART_CREATORS
                    || game_type == GameType.OVERALL || game_type == GameType.OVERALL_RACE)
            {
                var users = usersQuery.Skip(pageStart).Take(per_page).ToList();
                foreach (var user in users)
                {
                    if (game_type == GameType.OVERALL_RACE)
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
                    else
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
            }
            if (game_type == GameType.ONLINE_HOT_SEAT_RACE)
            {
                var scores = scoresQuery.Skip(pageStart).Take(per_page).ToList();
                foreach (var score in scores)
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

            var leaderboardColumns = new LeaderboardColumns();

            if (game_type == GameType.OVERALL_RACE)
            {
                leaderboardColumns.Columns = [
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
                ];
            }

            if (game_type == GameType.ONLINE_HOT_SEAT_RACE)
            {
                leaderboardColumns.Columns = [
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
                ];
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

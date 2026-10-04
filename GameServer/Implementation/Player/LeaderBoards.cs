using GameServer.Models;
using GameServer.Models.GameBrowser;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using LinqToDB;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Implementation.Player
{
    public class LeaderBoards
    {
        public static string ViewSubLeaderBoard(Database database, SessionData session, int sub_group_id, int sub_key_id, LeaderboardType type, Platform platform,
            int page, int per_page, int column_page, int cols_per_page, SortColumn sort_column, SortOrder sort_order, int? num_above_below, int limit, int playgroup_size,
            float? latitude, float? longitude, string usernameFilter = null, bool FriendsView = false)
        {
            var scoresQuery = database.Scores
                .AsNoTracking()
                .Include(s => s.User)
                .Where(match => match.SubKeyId == sub_key_id && match.SubGroupId == sub_group_id 
                    && match.PlaygroupSize == playgroup_size && match.IsMNR == session.IsMNR 
                    && match.Platform == platform);

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
            
            if (platform == Platform.PSV && (latitude == null || longitude == null))
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

            var finalQuery = scoresQuery.Select(score => new SubLeaderboardPlayer
            {
                id = score.Id,
                created_at = score.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                finish_time = score.FinishTime,
                platform = score.Platform.ToString(),
                player_id = score.PlayerId,
                username = score.Username,
                playgroup_size = score.PlaygroupSize,
                rank = (int)Sql.Window.RowNumber(f =>
                    sort_column == SortColumn.finish_time ? f.OrderBy(score.FinishTime) :
                    sort_column == SortColumn.best_lap_time ? f.OrderBy(score.BestLapTime) : f.OrderByDesc(score.Points)),
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
            }).ToLinqToDB();

            var myStats = new SubLeaderboardPlayer { };

            if (session.User != null)
                myStats = finalQuery.FirstOrDefault(match => match.player_id == session.UserId);

            var total = finalQuery.Count();

            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, total);

            if (pageEnd > total)
                pageEnd = total;

            var scores = finalQuery.Skip(pageStart).Take(per_page).ToList();

            if (FriendsView)
            {
                var friendsViewResp = new Response<SubLeaderboardFriendsViewResponse>
                {
                    status = new ResponseStatus { id = 0, message = "Successful completion" },
                    response = new SubLeaderboardFriendsViewResponse
                    {
                        my_stats = myStats,
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
                            LeaderboardPlayersList = scores
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
                    my_stats = myStats,
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
                        LeaderboardPlayersList = scores
                    }
                }
            };
            return resp.Serialize();
        }

        public static string ViewSubLeaderBoardAroundMe(Database database, SessionData session, int sub_group_id, int sub_key_id, LeaderboardType type, Platform platform,
            int column_page, int cols_per_page, SortColumn sort_column, SortOrder sort_order, int num_above_below, int playgroup_size, int limit)
        {
            var scoresQuery = database.Scores
                .AsNoTracking()
                .Where(match => match.SubKeyId == sub_key_id 
                    && match.SubGroupId == sub_group_id && match.PlaygroupSize == playgroup_size 
                    && match.Platform == platform && match.IsMNR == session.IsMNR);
            var user = session.User;

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

            var finalQuery = scoresQuery.Select(score => new SubLeaderboardPlayer
            {
                id = score.Id,
                created_at = score.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                finish_time = score.FinishTime,
                platform = score.Platform.ToString(),
                player_id = score.PlayerId,
                username = score.Username,
                playgroup_size = score.PlaygroupSize,
                rank = (int)Sql.Window.RowNumber(f =>
                    sort_column == SortColumn.finish_time ? f.OrderBy(score.FinishTime) :
                    sort_column == SortColumn.best_lap_time ? f.OrderBy(score.BestLapTime) : f.OrderByDesc(score.Points)),
                score = score.Points,
                sub_group_id = score.SubGroupId,
                sub_key_id = score.SubKeyId,
                updated_at = score.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")
            }).ToLinqToDB();
            
            var myStats = finalQuery
                .Select(s => new { s.player_id, s.rank })
                .FirstOrDefault(match => match.player_id == user.UserId);

            int myIndex = (myStats?.rank ?? 1) - 1;
            
            int minIndex = myIndex - num_above_below;

            var total = finalQuery.Count();

            if (minIndex < 0)
                minIndex = 0;

            var scores = finalQuery.Skip(minIndex).Take((num_above_below * 2) + 1).ToList();

            var resp = new Response<List<SubLeaderboard>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new SubLeaderboard {
                    playgroup_size = playgroup_size,
                    sub_group_id = sub_group_id,
                    sub_key_id = sub_key_id,
                    total = total,
                    type = type.ToString(),
                    LeaderboardPlayersList = scores
                }]
            };
            return resp.Serialize();
        }

        public static string ViewPersonalSubLeaderBoard(Database database, SessionData session, int limit, int page, 
            int per_page, LeaderboardType type, int sub_group_id, int sub_key_id, Platform platform, 
            Platform track_platform, SortOrder sort_order, SortColumn sort_column, float longitude, float latitude)
        {
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

            var scoresQuery = database.Scores
                .AsNoTracking()
                .Include(s => s.User)
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

            var finalQuery = scoresQuery.Select(score => new PersonalSubLeaderboardPlayer
            {
                player_id = score.PlayerId,
                username = score.Username,
                best_lap_time = score.BestLapTime,
                character_idx = score.CharacterIdx,
                kart_idx = score.KartIdx,
                rank = (int)Sql.Window.RowNumber(f =>
                    sort_column == SortColumn.finish_time ? f.OrderBy(score.FinishTime) :
                    sort_column == SortColumn.best_lap_time ? f.OrderBy(score.BestLapTime) : f.OrderByDesc(score.Points)),
                sub_key_id = score.SubKeyId,
                track_idx = score.SubKeyId,
                skill_level_id = score.User.SkillLevelId(platform),
                latitude = score.Latitude,
                longitude = score.Longitude,
                location_tag = score.LocationTag ?? "Unnamed location"
            }).ToLinqToDB();

            var scores = finalQuery.Skip(pageStart).Take(per_page).ToList();

            var leaderboardScores = new List<PersonalSubLeaderboardPlayer>();

            if (scores.Count > 0)
            {
                leaderboardScores.Add(scores[0]);//for some reason my game skips the first record, so here is my weird way to fix it ._.
                leaderboardScores.AddRange(scores);
            }

            var mystats = finalQuery.FirstOrDefault(match => match.player_id == user.UserId);

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
                        Scores = leaderboardScores
                    }
                }
            };
            return resp.Serialize();
        }

        public static string ViewLeaderBoard(Database database, SessionData session, LeaderboardType type, GameType game_type, Platform platform, int page, 
            int per_page, int column_page, int cols_per_page, SortColumn sort_column, SortOrder sort_order, int limit, 
            string usernameFilter = null, bool FriendsView = false)
        {
            var requestedBy = session.User;

            var usersQuery = database.Users
                .AsNoTracking()
                .Where(match => match.Username != "ufg" && match.PlayedMNR);
            var scoresQuery = database.Scores
                .AsNoTracking()
                .Include(s => s.User)
                .Where(match => match.IsMNR && match.SubGroupId == (int)game_type-10);

            if (usernameFilter != null)
            {
                var usernames = usernameFilter.Split(',');
                usersQuery = usersQuery.Where(u => usernames.Contains(u.Username));
                scoresQuery = scoresQuery.Where(s => usernames.Contains(s.Username));
            }

            int total = 0;

            //creator points
            if (game_type == GameType.OVERALL_CREATORS && type == LeaderboardType.LIFETIME)
                usersQuery = usersQuery.OrderByDescending(u => u.CreatorPoints(platform));
            if (game_type == GameType.OVERALL_CREATORS && type == LeaderboardType.WEEKLY)
                usersQuery = usersQuery.OrderByDescending(u => u.CreatorPointsThisWeek(platform));
            if (game_type == GameType.OVERALL_CREATORS && type == LeaderboardType.LAST_WEEK)
                usersQuery = usersQuery.OrderByDescending(u => u.CreatorPointsLastWeek(platform));

            //creator points for characters
            if (game_type == GameType.CHARACTER_CREATORS && type == LeaderboardType.LIFETIME)
                usersQuery = usersQuery.OrderByDescending(u => u.CreatorPoints(platform, PlayerCreationType.CHARACTER));
            if (game_type == GameType.CHARACTER_CREATORS && type == LeaderboardType.WEEKLY)
                usersQuery = usersQuery.OrderByDescending(u => u.CreatorPointsThisWeek(platform, PlayerCreationType.CHARACTER));
            if (game_type == GameType.CHARACTER_CREATORS && type == LeaderboardType.LAST_WEEK)
                usersQuery = usersQuery.OrderByDescending(u => u.CreatorPointsLastWeek(platform, PlayerCreationType.CHARACTER));

            //creator points for karts
            if (game_type == GameType.KART_CREATORS && type == LeaderboardType.LIFETIME)
                usersQuery = usersQuery.OrderByDescending(u => u.CreatorPoints(platform, PlayerCreationType.KART));
            if (game_type == GameType.KART_CREATORS && type == LeaderboardType.WEEKLY)
                usersQuery = usersQuery.OrderByDescending(u => u.CreatorPointsThisWeek(platform, PlayerCreationType.KART));
            if (game_type == GameType.KART_CREATORS && type == LeaderboardType.LAST_WEEK)
                usersQuery = usersQuery.OrderByDescending(u => u.CreatorPointsLastWeek(platform, PlayerCreationType.KART));

            //creator points for tracks
            if (game_type == GameType.TRACK_CREATORS && type == LeaderboardType.LIFETIME)
                usersQuery = usersQuery.OrderByDescending(u => u.CreatorPoints(platform, PlayerCreationType.TRACK));
            if (game_type == GameType.TRACK_CREATORS && type == LeaderboardType.WEEKLY)
                usersQuery = usersQuery.OrderByDescending(u => u.CreatorPointsThisWeek(platform, PlayerCreationType.TRACK));
            if (game_type == GameType.TRACK_CREATORS && type == LeaderboardType.LAST_WEEK)
                usersQuery = usersQuery.OrderByDescending(u => u.CreatorPointsLastWeek(platform, PlayerCreationType.TRACK));

            //Experience points
            if (game_type == GameType.OVERALL && type == LeaderboardType.LIFETIME)
                usersQuery = usersQuery.OrderByDescending(u => u.TotalXP(platform));
            if (game_type == GameType.OVERALL && type == LeaderboardType.WEEKLY)
                usersQuery = usersQuery.OrderByDescending(u => u.TotalXPThisWeek(platform));
            if (game_type == GameType.OVERALL && type == LeaderboardType.LAST_WEEK)
                usersQuery = usersQuery.OrderByDescending(u => u.TotalXPLastWeek(platform));

            if (game_type == GameType.OVERALL_RACE)
            {
                switch (sort_column)
                {
                    case SortColumn.experience_points:
                        if (type == LeaderboardType.LIFETIME)
                            usersQuery = usersQuery.OrderByDescending(u => u.ExperiencePoints(platform));
                        if (type == LeaderboardType.WEEKLY)
                            usersQuery = usersQuery.OrderByDescending(u => u.ExperiencePointsThisWeek(platform));
                        if (type == LeaderboardType.LAST_WEEK)
                            usersQuery = usersQuery.OrderByDescending(u => u.ExperiencePointsLastWeek(platform));
                        break;

                    case SortColumn.online_races:
                        usersQuery = usersQuery.OrderByDescending(u => u.OnlineRaces);
                        break;

                    case SortColumn.online_wins:
                        usersQuery = usersQuery.OrderByDescending(u => u.OnlineWins);
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

                    case SortColumn.points:
                        if (type == LeaderboardType.LIFETIME)
                            usersQuery = usersQuery.OrderByDescending(u => u.Points(platform));
                        if (type == LeaderboardType.WEEKLY)
                            usersQuery = usersQuery.OrderByDescending(u => u.PointsThisWeek(platform));
                        if (type == LeaderboardType.LAST_WEEK)
                            usersQuery = usersQuery.OrderByDescending(u => u.PointsLastWeek(platform));
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

            var finalUsersQuery = usersQuery.Select(user => game_type == GameType.OVERALL_RACE ? new LeaderboardPlayer
            {
                created_at = user.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                experience_points = user.ExperiencePoints(platform),
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
                points = user.Points(platform),
                updated_at = user.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                username = user.Username,
                win_streak = user.WinStreak,
                rank = (int)Sql.Window.RowNumber(f =>
                    sort_column == SortColumn.experience_points ? f.OrderByDesc(
                        type == LeaderboardType.LAST_WEEK ? user.ExperiencePointsLastWeek(platform) :
                        type == LeaderboardType.WEEKLY ? user.ExperiencePointsThisWeek(platform) :
                        user.ExperiencePoints(platform)) :
                    sort_column == SortColumn.online_races ? f.OrderByDesc(user.OnlineRaces) :
                    sort_column == SortColumn.online_wins ? f.OrderByDesc(user.OnlineWins) :
                    sort_column == SortColumn.longest_win_streak ? f.OrderByDesc(user.LongestWinStreak) :
                    sort_column == SortColumn.win_streak ? f.OrderByDesc(user.WinStreak) :
                    sort_column == SortColumn.longest_hang_time ? f.OrderByDesc(user.LongestHangTime) :
                    sort_column == SortColumn.longest_drift ? f.OrderByDesc(user.LongestDrift) : 
                    f.OrderByDesc(type == LeaderboardType.LAST_WEEK ? user.PointsLastWeek(platform) :
                              type == LeaderboardType.WEEKLY ? user.PointsThisWeek(platform) :
                              user.Points(platform))),
                skill_level_id = user.SkillLevelId(platform),
                skill_level_name = user.SkillLevelName(platform),
                character_idx = user.CharacterIdx,
                kart_idx = user.KartIdx
            } : new LeaderboardPlayer
            {
                created_at = user.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                id = user.UserId,
                player_id = user.UserId,
                points = game_type == GameType.OVERALL_CREATORS ? 
                             (type == LeaderboardType.LAST_WEEK ? user.CreatorPointsLastWeek(platform) : 
                              type == LeaderboardType.WEEKLY ? user.CreatorPointsThisWeek(platform) : 
                              user.CreatorPoints(platform)) : 
                         game_type == GameType.KART_CREATORS ? 
                             (type == LeaderboardType.LAST_WEEK ? user.CreatorPointsLastWeek(platform, PlayerCreationType.KART) : 
                              type == LeaderboardType.WEEKLY ? user.CreatorPointsThisWeek(platform, PlayerCreationType.KART) : 
                              user.CreatorPoints(platform, PlayerCreationType.KART)) : 
                         game_type == GameType.TRACK_CREATORS ? 
                             (type == LeaderboardType.LAST_WEEK ? user.CreatorPointsLastWeek(platform, PlayerCreationType.TRACK) : 
                              type == LeaderboardType.WEEKLY ? user.CreatorPointsThisWeek(platform, PlayerCreationType.TRACK) : 
                              user.CreatorPoints(platform, PlayerCreationType.TRACK)) : 
                         game_type == GameType.CHARACTER_CREATORS ? 
                             (type == LeaderboardType.LAST_WEEK ? user.CreatorPointsLastWeek(platform, PlayerCreationType.CHARACTER) : 
                              type == LeaderboardType.WEEKLY ? user.CreatorPointsThisWeek(platform, PlayerCreationType.CHARACTER) : 
                              user.CreatorPoints(platform, PlayerCreationType.CHARACTER)) : 
                         (type == LeaderboardType.LAST_WEEK ? user.TotalXPLastWeek(platform) : 
                          type == LeaderboardType.WEEKLY ? user.TotalXPThisWeek(platform) : 
                          user.TotalXP(platform)),
                updated_at = user.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                username = user.Username,
                rank = (int)Sql.Window.RowNumber(f =>
                    game_type == GameType.OVERALL_CREATORS ? 
                        f.OrderByDesc(type == LeaderboardType.LAST_WEEK ? user.CreatorPointsLastWeek(platform) : 
                                      type == LeaderboardType.WEEKLY ? user.CreatorPointsThisWeek(platform) : 
                                      user.CreatorPoints(platform)) : 
                    game_type == GameType.KART_CREATORS ? 
                        f.OrderByDesc(type == LeaderboardType.LAST_WEEK ? user.CreatorPointsLastWeek(platform, PlayerCreationType.KART) : 
                                      type == LeaderboardType.WEEKLY ? user.CreatorPointsThisWeek(platform, PlayerCreationType.KART) : 
                                      user.CreatorPoints(platform, PlayerCreationType.KART)) : 
                    game_type == GameType.TRACK_CREATORS ? 
                        f.OrderByDesc(type == LeaderboardType.LAST_WEEK ? user.CreatorPointsLastWeek(platform, PlayerCreationType.TRACK) : 
                                      type == LeaderboardType.WEEKLY ? user.CreatorPointsThisWeek(platform, PlayerCreationType.TRACK) : 
                                      user.CreatorPoints(platform, PlayerCreationType.TRACK)) : 
                    game_type == GameType.CHARACTER_CREATORS ? 
                        f.OrderByDesc(type == LeaderboardType.LAST_WEEK ? user.CreatorPointsLastWeek(platform, PlayerCreationType.CHARACTER) : 
                                      type == LeaderboardType.WEEKLY ? user.CreatorPointsThisWeek(platform, PlayerCreationType.CHARACTER) : 
                                      user.CreatorPoints(platform, PlayerCreationType.CHARACTER)) : 
                    f.OrderByDesc(type == LeaderboardType.LAST_WEEK ? user.TotalXPLastWeek(platform) : 
                                  type == LeaderboardType.WEEKLY ? user.TotalXPThisWeek(platform) : 
                                  user.TotalXP(platform))),
                skill_level_id = user.SkillLevelId(platform),
                skill_level_name = user.SkillLevelName(platform)
            }).ToLinqToDB();
            var finalScoresQuery = scoresQuery.Select(score => new LeaderboardPlayer
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
                rank = (int)Sql.Window.RowNumber(f =>
                    sort_column == SortColumn.finish_time ? f.OrderBy(score.FinishTime) :
                    sort_column == SortColumn.best_lap_time ? f.OrderBy(score.BestLapTime) : f.OrderByDesc(score.Points)),
                skill_level_id = score.User.SkillLevelId(platform),
                skill_level_name = score.User.SkillLevelName(platform)
            }).ToLinqToDB();
            
            if (game_type == GameType.OVERALL_CREATORS || game_type == GameType.CHARACTER_CREATORS 
                || game_type == GameType.TRACK_CREATORS || game_type == GameType.KART_CREATORS 
                || game_type == GameType.OVERALL || game_type == GameType.OVERALL_RACE) total = finalUsersQuery.Count();

            if (game_type == GameType.ONLINE_HOT_SEAT_RACE) total = finalScoresQuery.Count();

            var myStats = new LeaderboardPlayer { };

            if (requestedBy != null)
            {
                if (game_type == GameType.OVERALL_CREATORS || game_type == GameType.CHARACTER_CREATORS
                    || game_type == GameType.TRACK_CREATORS || game_type == GameType.KART_CREATORS 
                    || game_type == GameType.OVERALL || game_type == GameType.OVERALL_RACE)
                    myStats = finalUsersQuery.FirstOrDefault(match => match.player_id == requestedBy.UserId);
                if (game_type == GameType.ONLINE_HOT_SEAT_RACE)
                    myStats = finalScoresQuery.FirstOrDefault(match => match.player_id == requestedBy.UserId);
            }

            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, total);

            if (pageEnd > total)
                pageEnd = total;

            var leaderboardPlayers = new List<LeaderboardPlayer>();

            if (game_type == GameType.OVERALL_CREATORS || game_type == GameType.CHARACTER_CREATORS
                || game_type == GameType.TRACK_CREATORS || game_type == GameType.KART_CREATORS
                || game_type == GameType.OVERALL || game_type == GameType.OVERALL_RACE)
                leaderboardPlayers = finalUsersQuery.Skip(pageStart).Take(per_page).ToList();
            if (game_type == GameType.ONLINE_HOT_SEAT_RACE)
                leaderboardPlayers = finalScoresQuery.Skip(pageStart).Take(per_page).ToList();

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
                        my_stats = myStats,
                        friends_leaderboard = new Leaderboard
                        {
                            total = total,
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
                    my_stats = myStats,
                    leaderboard = new Leaderboard
                    {
                        total = total,
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
        
        public static string PlayerStats(Database database, SessionData session, LeaderboardType type, GameType game_type, Platform platform, int player_id)
        {
            var user = database.Users
                .AsNoTracking()
                .Where(match => match.PlayedMNR)
                .Select(user => game_type == GameType.OVERALL_RACE ? new LeaderboardPlayer
                {
                    created_at = user.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    experience_points = user.ExperiencePoints(platform),
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
                    points = user.Points(platform),
                    updated_at = user.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    username = user.Username,
                    win_streak = user.WinStreak,
                    rank = (int)Sql.Window.RowNumber(f =>
                         f.OrderByDesc(
                            type == LeaderboardType.LAST_WEEK ? user.ExperiencePointsLastWeek(platform) :
                            type == LeaderboardType.WEEKLY ? user.ExperiencePointsThisWeek(platform) :
                            user.ExperiencePoints(platform))),
                    skill_level_id = user.SkillLevelId(platform),
                    skill_level_name = user.SkillLevelName(platform),
                    character_idx = user.CharacterIdx,
                    kart_idx = user.KartIdx
                } : new LeaderboardPlayer
                {
                    created_at = user.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    id = user.UserId,
                    player_id = user.UserId,
                    points = game_type == GameType.OVERALL_CREATORS ? 
                                 (type == LeaderboardType.LAST_WEEK ? user.CreatorPointsLastWeek(platform) : 
                                  type == LeaderboardType.WEEKLY ? user.CreatorPointsThisWeek(platform) : 
                                  user.CreatorPoints(platform)) : 
                             game_type == GameType.KART_CREATORS ? 
                                 (type == LeaderboardType.LAST_WEEK ? user.CreatorPointsLastWeek(platform, PlayerCreationType.KART) : 
                                  type == LeaderboardType.WEEKLY ? user.CreatorPointsThisWeek(platform, PlayerCreationType.KART) : 
                                  user.CreatorPoints(platform, PlayerCreationType.KART)) : 
                             game_type == GameType.TRACK_CREATORS ? 
                                 (type == LeaderboardType.LAST_WEEK ? user.CreatorPointsLastWeek(platform, PlayerCreationType.TRACK) : 
                                  type == LeaderboardType.WEEKLY ? user.CreatorPointsThisWeek(platform, PlayerCreationType.TRACK) : 
                                  user.CreatorPoints(platform, PlayerCreationType.TRACK)) : 
                             game_type == GameType.CHARACTER_CREATORS ? 
                                 (type == LeaderboardType.LAST_WEEK ? user.CreatorPointsLastWeek(platform, PlayerCreationType.CHARACTER) : 
                                  type == LeaderboardType.WEEKLY ? user.CreatorPointsThisWeek(platform, PlayerCreationType.CHARACTER) : 
                                  user.CreatorPoints(platform, PlayerCreationType.CHARACTER)) : 
                             (type == LeaderboardType.LAST_WEEK ? user.TotalXPLastWeek(platform) : 
                              type == LeaderboardType.WEEKLY ? user.TotalXPThisWeek(platform) : 
                              user.TotalXP(platform)),
                    updated_at = user.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    username = user.Username,
                    rank = (int)Sql.Window.RowNumber(f =>
                        game_type == GameType.OVERALL_CREATORS ? 
                            f.OrderByDesc(type == LeaderboardType.LAST_WEEK ? user.CreatorPointsLastWeek(platform) : 
                                          type == LeaderboardType.WEEKLY ? user.CreatorPointsThisWeek(platform) : 
                                          user.CreatorPoints(platform)) : 
                        game_type == GameType.KART_CREATORS ? 
                            f.OrderByDesc(type == LeaderboardType.LAST_WEEK ? user.CreatorPointsLastWeek(platform, PlayerCreationType.KART) : 
                                          type == LeaderboardType.WEEKLY ? user.CreatorPointsThisWeek(platform, PlayerCreationType.KART) : 
                                          user.CreatorPoints(platform, PlayerCreationType.KART)) : 
                        game_type == GameType.TRACK_CREATORS ? 
                            f.OrderByDesc(type == LeaderboardType.LAST_WEEK ? user.CreatorPointsLastWeek(platform, PlayerCreationType.TRACK) : 
                                          type == LeaderboardType.WEEKLY ? user.CreatorPointsThisWeek(platform, PlayerCreationType.TRACK) : 
                                          user.CreatorPoints(platform, PlayerCreationType.TRACK)) : 
                        game_type == GameType.CHARACTER_CREATORS ? 
                            f.OrderByDesc(type == LeaderboardType.LAST_WEEK ? user.CreatorPointsLastWeek(platform, PlayerCreationType.CHARACTER) : 
                                          type == LeaderboardType.WEEKLY ? user.CreatorPointsThisWeek(platform, PlayerCreationType.CHARACTER) : 
                                          user.CreatorPoints(platform, PlayerCreationType.CHARACTER)) : 
                        f.OrderByDesc(type == LeaderboardType.LAST_WEEK ? user.TotalXPLastWeek(platform) : 
                                      type == LeaderboardType.WEEKLY ? user.TotalXPThisWeek(platform) : 
                                      user.TotalXP(platform))),
                    skill_level_id = user.SkillLevelId(platform),
                    skill_level_name = user.SkillLevelName(platform)
                })
                .ToLinqToDB()
                .FirstOrDefault(match => match.player_id == player_id);
            
            var score = database.Scores
                .AsNoTracking()
                .Include(s => s.User)
                .Where(match => match.IsMNR && match.SubGroupId == (int)game_type-10)
                .Select(score => new LeaderboardPlayer
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
                    rank = (int)Sql.Window.RowNumber(f => f.OrderBy(score.BestLapTime)),
                    skill_level_id = score.User.SkillLevelId(platform),
                    skill_level_name = score.User.SkillLevelName(platform)
                })
                .ToLinqToDB()
                .FirstOrDefault(match => match.player_id == player_id);

            if (user == null || (game_type == GameType.ONLINE_HOT_SEAT_RACE && score == null))
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse() { }
                };
                return errorResp.Serialize();
            }
            
            var playerStats = new LeaderboardPlayer();
            
            if (game_type == GameType.OVERALL_CREATORS || game_type == GameType.CHARACTER_CREATORS
                 || game_type == GameType.TRACK_CREATORS || game_type == GameType.KART_CREATORS || game_type == GameType.OVERALL
                 || game_type == GameType.OVERALL_RACE)
                 playerStats = user;
            if (game_type == GameType.ONLINE_HOT_SEAT_RACE)
                playerStats = score;
            
            var resp = new Response<LeaderboardPlayerStatsResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new LeaderboardPlayerStatsResponse()
                {
                    player_stats = playerStats
                }
            };
            return resp.Serialize();
        }
    }
}

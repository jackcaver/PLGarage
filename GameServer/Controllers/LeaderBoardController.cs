using GameServer.Models.Response;
using GameServer.Models;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using GameServer.Models.Request;
using System.Text.RegularExpressions;
using System.Linq;
using Serilog;
using GameServer.Models.PlayerData;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using GameServer.Models.PlayerData.PlayerCreations;

namespace GameServer.Controllers
{
    public class LeaderBoardController : Controller
    {
        private readonly Database database;

        public LeaderBoardController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("sub_leaderboards/view.xml")]
        public IActionResult View(int sub_group_id, int sub_key_id, LeaderboardType type, Platform platform, int page, int per_page, int column_page, int cols_per_page, SortColumn sort_column, SortOrder sort_order, int num_above_below, int limit, int playgroup_size)
        {
            var scores = this.database.Scores.Where(match => match.SubKeyId == sub_key_id && match.SubGroupId == match.SubGroupId && match.PlaygroupSize == playgroup_size).ToList();
            var user = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);

            if (sort_column == SortColumn.finish_time)
                scores.Sort((curr, prev) => prev.FinishTime.CompareTo(curr.FinishTime));
            if (sort_column == SortColumn.score)
                scores.Sort((curr, prev) => prev.Points.CompareTo(curr.Points));

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var MyStats = scores.FirstOrDefault(match => match.PlayerId == user.UserId);
            var mystats = new LeaderboardPlayer {};

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
            }

            //calculating pages
            int pageEnd = per_page * page;
            int pageStart = pageEnd - per_page;
            int totalPages = scores.Count / per_page;

            if (pageEnd > scores.Count)
                pageEnd = scores.Count;

            var leaderboardPlayers = new List<LeaderboardPlayer> {};

            for (int i = pageStart; i < pageEnd; i++)
            {
                var score = scores[i];
                if (score != null)
                {
                    leaderboardPlayers.Add(new LeaderboardPlayer
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

            var resp = new Response<SubLeaderboardViewResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new SubLeaderboardViewResponse { 
                    my_stats = mystats,
                    leaderboard = new leaderboard {
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
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("sub_leaderboards/around_me.xml")]
        public IActionResult AroundMe(int sub_group_id, int sub_key_id, LeaderboardType type, Platform platform, int column_page, int cols_per_page, SortColumn sort_column, SortOrder sort_order, int num_above_below, int playgroup_size, int limit) 
        {
            var scores = this.database.Scores.Where(match => match.SubKeyId == sub_key_id && match.SubGroupId == match.SubGroupId && match.PlaygroupSize == playgroup_size).ToList();
            var user = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);

            if (sort_column == SortColumn.finish_time)
                scores.Sort((curr, prev) => prev.FinishTime.CompareTo(curr.FinishTime));
            if (sort_column == SortColumn.score)
                scores.Sort((curr, prev) => prev.Points.CompareTo(curr.Points));

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            int MyStatsIndex = scores.FindIndex(match => match.PlayerId == user.UserId);

            var leaderboardPlayers = new List<LeaderboardPlayer> { };
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
                    leaderboardPlayers.Add(new LeaderboardPlayer
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

            var resp = new Response<List<leaderboard>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<leaderboard> { new leaderboard {
                    playgroup_size = playgroup_size,
                    sub_group_id = sub_group_id,
                    sub_key_id = sub_key_id,
                    total = leaderboardPlayers.Count,
                    type = type.ToString(),
                    LeaderboardPlayersList = leaderboardPlayers
                }}
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("sub_leaderboards/friends_view.xml")]
        public IActionResult FriendsView(int sub_group_id, int sub_key_id, LeaderboardType type, Platform platform, int page, int per_page, int column_page, int cols_per_page, SortColumn sort_column, SortOrder sort_order, int? num_above_below, int limit, int playgroup_size)
        {
            var user = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            string usernameFilterQuery = Request.Query["filters[username]"];
            string[] usernameFilter = new string[0];
            var scores = new List<Score> {};

            //genious story level check
            List<int> storyLevelIds = new List<int> { 845, 576, 753, 734, 510, 612, 861, 755, 758, 657, 930, 951, 610, 501, 777, 689, 814, 869, 729, 596, 760, 
                699, 705, 939, 903, 609, 647, 529, 915, 684, 849, 582, 790, 857, 715, 738, 625, 959, 998, 520, 881, 828, 712, 840, 624, 811, 823, 918, 614, 
                1049, 821, 702, 913, 766, 606, 550, 708, 648, 772, 688, 579, 539, 698, 759, 763, 941, 630, 697, 808, 1023, 703 };
            if (storyLevelIds.Contains(sub_key_id))
                UserGeneratedContentUtils.AddStoryLevel(this.database, sub_key_id, sort_column);
            
            if (usernameFilterQuery != null)
            {
                usernameFilter = usernameFilterQuery.Split(',');
            }

            foreach (string username in usernameFilter)
            {
                var friend = this.database.Users.FirstOrDefault(match => match.Username == username);
                if (friend != null)
                {
                    var score = this.database.Scores.FirstOrDefault(match => match.PlayerId == friend.UserId && match.SubKeyId == sub_key_id
                        && match.SubGroupId == match.SubGroupId && match.PlaygroupSize == playgroup_size);
                    if (score != null)
                        scores.Add(score);
                }    
            }

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var MyStats = this.database.Scores.FirstOrDefault(match => match.PlayerId == user.UserId 
                && match.SubKeyId == sub_key_id && match.SubGroupId == match.SubGroupId && match.PlaygroupSize == playgroup_size);
            var mystats = new LeaderboardPlayer { };

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
            }

            if (sort_column == SortColumn.finish_time)
                scores.Sort((curr, prev) => prev.FinishTime.CompareTo(curr.FinishTime));
            if (sort_column == SortColumn.score)
                scores.Sort((curr, prev) => prev.Points.CompareTo(curr.Points));

            //calculating pages
            int pageEnd = per_page * page;
            int pageStart = pageEnd - per_page;
            int totalPages = scores.Count / per_page;

            if (pageEnd > scores.Count)
                pageEnd = scores.Count;

            var leaderboardPlayers = new List<LeaderboardPlayer> { };

            for (int i = pageStart; i < pageEnd; i++)
            {
                var score = scores[i];
                if (score != null)
                {
                    leaderboardPlayers.Add(new LeaderboardPlayer
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

            var resp = new Response<SubLeaderboardFriendsViewResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new SubLeaderboardFriendsViewResponse { 
                    my_stats = mystats,
                    friends_leaderboard = new leaderboard {
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
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}

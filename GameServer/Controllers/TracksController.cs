using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace GameServer.Controllers
{
    public class TracksController : Controller
    {
        private readonly Database database;

        public TracksController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("tracks/{id}/profile.xml")]
        public IActionResult GetProfile(int id) 
        {
            var Track = this.database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == id);
            var requestedBy = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            var TrackPhotos = this.database.PlayerCreations.Where(match => match.TrackId == id && match.Type == PlayerCreationType.PHOTO).ToList();
            var TrackScores = this.database.Scores.Where(match => match.SubKeyId == id).ToList();
            var TrackComments = this.database.PlayerCreationComments.Where(match => match.PlayerCreationId == id).ToList();
            var TrackReviews = this.database.PlayerCreationReviews.Where(match => match.PlayerCreationId == id).ToList();
            List<photo> PhotoList = new List<photo> { };
            List<LeaderboardPlayer> ScoresList = new List<LeaderboardPlayer> { };
            List<comment> CommentsList = new List<comment> { };
            List<review> ReviewsList = new List<review> { };

            TrackPhotos.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));
            TrackComments.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));
            TrackReviews.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));

            if (Track == null || id < 9000)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            if (Track.ScoreboardMode == 1)
                TrackScores.Sort((curr, prev) => prev.FinishTime.CompareTo(curr.FinishTime));
            else
                TrackScores.Sort((curr, prev) => prev.Points.CompareTo(curr.Points));

            if (requestedBy == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            foreach (PlayerCreationData Photo in TrackPhotos)
            {
                PhotoList.Add(new photo
                {
                    id = Photo.PlayerCreationId
                });
            }

            foreach (Score Score in TrackScores)
            {
                ScoresList.Add(new LeaderboardPlayer
                {
                    player_id = Score.PlayerId,
                    username = Score.Username,
                    rank = Score.GetRank((Track.ScoreboardMode == 1) ? SortColumn.finish_time : SortColumn.score),
                    score = Score.Points,
                    finish_time = Score.FinishTime
                });
            }

            foreach (PlayerCreationCommentData Comment in TrackComments)
            {
                if (Comment != null)
                {
                    CommentsList.Add(new comment
                    {
                        id = Comment.Id,
                        player_id = Comment.PlayerId,
                        username = Comment.Username,
                        body = Comment.Body,
                        rating_up = 0,
                        rated_by_me = false,
                        updated_at = Comment.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")
                    });
                }
            }

            foreach (PlayerCreationReview Review in TrackReviews)
            {
                if (Review != null)
                {
                    ReviewsList.Add(new review
                    {
                        id = Review.Id,
                        content = Review.Content,
                        mine = "false",
                        player_creation_id = Review.PlayerCreationId,
                        player_creation_name = Review.PlayerCreationName,
                        player_creation_username = Review.PlayerCreationUsername,
                        player_id = Review.PlayerId,
                        rated_by_me = "false",
                        rating_down = Review.RatingDown.ToString(),
                        rating_up = Review.RatingUp.ToString(),
                        username = Review.Username,
                        tags = Review.Tags,
                        updated_at = Review.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")
                    });
                }
            }

            var resp = new Response<List<track>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<track> {
                    new track
                    {
                        id = Track.PlayerCreationId,
                        ai = Track.AI,
                        associated_item_ids = Track.AssociatedItemIds,
                        auto_reset = Track.AutoReset,
                        battle_friendly_fire = Track.BattleFriendlyFire,
                        battle_kill_count = Track.BattleKillCount,
                        battle_time_limit = Track.BattleTimeLimit,
                        coolness = Track.Coolness,
                        created_at = Track.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        description = Track.Description,
                        difficulty = Track.Difficulty.ToString(),
                        dlc_keys = Track.DLCKeys,
                        downloads = Track.Downloads,
                        downloads_last_week = Track.DownloadsLastWeek,
                        downloads_this_week = Track.DownloadsThisWeek,
                        first_published = Track.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        last_published = Track.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        hearts = Track.Hearts,
                        is_remixable = Track.IsRemixable,
                        is_team_pick = Track.IsTeamPick,
                        level_mode = Track.LevelMode,
                        longest_drift = Track.LongestDrift,
                        longest_hang_time = Track.LongestHangTime,
                        max_humans = Track.MaxHumans,
                        name = Track.Name,
                        num_laps = Track.NumLaps,
                        num_racers = Track.NumRacers,
                        platform = Track.Platform.ToString(),
                        player_creation_type = (Track.Type == PlayerCreationType.STORY) ? PlayerCreationType.TRACK.ToString() : Track.Type.ToString(),
                        player_id = Track.PlayerId,
                        races_finished = Track.RacesFinished,
                        races_started = Track.RacesStarted,
                        races_started_this_month = Track.RacesStartedThisMonth,
                        races_started_this_week = Track.RacesStartedThisWeek,
                        races_won = Track.RacesWon,
                        race_type = Track.RaceType.ToString(),
                        rank = Track.Rank,
                        rating_down = Track.RatingDown,
                        rating_up = Track.RatingUp,
                        scoreboard_mode = Track.ScoreboardMode,
                        speed = Track.Speed.ToString(),
                        tags = Track.Tags,
                        track_theme = Track.TrackTheme,
                        unique_racer_count = Track.UniqueRacerCount,
                        updated_at = Track.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        username = Track.Username,
                        user_tags = Track.UserTags,
                        version = Track.Version,
                        views = Track.Views,
                        views_last_week = Track.ViewsLastWeek,
                        views_this_week = Track.ViewsThisWeek,
                        votes = Track.Votes,
                        weapon_set = Track.WeaponSet,
                        hearted_by_me = Track.IsHeartedByMe(requestedBy.UserId).ToString().ToLower(),
                        queued_by_me = Track.IsBookmarkedByMe(requestedBy.UserId).ToString().ToLower(),
                        reviewed_by_me = Track.IsReviewedByMe(requestedBy.UserId).ToString().ToLower(),
                        activities = new List<activities> { new activities { total = 0 } },
                        comments = CommentsList,
                        leaderboard = new List<leaderboard> { new leaderboard { total = TrackScores.Count, LeaderboardPlayersList = ScoresList } },
                        photos = new List<photos> { new photos { total = PhotoList.Count, PhotoList = PhotoList } },
                        reviews = new List<reviews> { new reviews { total = TrackReviews.Count, ReviewList = ReviewsList } }
                    }
                }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("tracks/{id}.xml")]
        public IActionResult Get(int id, bool is_counted)
        {
            var Track = this.database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == id);
            var User = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);

            if (Track == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            if (User != null)
            {
                if (User.UserId != Track.PlayerId)
                {
                    this.database.PlayerCreationViews.Add(new PlayerCreationView { PlayerCreationId = Track.PlayerCreationId, ViewedAt = DateTime.UtcNow });
                    this.database.SaveChanges();
                }
            }

            var resp = new Response<List<player_creation>> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_creation> {
                    new player_creation
                    {
                        id = Track.PlayerCreationId,
                        ai = Track.AI,
                        associated_item_ids = Track.AssociatedItemIds,
                        auto_reset = Track.AutoReset,
                        battle_friendly_fire = Track.BattleFriendlyFire,
                        battle_kill_count = Track.BattleKillCount,
                        battle_time_limit = Track.BattleTimeLimit,
                        coolness = Track.Coolness,
                        created_at = Track.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        description = Track.Description,
                        difficulty = Track.Difficulty.ToString(),
                        dlc_keys = Track.DLCKeys,
                        downloads = Track.Downloads,
                        downloads_last_week = Track.DownloadsLastWeek,
                        downloads_this_week = Track.DownloadsThisWeek,
                        first_published = Track.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        last_published = Track.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        hearts = Track.Hearts,
                        is_remixable = Track.IsRemixable,
                        is_team_pick = Track.IsTeamPick,
                        level_mode = Track.LevelMode,
                        longest_drift = Track.LongestDrift,
                        longest_hang_time = Track.LongestHangTime,
                        max_humans = Track.MaxHumans,
                        name = Track.Name,
                        num_laps = Track.NumLaps,
                        num_racers = Track.NumRacers,
                        platform = Track.Platform.ToString(),
                        player_creation_type = Track.Type.ToString(),
                        player_id = Track.PlayerId,
                        races_finished = Track.RacesFinished,
                        races_started = Track.RacesStarted,
                        races_started_this_month = Track.RacesStartedThisMonth,
                        races_started_this_week = Track.RacesStartedThisWeek,
                        races_won = Track.RacesWon,
                        race_type = Track.RaceType.ToString(),
                        rank = Track.Rank,
                        rating_down = Track.RatingDown,
                        rating_up = Track.RatingUp,
                        scoreboard_mode = Track.ScoreboardMode,
                        speed = Track.Speed.ToString(),
                        tags = Track.Tags,
                        track_theme = Track.TrackTheme,
                        unique_racer_count = Track.UniqueRacerCount,
                        updated_at = Track.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        username = Track.Username,
                        user_tags = Track.UserTags,
                        version = Track.Version,
                        views = Track.Views,
                        views_last_week = Track.ViewsLastWeek,
                        views_this_week = Track.ViewsThisWeek,
                        votes = Track.Votes,
                        weapon_set = Track.WeaponSet
                    }
                }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("tracks/{id}.xml")]
        public IActionResult Delete(int id)
        {
            string username = Request.Cookies["username"];
            var user = this.database.Users.FirstOrDefault(match => match.Username == username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }
            var Track = this.database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == id && match.PlayerId == user.UserId 
                && match.Type == PlayerCreationType.TRACK);

            if (Track == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            this.database.PlayerCreations.Remove(Track);
            this.database.SaveChanges();

            UserGeneratedContentUtils.RemovePlayerCreation(id);

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse {}
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("tracks/{id}/download.xml")]
        public IActionResult Download(int id, bool is_counted)
        {
            var Track = this.database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == id);
            var User = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);

            if (Track == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            if (User != null)
            {
                if (User.UserId != Track.PlayerId)
                {
                    this.database.PlayerCreationDownloads.Add(new PlayerCreationDownload { PlayerCreationId = Track.PlayerCreationId, DownloadedAt = DateTime.UtcNow });
                    this.database.SaveChanges();
                }
            }

            var resp = new Response<List<player_creation>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_creation> {
                    new player_creation
                    {
                        id = Track.PlayerCreationId,
                        ai = Track.AI,
                        associated_item_ids = Track.AssociatedItemIds,
                        auto_reset = Track.AutoReset,
                        battle_friendly_fire = Track.BattleFriendlyFire,
                        battle_kill_count = Track.BattleKillCount,
                        battle_time_limit = Track.BattleTimeLimit,
                        coolness = Track.Coolness,
                        created_at = Track.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        description = Track.Description,
                        difficulty = Track.Difficulty.ToString(),
                        dlc_keys = Track.DLCKeys,
                        downloads = Track.Downloads,
                        downloads_last_week = Track.DownloadsLastWeek,
                        downloads_this_week = Track.DownloadsThisWeek,
                        first_published = Track.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        last_published = Track.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        hearts = Track.Hearts,
                        is_remixable = Track.IsRemixable,
                        is_team_pick = Track.IsTeamPick,
                        level_mode = Track.LevelMode,
                        longest_drift = Track.LongestDrift,
                        longest_hang_time = Track.LongestHangTime,
                        max_humans = Track.MaxHumans,
                        name = Track.Name,
                        num_laps = Track.NumLaps,
                        num_racers = Track.NumRacers,
                        platform = Track.Platform.ToString(),
                        player_creation_type = Track.Type.ToString(),
                        player_id = Track.PlayerId,
                        races_finished = Track.RacesFinished,
                        races_started = Track.RacesStarted,
                        races_started_this_month = Track.RacesStartedThisMonth,
                        races_started_this_week = Track.RacesStartedThisWeek,
                        races_won = Track.RacesWon,
                        race_type = Track.RaceType.ToString(),
                        rank = Track.Rank,
                        rating_down = Track.RatingDown,
                        rating_up = Track.RatingUp,
                        scoreboard_mode = Track.ScoreboardMode,
                        speed = Track.Speed.ToString(),
                        tags = Track.Tags,
                        track_theme = Track.TrackTheme,
                        unique_racer_count = Track.UniqueRacerCount,
                        updated_at = Track.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        username = Track.Username,
                        user_tags = Track.UserTags,
                        version = Track.Version,
                        views = Track.Views,
                        views_last_week = Track.ViewsLastWeek,
                        views_this_week = Track.ViewsThisWeek,
                        votes = Track.Votes,
                        weapon_set = Track.WeaponSet,
                        auto_tags = Track.AutoTags,
                        moderation_status = "active",
                        data_size = new FileInfo($"UGC/PlayerCreations/{id}/data.bin").Length,
                        data_md5 = UserGeneratedContentUtils.CalculateMD5($"UGC/PlayerCreations/{id}/data.bin"),
                        preview_size = new FileInfo($"UGC/PlayerCreations/{id}/preview_image.png").Length,
                        preview_md5 = UserGeneratedContentUtils.CalculateMD5($"UGC/PlayerCreations/{id}/preview_image.png")
                    }
                }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("tracks/friends_published.xml")]
        public IActionResult FriendsPublished(Platform platform)
        {
            var Tracks = new List<PlayerCreationData> {};
            string usernameFilter = Request.Query["filters[username]"];

            if (usernameFilter != null)
            {
                foreach (string username in usernameFilter.Split(','))
                {
                    var user = this.database.Users.FirstOrDefault(match => match.Username == username);
                    if (user != null)
                    {
                        var userTracks = this.database.PlayerCreations.Where(match => match.PlayerId == user.UserId
                            && match.Type == PlayerCreationType.TRACK).ToList();
                        if (userTracks != null)
                            Tracks.AddRange(userTracks);
                    }
                }
            }

            bool published = (Tracks.Count != 0);

            var resp = new Response<List<player_creations>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_creations> { new player_creations { friends_published = published } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("tracks/lucky_dip.xml")]
        public IActionResult LuckyDip(int page, int per_page, string keyword, int limit, Platform platform)
        {
            var Tracks = this.database.PlayerCreations.Where(match => match.Type == PlayerCreationType.TRACK && match.Platform == platform).ToList();
            string raceTypeFilter = Request.Query["filters[race_type]"];
            string tagsFilter = Request.Query["filters[tags]"];

            //filters
            if (raceTypeFilter != null)
                Tracks = Tracks.Where(match => raceTypeFilter.Contains(match.RaceType.ToString())).ToList();

            if (tagsFilter != null)
            {
                Tracks.RemoveAll(match => match.Tags == null);
                foreach (string tag in tagsFilter.Split(','))
                {
                    Tracks.RemoveAll(match => !match.Tags.Contains(tag));
                }
            }

            //calculating pages
            int pageEnd = per_page * page;
            int pageStart = pageEnd - per_page;
            int totalPages = Tracks.Count / per_page;

            if (pageEnd > Tracks.Count)
                pageEnd = Tracks.Count;

            var playerCreationsList = new List<player_creation> { };

            for (int i = pageStart; i < pageEnd; i++)
            {
                var Track = Tracks[i];
                if (Track != null)
                {
                    playerCreationsList.Add(new player_creation
                    {
                        id = Track.PlayerCreationId,
                        ai = Track.AI,
                        associated_item_ids = Track.AssociatedItemIds,
                        auto_reset = Track.AutoReset,
                        battle_friendly_fire = Track.BattleFriendlyFire,
                        battle_kill_count = Track.BattleKillCount,
                        battle_time_limit = Track.BattleTimeLimit,
                        coolness = Track.Coolness,
                        created_at = Track.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        description = Track.Description,
                        difficulty = Track.Difficulty.ToString(),
                        dlc_keys = Track.DLCKeys,
                        downloads = Track.Downloads,
                        downloads_last_week = Track.DownloadsLastWeek,
                        downloads_this_week = Track.DownloadsThisWeek,
                        first_published = Track.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        last_published = Track.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        hearts = Track.Hearts,
                        is_remixable = Track.IsRemixable,
                        is_team_pick = Track.IsTeamPick,
                        level_mode = Track.LevelMode,
                        longest_drift = Track.LongestDrift,
                        longest_hang_time = Track.LongestHangTime,
                        max_humans = Track.MaxHumans,
                        name = Track.Name,
                        num_laps = Track.NumLaps,
                        num_racers = Track.NumRacers,
                        platform = Track.Platform.ToString(),
                        player_creation_type = Track.Type.ToString(),
                        player_id = Track.PlayerId,
                        races_finished = Track.RacesFinished,
                        races_started = Track.RacesStarted,
                        races_started_this_month = Track.RacesStartedThisMonth,
                        races_started_this_week = Track.RacesStartedThisWeek,
                        races_won = Track.RacesWon,
                        race_type = Track.RaceType.ToString(),
                        rank = Track.Rank,
                        rating_down = Track.RatingDown,
                        rating_up = Track.RatingUp,
                        scoreboard_mode = Track.ScoreboardMode,
                        speed = Track.Speed.ToString(),
                        tags = Track.Tags,
                        track_theme = Track.TrackTheme,
                        unique_racer_count = Track.UniqueRacerCount,
                        updated_at = Track.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        username = Track.Username,
                        user_tags = Track.UserTags,
                        version = Track.Version,
                        views = Track.Views,
                        views_last_week = Track.ViewsLastWeek,
                        views_this_week = Track.ViewsThisWeek,
                        votes = Track.Votes,
                        weapon_set = Track.WeaponSet
                    });
                }
            }

            var resp = new Response<List<player_creations>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_creations> {
                    new player_creations
                    {
                        page = page,
                        row_end = pageEnd,
                        row_start = pageStart,
                        total = Tracks.Count,
                        total_pages = totalPages,
                        PlayerCreationsList = playerCreationsList
                    }
                }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("tracks/ufg_picks.xml")]
        public IActionResult GetTeamPicks(int page, int per_page, SortColumn sort_column, SortOrder sort_order, string keyword, int limit, Platform platform)
        {
            var Tracks = this.database.PlayerCreations.Where(match => match.IsTeamPick == true && match.Type == PlayerCreationType.TRACK && match.Platform == platform).ToList();
            string raceTypeFilter = Request.Query["filters[race_type]"];
            string tagsFilter = Request.Query["filters[tags]"];

            //filters
            if (raceTypeFilter != null)
                Tracks = Tracks.Where(match => raceTypeFilter.Contains(match.RaceType.ToString())).ToList();

            if (tagsFilter != null)
            {
                Tracks.RemoveAll(match => match.Tags == null);
                foreach (string tag in tagsFilter.Split(','))
                {
                    Tracks.RemoveAll(match => !match.Tags.Contains(tag));
                }
            }

            //calculating pages
            int pageEnd = per_page * page;
            int pageStart = pageEnd - per_page;
            int totalPages = Tracks.Count / per_page;

            if (pageEnd > Tracks.Count)
                pageEnd = Tracks.Count;

            var playerCreationsList = new List<player_creation> { };

            for (int i = pageStart; i < pageEnd; i++)
            {
                var Track = Tracks[i];
                if (Track != null)
                {
                    playerCreationsList.Add(new player_creation
                    {
                        id = Track.PlayerCreationId,
                        ai = Track.AI,
                        associated_item_ids = Track.AssociatedItemIds,
                        auto_reset = Track.AutoReset,
                        battle_friendly_fire = Track.BattleFriendlyFire,
                        battle_kill_count = Track.BattleKillCount,
                        battle_time_limit = Track.BattleTimeLimit,
                        coolness = Track.Coolness,
                        created_at = Track.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        description = Track.Description,
                        difficulty = Track.Difficulty.ToString(),
                        dlc_keys = Track.DLCKeys,
                        downloads = Track.Downloads,
                        downloads_last_week = Track.DownloadsLastWeek,
                        downloads_this_week = Track.DownloadsThisWeek,
                        first_published = Track.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        last_published = Track.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        hearts = Track.Hearts,
                        is_remixable = Track.IsRemixable,
                        is_team_pick = Track.IsTeamPick,
                        level_mode = Track.LevelMode,
                        longest_drift = Track.LongestDrift,
                        longest_hang_time = Track.LongestHangTime,
                        max_humans = Track.MaxHumans,
                        name = Track.Name,
                        num_laps = Track.NumLaps,
                        num_racers = Track.NumRacers,
                        platform = Track.Platform.ToString(),
                        player_creation_type = Track.Type.ToString(),
                        player_id = Track.PlayerId,
                        races_finished = Track.RacesFinished,
                        races_started = Track.RacesStarted,
                        races_started_this_month = Track.RacesStartedThisMonth,
                        races_started_this_week = Track.RacesStartedThisWeek,
                        races_won = Track.RacesWon,
                        race_type = Track.RaceType.ToString(),
                        rank = Track.Rank,
                        rating_down = Track.RatingDown,
                        rating_up = Track.RatingUp,
                        scoreboard_mode = Track.ScoreboardMode,
                        speed = Track.Speed.ToString(),
                        tags = Track.Tags,
                        track_theme = Track.TrackTheme,
                        unique_racer_count = Track.UniqueRacerCount,
                        updated_at = Track.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        username = Track.Username,
                        user_tags = Track.UserTags,
                        version = Track.Version,
                        views = Track.Views,
                        views_last_week = Track.ViewsLastWeek,
                        views_this_week = Track.ViewsThisWeek,
                        votes = Track.Votes,
                        weapon_set = Track.WeaponSet
                    });
                }
            }

            var resp = new Response<List<player_creations>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_creations> {
                    new player_creations
                    {
                        page = page,
                        row_end = pageEnd,
                        row_start = pageStart,
                        total = Tracks.Count,
                        total_pages = totalPages,
                        PlayerCreationsList = playerCreationsList
                    }
                }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("tracks.xml")]
        public IActionResult Search(int page, int per_page, SortColumn sort_column, SortOrder sort_order, string keyword, int limit, Platform platform)
        {
            string idFilter = Request.Query["filters[id]"];
            string usernameFilter = Request.Query["filters[username]"];
            string raceTypeFilter = Request.Query["filters[race_type]"];
            string tagsFilter = Request.Query["filters[tags]"];
            var Tracks = new List<PlayerCreationData> { };

            if (usernameFilter == null && idFilter == null)
                Tracks = this.database.PlayerCreations.Where(match => match.Type == PlayerCreationType.TRACK && match.Platform == platform).ToList();

            //filters
            if (usernameFilter != null)
            {
                foreach (string username in usernameFilter.Split(','))
                {
                    var user = this.database.Users.FirstOrDefault(match => match.Username == username);
                    if (user != null)
                    {
                        var userTracks = this.database.PlayerCreations.Where(match => match.PlayerId == user.UserId
                            && match.Type == PlayerCreationType.TRACK && match.Platform == platform).ToList();
                        if (userTracks != null)
                            Tracks.AddRange(userTracks);
                    }
                }
            }

            if (idFilter != null)
            {
                foreach (string id in idFilter.Split(','))
                {
                    var Track = this.database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId.ToString() == id && 
                        (match.Type == PlayerCreationType.TRACK || match.Type == PlayerCreationType.STORY));
                    if (Track != null)
                        Tracks.Add(Track);
                }
            }

            if (keyword != null)
                Tracks.RemoveAll(match => !match.Name.Contains(keyword));

            if (raceTypeFilter != null)
                Tracks.RemoveAll(match => !raceTypeFilter.Contains(match.RaceType.ToString()));

            if (tagsFilter != null)
            {
                Tracks.RemoveAll(match => match.Tags == null);
                foreach (string tag in tagsFilter.Split(','))
                {
                    Tracks.RemoveAll(match => !match.Tags.Contains(tag));
                }
            }

            //cool levels
            if (sort_column == SortColumn.coolness)
                Tracks.Sort((curr, prev) => prev.Coolness.CompareTo(curr.Coolness));

            //newest levels
            if (sort_column == SortColumn.created_at)
                Tracks.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));

            //most played 
            if (sort_column == SortColumn.races_started)
                Tracks.Sort((curr, prev) => prev.RacesStarted.CompareTo(curr.RacesStarted));
            if (sort_column == SortColumn.races_started_this_week)
                Tracks.Sort((curr, prev) => prev.RacesStartedThisWeek.CompareTo(curr.RacesStartedThisWeek));
            if (sort_column == SortColumn.races_started_this_month)
                Tracks.Sort((curr, prev) => prev.RacesStartedThisMonth.CompareTo(curr.RacesStartedThisMonth));

            //highest rated
            if (sort_column == SortColumn.rating_up)
                Tracks.Sort((curr, prev) => prev.RatingUp.CompareTo(curr.RatingUp));
            if (sort_column == SortColumn.rating_up_this_week)
                Tracks.Sort((curr, prev) => prev.RatingUpThisWeek.CompareTo(curr.RatingUpThisWeek));
            if (sort_column == SortColumn.rating_up_this_month)
                Tracks.Sort((curr, prev) => prev.RatingUpThisMonth.CompareTo(curr.RatingUpThisMonth));

            //most hearted
            if (sort_column == SortColumn.hearts)
                Tracks.Sort((curr, prev) => prev.Hearts.CompareTo(curr.Hearts));
            if (sort_column == SortColumn.hearts_this_week)
                Tracks.Sort((curr, prev) => prev.HeartsThisWeek.CompareTo(curr.HeartsThisWeek));
            if (sort_column == SortColumn.hearts_this_month)
                Tracks.Sort((curr, prev) => prev.HeartsThisMonth.CompareTo(curr.HeartsThisMonth));

            //calculating pages
            int pageEnd = per_page * page;
            int pageStart = pageEnd - per_page;
            int totalPages = Tracks.Count / per_page;

            if (pageEnd > Tracks.Count)
                pageEnd = Tracks.Count;

            var playerCreationsList = new List<player_creation> { };

            for (int i = pageStart; i < pageEnd; i++)
            {
                var Track = Tracks[i];
                if (Track != null)
                {
                    playerCreationsList.Add(new player_creation
                    {
                        id = Track.PlayerCreationId,
                        ai = Track.AI,
                        associated_item_ids = Track.AssociatedItemIds,
                        auto_reset = Track.AutoReset,
                        battle_friendly_fire = Track.BattleFriendlyFire,
                        battle_kill_count = Track.BattleKillCount,
                        battle_time_limit = Track.BattleTimeLimit,
                        coolness = Track.Coolness,
                        created_at = Track.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        description = Track.Description,
                        difficulty = Track.Difficulty.ToString(),
                        dlc_keys = Track.DLCKeys,
                        downloads = Track.Downloads,
                        downloads_last_week = Track.DownloadsLastWeek,
                        downloads_this_week = Track.DownloadsThisWeek,
                        first_published = Track.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        last_published = Track.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        hearts = Track.Hearts,
                        is_remixable = Track.IsRemixable,
                        is_team_pick = Track.IsTeamPick,
                        level_mode = Track.LevelMode,
                        longest_drift = Track.LongestDrift,
                        longest_hang_time = Track.LongestHangTime,
                        max_humans = Track.MaxHumans,
                        name = Track.Name,
                        num_laps = Track.NumLaps,
                        num_racers = Track.NumRacers,
                        platform = Track.Platform.ToString(),
                        player_creation_type = Track.Type.ToString(),
                        player_id = Track.PlayerId,
                        races_finished = Track.RacesFinished,
                        races_started = Track.RacesStarted,
                        races_started_this_month = Track.RacesStartedThisMonth,
                        races_started_this_week = Track.RacesStartedThisWeek,
                        races_won = Track.RacesWon,
                        race_type = Track.RaceType.ToString(),
                        rank = Track.Rank,
                        rating_down = Track.RatingDown,
                        rating_up = Track.RatingUp,
                        scoreboard_mode = Track.ScoreboardMode,
                        speed = Track.Speed.ToString(),
                        tags = Track.Tags,
                        track_theme = Track.TrackTheme,
                        unique_racer_count = Track.UniqueRacerCount,
                        updated_at = Track.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        username = Track.Username,
                        user_tags = Track.UserTags,
                        version = Track.Version,
                        views = Track.Views,
                        views_last_week = Track.ViewsLastWeek,
                        views_this_week = Track.ViewsThisWeek,
                        votes = Track.Votes,
                        weapon_set = Track.WeaponSet
                    });
                }
            }

            var resp = new Response<List<player_creations>> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_creations> { 
                    new player_creations 
                    {
                        page = page,
                        row_end = pageEnd,
                        row_start = pageStart,
                        total = Tracks.Count,
                        total_pages = totalPages,
                        PlayerCreationsList = playerCreationsList
                    }
                }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("tracks.xml")]
        public IActionResult Create(PlayerCreation player_creation)
        {
            int id = this.database.PlayerCreations.Count() + 10000;
            string username = Request.Cookies["username"];
            var user = this.database.Users.FirstOrDefault(match => match.Username == username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            this.database.PlayerCreations.Add(new PlayerCreationData
            {
                PlayerCreationId = id,
                AI = player_creation.ai,
                AssociatedCoordinates = player_creation.associated_coordinates,
                AssociatedItemIds = player_creation.associated_item_ids,
                AssociatedUsernames = player_creation.associated_usernames,
                AutoReset = player_creation.auto_reset,
                AutoTags = player_creation.auto_tags,
                BattleFriendlyFire = player_creation.battle_friendly_fire,
                BattleKillCount = player_creation.battle_kill_count,
                BattleTimeLimit = player_creation.battle_time_limit,
                CreatedAt = DateTime.UtcNow,
                Description = player_creation.description,
                Difficulty = player_creation.difficulty,
                DLCKeys = player_creation.dlc_keys,
                FirstPublished = DateTime.UtcNow,
                IsRemixable = player_creation.is_remixable,
                IsTeamPick = player_creation.is_team_pick,
                LastPublished = DateTime.UtcNow,
                LevelMode = player_creation.level_mode,
                LongestDrift = player_creation.longest_drift,
                LongestHangTime = player_creation.longest_hang_time,
                MaxHumans = player_creation.max_humans,
                Name = player_creation.name,
                NumLaps = player_creation.num_laps,
                NumRacers = player_creation.num_racers,
                Platform = player_creation.platform,
                PlayerId = user.UserId,
                RaceType = player_creation.race_type,
                RequiresDLC = player_creation.requires_dlc,
                ScoreboardMode = player_creation.scoreboard_mode,
                Speed = player_creation.speed,
                Tags = player_creation.tags,
                TrackTheme = player_creation.track_theme,
                Type = player_creation.player_creation_type,
                UniqueRacerCount = player_creation.unique_racer_count,
                UpdatedAt = DateTime.UtcNow,
                UserTags = player_creation.user_tags,
                WeaponSet = player_creation.weapon_set,
                Votes = player_creation.votes,
                TrackId = id,
                Version = 1
            });

            this.database.SaveChanges();

            UserGeneratedContentUtils.SavePlayerCreation(id,
                   Request.Form.Files.GetFile("player_creation[data]").OpenReadStream(),
                   Request.Form.Files.GetFile("player_creation[preview]").OpenReadStream());

            var resp = new Response<List<player_creation>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_creation> { new player_creation { id = id } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("tracks/{id}/update.xml")]
        public IActionResult Update(int id, PlayerCreation player_creation)
        {
            string username = Request.Cookies["username"];
            var user = this.database.Users.FirstOrDefault(match => match.Username == username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var Track = this.database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == id && match.PlayerId == user.UserId && match.Type == PlayerCreationType.TRACK);

            if (Track == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            Track.AI = player_creation.ai;
            Track.AssociatedCoordinates = player_creation.associated_coordinates;
            Track.AssociatedItemIds = player_creation.associated_item_ids;
            Track.AssociatedUsernames = player_creation.associated_usernames;
            Track.AutoReset = player_creation.auto_reset;
            Track.AutoTags = player_creation.auto_tags;
            Track.BattleFriendlyFire = player_creation.battle_friendly_fire;
            Track.BattleKillCount = player_creation.battle_kill_count;
            Track.BattleTimeLimit = player_creation.battle_time_limit;
            Track.Description = player_creation.description;
            Track.Difficulty = player_creation.difficulty;
            Track.DLCKeys = player_creation.dlc_keys;
            Track.IsRemixable = player_creation.is_remixable;
            Track.LastPublished = DateTime.UtcNow;
            Track.LevelMode = player_creation.level_mode;
            Track.LongestDrift = player_creation.longest_drift;
            Track.LongestHangTime = player_creation.longest_hang_time;
            Track.MaxHumans = player_creation.max_humans;
            Track.Name = player_creation.name;
            Track.NumLaps = player_creation.num_laps;
            Track.NumRacers = player_creation.num_racers;
            Track.Platform = player_creation.platform;
            Track.RaceType = player_creation.race_type;
            Track.RequiresDLC = player_creation.requires_dlc;
            Track.ScoreboardMode = player_creation.scoreboard_mode;
            Track.Speed = player_creation.speed;
            Track.Tags = player_creation.tags;
            Track.TrackTheme = player_creation.track_theme;
            Track.Type = player_creation.player_creation_type;
            Track.UniqueRacerCount = player_creation.unique_racer_count;
            Track.UpdatedAt = DateTime.UtcNow;
            Track.UserTags = player_creation.user_tags;
            Track.WeaponSet = player_creation.weapon_set;
            Track.Votes = player_creation.votes;
            Track.Version++;

            this.database.SaveChanges();

            UserGeneratedContentUtils.SavePlayerCreation(id,
                   Request.Form.Files.GetFile("player_creation[data]").OpenReadStream(),
                   Request.Form.Files.GetFile("player_creation[preview]").OpenReadStream());

            var resp = new Response<List<player_creation>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_creation> { new player_creation { id = id } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
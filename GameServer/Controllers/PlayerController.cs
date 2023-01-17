using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers
{
    [Route("preferences.xml")] 
    public class PrefferencesController : Controller
    {
        [HttpPost]
        public IActionResult UpdatePreferences(ClientPreferences preference)
        {
            var resp = new Response<List<preference>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<preference> { new preference { domain = preference.domain, ip_address = HttpContext.Connection.RemoteIpAddress.ToString(), language_code = preference.language_code, region_code = preference.region_code, timezone = preference.timezone } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }

    public class AchievementsController : Controller
    {
        [Route("achievements.xml")]
        public IActionResult Get() 
        {
            var resp = new Response<List<achievements>> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<achievements> { new achievements { total = 0, AchievementList = new List<achievement> {} } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }

    public class PlayerGlickosController : Controller
    {
        [HttpGet]
        [Route("player_glickos/bulk_fetch.xml")]
        public IActionResult BulkFetch(int player_ids)
        {
            var resp = new Response<List<player_metrics>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_metrics> { new player_metrics { total = 0 } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }

    public class PrivacySettingsController : Controller
    {
        [HttpGet]
        [Route("privacy_setting.xml")]
        public IActionResult Get()
        {
            var resp = new Response<privacy_settings>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new privacy_settings { 
                    profile_acls = new profile_acls { 
                        allow_all = false,
                        allow_psn = false,
                        deny_all = true
                    }, 
                    player_creation_acls = new player_creation_acls { 
                        allow_all = false,
                        allow_psn = false,
                        deny_all = true
                    } 
                }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }

    public class FavoritePlayersController : Controller
    {
        private readonly Database database;

        public FavoritePlayersController(Database database)
        {
            this.database = database;
        }

        [HttpPost]
        [Route("favorite_players.xml")]
        public IActionResult Create(FavoritePlayer favorite_player) 
        {
            var requestedBy = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            var user = this.database.Users.FirstOrDefault(match => match.Username == favorite_player.username);

            if (user == null || requestedBy == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            if (!user.IsHeartedByMe(requestedBy.UserId))
            {
                this.database.HeartedProfiles.Add(new HeartedProfile
                {
                    HeartedUserId = user.UserId,
                    UserId = requestedBy.UserId,
                    HeartedAt = DateTime.UtcNow,
                });
                this.database.SaveChanges();
            }

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("favorite_players/remove.xml")]
        public IActionResult Remove(FavoritePlayer favorite_player) 
        {
            var user = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            int id = this.database.Users.FirstOrDefault(match => match.Username == favorite_player.username).UserId;

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var HeartedUser = this.database.HeartedProfiles.FirstOrDefault(match => match.HeartedUserId == id && match.UserId == user.UserId);

            if (HeartedUser == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            this.database.HeartedProfiles.Remove(HeartedUser);
            this.database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("favorite_players.xml")]
        public IActionResult Get(string player_id_or_username) 
        {
            int id = -1;
            Int32.TryParse(player_id_or_username, out id);
            var requestedBy = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            var user = this.database.Users.FirstOrDefault(match => match.Username == player_id_or_username || match.UserId == id);

            if (user == null || requestedBy == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var Players = new List<favorite_player> {};
            var HeartedUsers = this.database.HeartedProfiles.Where(match => match.UserId == user.UserId).ToList();

            HeartedUsers.Sort((curr, prev) => prev.HeartedAt.CompareTo(curr.HeartedAt));

            foreach (var profile in HeartedUsers)
            {
                var heartedUser = this.database.Users.FirstOrDefault(match => match.UserId == profile.HeartedUserId);
                if (heartedUser != null)
                {
                    Players.Add(new favorite_player
                    {
                        favorite_player_id = profile.HeartedUserId,
                        hearted_by_me = heartedUser.IsHeartedByMe(requestedBy.UserId) ? 1 : 0,
                        hearts = heartedUser.Hearts,
                        id = Players.Count+1,
                        quote = heartedUser.Quote,
                        total_tracks = heartedUser.TotalTracks,
                        username = heartedUser.Username
                    });
                }
            }

            var resp = new Response<List<favorite_players>> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<favorite_players> { new favorite_players { total = Players.Count, Players = Players } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }

    public class BuddiesController : Controller
    {
        [HttpPost]
        [Route("buddies/replicate.xml")]
        public IActionResult Replicate(List<string> usernames, List<string> blocked_usernames) 
        {
            var resp = new Response<EmptyResponse> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse {}
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }

    public class PlayerProfileController : Controller
    {
        private readonly Database database;

        public PlayerProfileController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("player_profile/view.xml")]
        public IActionResult ViewProfile(int player_id, Platform platform)
        {
            var user = this.database.Users.FirstOrDefault(match => match.UserId == player_id);
            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var resp = new Response<List<player_profile>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_profile> { new player_profile { player_id = user.UserId, quote = user.Quote, username = user.Username } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_profile.xml")]
        public IActionResult UpdateProfile(PlayerProfile player_profile)
        {
            int id = -130;
            string username = Request.Cookies["username"], message = "The player doesn't exist";
            var user = this.database.Users.FirstOrDefault(match => match.Username == username);
            if (user != null)
            {
                id = 0;
                message = "Successful completion";
                user.Quote = player_profile.quote;
                this.database.SaveChanges();
            }

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = id, message = message },
                response = new EmptyResponse { }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }

    public class PlayerAvatarsController : Controller
    {
        private readonly Database database;

        public PlayerAvatarsController(Database database)
        {
            this.database = database;
        }

        [HttpPost]
        [Route("player_avatars/update.xml")]
        public IActionResult Upload(PlayerAvatar player_avatar)
        {
            var user = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            if (user != null)
            {
                Stream stream = Request.Form.Files.GetFile("player_avatar[avatar]").OpenReadStream();
                UserGeneratedContentUtils.SaveAvatar(user.UserId, player_avatar, stream);
            }
            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse {}
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("player_avatars/{id}/{file}")]
        public IActionResult GetData(int id, string file)
        {
            var avatar = UserGeneratedContentUtils.LoadPlayerAvatar(id, file);
            if (avatar == null)
                return NotFound();
            return File(avatar, "image/png");
        }
    }

    public class PlanetController : Controller
    {
        private readonly Database database;

        public PlanetController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("planet.xml")]
        public IActionResult GetPlanet(int player_id, bool is_counted)
        {
            var Planet = this.database.PlayerCreations.FirstOrDefault(match => match.PlayerId == player_id && match.Type == PlayerCreationType.PLANET);

            if (Planet == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var resp = new Response<List<planet>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<planet> { new planet { id = Planet.PlayerCreationId } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("planet.xml")]
        public IActionResult UpdatePlanet(Planet planet)
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

            var Planet = this.database.PlayerCreations.FirstOrDefault(match => match.PlayerId == user.UserId && match.Type == PlayerCreationType.PLANET);
            if (Planet != null)
            {
                id = Planet.PlayerCreationId;
                Planet.Name = planet.name;
                Planet.UpdatedAt = DateTime.UtcNow;
                Planet.LastPublished = DateTime.UtcNow;
            }
            else
            {
                this.database.PlayerCreations.Add(new PlayerCreationData { 
                    PlayerCreationId = id, 
                    PlayerId = user.UserId, 
                    Name = planet.name, 
                    Type = PlayerCreationType.PLANET,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    FirstPublished = DateTime.UtcNow,
                    LastPublished = DateTime.UtcNow,
                    Version = 1
                });
            }
            this.database.SaveChanges();

            UserGeneratedContentUtils.SavePlayerCreation(id, Request.Form.Files.GetFile("planet[data]").OpenReadStream());

            var resp = new Response<List<planet>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<planet> { new planet { id = id } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("planet/profile.xml")]
        public IActionResult GetPlanetProfile(int player_id)
        {
            var Planet = this.database.PlayerCreations.FirstOrDefault(match => match.PlayerId == player_id && match.Type == PlayerCreationType.PLANET);

            if (Planet == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }
            var trackList = new List<track> {};
            var creations = this.database.PlayerCreations.Where(match => match.PlayerId == player_id && match.Type == PlayerCreationType.TRACK).ToList();
            foreach (PlayerCreationData Track in creations)
            {
                trackList.Add(new track {
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

            var resp = new Response<List<planet>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<planet> { new planet { 
                    id = Planet.PlayerCreationId,
                    name = Planet.Name,
                    player_id = Planet.PlayerId,
                    username = Planet.Username,
                    tracks = new tracks {
                        total = Planet.Author.TotalTracks,
                        TrackList = trackList
                    }
                } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }

    public class PlayerController : Controller
    {
        private readonly Database database;

        public PlayerController(Database database)
        {
            this.database = database;
        }

        [Route("players/to_id.xml")]
        public IActionResult ToID(string username) 
        {
            var user = this.database.Users.FirstOrDefault(match => match.Username == username);
            if (user == null)
            {
                var errorResp = new Response<EmptyResponse> { 
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var resp = new Response<PlayerIDResponse> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new PlayerIDResponse { player_id = user.UserId }
            };

            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [Route("players/{id}/info.xml")]
        public IActionResult GetPlayerInfo(int id, string platfom) 
        {
            var user = this.database.Users.FirstOrDefault(match => match.UserId == id);
            var requestedBy = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);

            if (user == null || requestedBy == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var resp = new Response<List<player>> {
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
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }

    public class PlayerCommmentsController : Controller
    {
        private readonly Database database;

        public PlayerCommmentsController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("player_comments.xml")]
        public IActionResult GetComments(int page, int per_page, SortColumn sort_column, SortOrder sort_order, int limit, Platform platform)
        {
            var PlayerIDFilter = Request.Query["filters[player_id]"];
            var AuthorIDFilter = Request.Query["filters[author_id]"];
            var Comments = new List<PlayerCommentData> {};
            var requestedBy = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);

            foreach (string id in PlayerIDFilter)
            {
                Comments = this.database.PlayerComments.Where(match => match.PlayerId == Int32.Parse(id)).ToList();
            }

            //sorting
            if (sort_column == SortColumn.created_at)
                Comments.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));

            var commentsList = new List<player_comment> {};

            //calculating pages
            int pageEnd = per_page * page;
            int pageStart = pageEnd - per_page;
            int totalPages = Comments.Count / per_page;

            if (pageEnd > Comments.Count)
                pageEnd = Comments.Count;

            for (int i = pageStart; i < pageEnd; i++)
            {
                var Comment = Comments[i];
                if (Comment != null)
                {
                    commentsList.Add(new player_comment
                    {
                        author_id = Comment.AuthorId,
                        author_username = Comment.AuthorUsername,
                        body = Comment.Body,
                        created_at = Comment.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        updated_at = Comment.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        id = Comment.Id,
                        platform = Comment.Platform.ToString(),
                        player_id = Comment.PlayerId,
                        username = Comment.Username,
                        rating_down = Comment.RatingDown,
                        rating_up = Comment.RatingUp,
                        rated_by_me = Comment.IsRatedByMe(requestedBy.UserId)
                    });
                }
            }

            var resp = new Response<List<player_comments>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_comments> { new player_comments {
                    page = page,
                    row_start = pageStart,
                    row_end = pageEnd,
                    total = Comments.Count,
                    total_pages = totalPages,
                    PlayerCommentList = commentsList
                } }
            };

            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_comments.xml")]
        public IActionResult Create(PlayerComment player_comment)
        {
            var author = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            var user = this.database.Users.FirstOrDefault(match => match.UserId == player_comment.player_id);

            if (user == null || author == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            this.database.PlayerComments.Add(new PlayerCommentData
            {
                AuthorId = author.UserId,
                Body = player_comment.body,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Platform = Platform.PS3,
                PlayerId = player_comment.player_id
            });
            this.database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_comments/{id}.xml")]
        public IActionResult Delete(int id)
        {
            var user = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            var comment = this.database.PlayerComments.FirstOrDefault(match => match.Id == id);

            if (user == null || comment == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            if (comment.AuthorId != user.UserId && comment.PlayerId != user.UserId)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            this.database.PlayerComments.Remove(comment);
            this.database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_comment_ratings.xml")]
        public IActionResult Rate(PlayerCommentRating player_comment_rating)
        {
            var user = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            var comment = this.database.PlayerComments.FirstOrDefault(match => match.Id == player_comment_rating.player_comment_id);

            if (user == null || comment == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var rating = this.database.PlayerCommentRatings.FirstOrDefault(match => match.PlayerCommentId == player_comment_rating.player_comment_id && match.PlayerId == user.UserId);

            if (rating == null)
            {
                this.database.PlayerCommentRatings.Add(new PlayerCommentRatingData
                {
                    PlayerCommentId = player_comment_rating.player_comment_id,
                    PlayerId = user.UserId,
                    Type = RatingType.YAY,
                    RatedAt = DateTime.UtcNow
                });
                this.database.SaveChanges();
            }

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
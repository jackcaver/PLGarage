using System;
using System.Collections.Generic;
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
    public class PlayerCreationBookmarksController : Controller
    {
        private readonly Database database;

        public PlayerCreationBookmarksController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("player_creation_bookmarks.xml")]
        public IActionResult Get(int page, int per_page, SortColumn sort_column, SortOrder sort_order, string keyword, int limit, Platform platform)
        {
            var user = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            var Creations = new List<PlayerCreationData> {};
            string raceTypeFilter = Request.Query["filters[race_type]"];
            string tagsFilterQuery = Request.Query["filters[tags]"];
            string[] tagsFilter = new string[0];

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var BookmarkedCreations = this.database.PlayerCreationBookmarks.Where(match => match.UserId == user.UserId).ToList();
            BookmarkedCreations.Sort((curr, prev) => prev.BookmarkedAt.CompareTo(curr.BookmarkedAt));

            foreach (var bookmark in BookmarkedCreations) 
            {
                Creations.Add(this.database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == bookmark.BookmarkedPlayerCreationId));
            }

            //filters
            if (raceTypeFilter != null)
                Creations = Creations.Where(match => raceTypeFilter.Contains(match.RaceType.ToString())).ToList();

            if (tagsFilterQuery != null)
            {
                tagsFilter = tagsFilterQuery.Split(',');
                Creations.RemoveAll(match => match.Tags == null);
            }

            foreach (string tag in tagsFilter)
            {
                Creations.RemoveAll(match => !match.Tags.Contains(tag));
            }

            //calculating pages
            int pageEnd = per_page * page;
            int pageStart = pageEnd - per_page;
            int totalPages = Creations.Count / per_page;

            if (pageEnd > Creations.Count)
                pageEnd = Creations.Count;

            var playerCreationsList = new List<player_creation> { };

            for (int i = pageStart; i < pageEnd; i++)
            {
                var Creation = Creations[i];
                if (Creation != null)
                {
                    playerCreationsList.Add(new player_creation
                    {
                        id = Creation.PlayerCreationId,
                        ai = Creation.AI,
                        associated_item_ids = Creation.AssociatedItemIds,
                        auto_reset = Creation.AutoReset,
                        battle_friendly_fire = Creation.BattleFriendlyFire,
                        battle_kill_count = Creation.BattleKillCount,
                        battle_time_limit = Creation.BattleTimeLimit,
                        coolness = Creation.Coolness,
                        created_at = Creation.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        description = Creation .Description,
                        difficulty = Creation.Difficulty.ToString(),
                        dlc_keys = Creation.DLCKeys,
                        downloads = Creation.Downloads,
                        downloads_last_week = Creation.DownloadsLastWeek,
                        downloads_this_week = Creation.DownloadsThisWeek,
                        first_published = Creation.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        last_published = Creation.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        hearts = Creation.Hearts,
                        is_remixable = Creation.IsRemixable,
                        is_team_pick = Creation.IsTeamPick,
                        level_mode = Creation.LevelMode,
                        longest_drift = Creation.LongestDrift,
                        longest_hang_time = Creation.LongestHangTime,
                        max_humans = Creation.MaxHumans,
                        name = Creation.Name,
                        num_laps = Creation.NumLaps,
                        num_racers = Creation.NumRacers,
                        platform = Creation.Platform.ToString(),
                        player_creation_type = Creation.Type.ToString(),
                        player_id = Creation.PlayerId,
                        races_finished = Creation.RacesFinished,
                        races_started = Creation.RacesStarted,
                        races_started_this_month = Creation.RacesStartedThisMonth,
                        races_started_this_week = Creation.RacesStartedThisWeek,
                        races_won = Creation.RacesWon,
                        race_type = Creation.RaceType.ToString(),
                        rank = Creation.Rank,
                        rating_down = Creation.RatingDown,
                        rating_up = Creation.RatingUp,
                        scoreboard_mode = Creation.ScoreboardMode,
                        speed = Creation.Speed.ToString(),
                        tags = Creation.Tags,
                        track_theme = Creation.TrackTheme,
                        unique_racer_count = Creation.UniqueRacerCount,
                        updated_at = Creation.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        username = Creation.Username,
                        user_tags = Creation.UserTags,
                        version = Creation.Version,
                        views = Creation.Views,
                        views_last_week = Creation.ViewsLastWeek,
                        views_this_week = Creation.ViewsThisWeek,
                        votes = Creation.Votes,
                        weapon_set = Creation.WeaponSet
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
                        total = Creations.Count,
                        total_pages = totalPages,
                        PlayerCreationsList = playerCreationsList
                    }
                }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_bookmarks.xml")]
        public IActionResult Create(int player_creation_id) 
        {
            var user = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            var Creation = this.database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == player_creation_id);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            if (Creation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            if (!Creation.IsBookmarkedByMe(user.UserId))
            {
                this.database.PlayerCreationBookmarks.Add(new PlayerCreationBookmark
                {
                    BookmarkedPlayerCreationId = Creation.PlayerCreationId,
                    UserId = user.UserId,
                    BookmarkedAt = DateTime.UtcNow,
                });
                this.database.SaveChanges();
            }

            var resp = new Response<EmptyResponse> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse {}
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_bookmarks/remove.xml")]
        public IActionResult Remove(int player_creation_id) 
        {
            var user = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var BookmarkedCreation = this.database.PlayerCreationBookmarks.FirstOrDefault(match => match.BookmarkedPlayerCreationId == player_creation_id && match.UserId == user.UserId);

            if (BookmarkedCreation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            this.database.PlayerCreationBookmarks.Remove(BookmarkedCreation);
            this.database.SaveChanges();

            var resp = new Response<EmptyResponse> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse {}
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [Route("player_creation_bookmarks/tally.xml")]
        public IActionResult Tally() 
        {
            var user = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var BookmarkedCreations = this.database.PlayerCreationBookmarks.Where(match => match.UserId == user.UserId).ToList();

            var resp = new Response<List<PlayerCreationBookmarks>> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<PlayerCreationBookmarks> { new PlayerCreationBookmarks { total = BookmarkedCreations.Count } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }

    public class PlayerCreationCommmentsController : Controller
    {
        private readonly Database database;

        public PlayerCreationCommmentsController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("player_creation_comments.xml")]
        public IActionResult GetComments(int page, int per_page, SortColumn sort_column, SortOrder sort_order, int limit, Platform platform)
        {
            var PlayerCreationIDFilter = Request.Query["filters[player_creation_id]"];
            var AuthorIDFilter = Request.Query["filters[player_id]"];
            var Comments = new List<PlayerCreationCommentData> { };
            var requestedBy = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);

            foreach (string id in PlayerCreationIDFilter)
            {
                Comments = this.database.PlayerCreationComments.Where(match => match.PlayerCreationId == Int32.Parse(id)).ToList();
            }

            //sorting
            if (sort_column == SortColumn.created_at)
                Comments.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));

            var CommentsList = new List<player_creation_comment> { };

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
                    CommentsList.Add(new player_creation_comment
                    {
                        body = Comment.Body,
                        created_at = Comment.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        updated_at = Comment.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        id = Comment.Id,
                        platform = Comment.Platform.ToString(),
                        player_creation_id = Comment.PlayerCreationId,
                        player_id = Comment.PlayerId,
                        username = Comment.Username,
                        rating_down = Comment.RatingDown,
                        rating_up = Comment.RatingUp,
                        rated_by_me = Comment.IsRatedByMe(requestedBy.UserId)
                    });
                }
            }

            var resp = new Response<List<player_creation_comments>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_creation_comments> { new player_creation_comments {
                    page = page,
                    row_start = pageStart,
                    row_end = pageEnd,
                    total = Comments.Count,
                    total_pages = totalPages,
                    PlayerCreationCommentList = CommentsList
                } }
            };

            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_comments.xml")]
        public IActionResult Create(PlayerCreationComment player_creation_comment)
        {
            var author = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            var Creation = this.database.Users.FirstOrDefault(match => match.UserId == player_creation_comment.player_creation_id);

            if (author == null || Creation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            this.database.PlayerCreationComments.Add(new PlayerCreationCommentData
            {
                PlayerId = author.UserId,
                Body = player_creation_comment.body,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Platform = Platform.PS3,
                PlayerCreationId = player_creation_comment.player_creation_id
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
        [Route("player_creation_comments/{id}.xml")]
        public IActionResult Delete(int id)
        {
            var user = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            var comment = this.database.PlayerCreationComments.FirstOrDefault(match => match.Id == id);

            if (user == null || comment == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var creation = this.database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == comment.PlayerCreationId);

            if (creation != null)
            {
                if (creation.PlayerId != user.UserId && comment.PlayerId != user.UserId)
                {
                    var errorResp = new Response<EmptyResponse>
                    {
                        status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                        response = new EmptyResponse { }
                    };
                    return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
                }
            }

            this.database.PlayerCreationComments.Remove(comment);
            this.database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_comment_ratings.xml")]
        public IActionResult Rate(PlayerCreationCommentRating player_creation_comment_rating)
        {
            var user = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            var comment = this.database.PlayerCreationComments.FirstOrDefault(match => match.Id == player_creation_comment_rating.player_creation_comment_id);

            if (user == null || comment == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var rating = this.database.PlayerCreationCommentRatings.FirstOrDefault(match => match.PlayerCreationCommentId == player_creation_comment_rating.player_creation_comment_id && match.PlayerId == user.UserId);

            if (rating == null)
            {
                this.database.PlayerCreationCommentRatings.Add(new PlayerCreationCommentRatingData
                {
                    PlayerCreationCommentId = player_creation_comment_rating.player_creation_comment_id,
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

    public class PlayerCreationRatingsController : Controller
    {
        private readonly Database database;

        public PlayerCreationRatingsController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("player_creation_ratings/view.xml")]
        public IActionResult View(int player_creation_id, int player_id)
        {
            var user = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            var rating = this.database.PlayerCreationRatings.FirstOrDefault(match => match.PlayerCreationId == player_creation_id && match.PlayerId == user.UserId);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var resp = new Response<List<player_creation_rating>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_creation_rating> { new player_creation_rating {
                    comments = rating != null ? rating.Comment : null,
                    rating = rating != null
                } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_ratings.xml")]
        public IActionResult Create(PlayerCreationRating player_creation_rating)
        {
            var user = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            var Creation = this.database.PlayerCreationComments.FirstOrDefault(match => match.Id == player_creation_rating.player_creation_id);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var rating = this.database.PlayerCreationRatings.FirstOrDefault(match => match.PlayerCreationId == player_creation_rating.player_creation_id && match.PlayerId == user.UserId);

            if (rating == null)
            {
                this.database.PlayerCreationRatings.Add(new PlayerCreationRatingData
                {
                    PlayerCreationId = player_creation_rating.player_creation_id,
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

        [HttpPost]
        [Route("player_creation_ratings/clear.xml")]
        public IActionResult Clear(int player_creation_id)
        {
            var user = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            var rating = this.database.PlayerCreationRatings.FirstOrDefault(match => match.PlayerId == user.UserId && match.PlayerCreationId == player_creation_id);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            this.database.PlayerCreationRatings.Remove(rating);
            this.database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }

    public class PlayerCreationReviewsController : Controller
    {
        private readonly Database database;

        public PlayerCreationReviewsController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("player_creation_reviews.xml")]
        public IActionResult List(int player_creation_id, int page, int per_page)
        {
            var user = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            var Reviews = this.database.PlayerCreationReviews.Where(match => match.PlayerCreationId == player_creation_id).ToList();

            Reviews.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var ReviewList = new List<review> {};

            //calculating pages
            int pageEnd = per_page * page;
            int pageStart = pageEnd - per_page;
            int totalPages = Reviews.Count / per_page;

            if (pageEnd > Reviews.Count)
                pageEnd = Reviews.Count;

            for (int i = pageStart; i < pageEnd; i++)
            {
                var Review = Reviews[i];
                if (Review != null)
                {
                    ReviewList.Add(new review
                    {
                        id = Review.Id,
                        content = Review.Content,
                        mine = Review.IsMine(user.UserId).ToString().ToLower(),
                        player_creation_id = Review.PlayerCreationId,
                        player_creation_name = Review.PlayerCreationName,
                        player_creation_username = Review.PlayerCreationUsername,
                        player_id = Review.PlayerId,
                        rated_by_me = Review.IsRatedByMe(user.UserId).ToString().ToLower(),
                        rating_down = Review.RatingDown.ToString(),
                        rating_up = Review.RatingUp.ToString(),
                        username = Review.Username,
                        tags = Review.Tags,
                        updated_at = Review.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")
                    });
                }
            }

            var resp = new Response<List<reviews>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<reviews> { new reviews { 
                    page = page,
                    row_start = pageStart,
                    row_end = pageEnd,
                    total_pages = totalPages,
                    total = Reviews.Count,
                    ReviewList = ReviewList
                } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("player_creation_reviews/by_player.xml")]
        public IActionResult ByPlayer(int player_id, int page, int per_page)
        {
            var requestedBy = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            var user = this.database.Users.FirstOrDefault(match => match.UserId == player_id);
            var Reviews = this.database.PlayerCreationReviews.Where(match => match.PlayerId == player_id).ToList();

            Reviews.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var ReviewList = new List<review> { };

            //calculating pages
            int pageEnd = per_page * page;
            int pageStart = pageEnd - per_page;
            int totalPages = Reviews.Count / per_page;

            if (pageEnd > Reviews.Count)
                pageEnd = Reviews.Count;

            for (int i = pageStart; i < pageEnd; i++)
            {
                var Review = Reviews[i];
                if (Review != null)
                {
                    ReviewList.Add(new review
                    {
                        id = Review.Id,
                        content = Review.Content,
                        mine = Review.IsMine(requestedBy.UserId).ToString().ToLower(),
                        player_creation_id = Review.PlayerCreationId,
                        player_creation_name = Review.PlayerCreationName,
                        player_creation_username = Review.PlayerCreationUsername,
                        player_id = Review.PlayerId,
                        rated_by_me = Review.IsRatedByMe(requestedBy.UserId).ToString().ToLower(),
                        rating_down = Review.RatingDown.ToString(),
                        rating_up = Review.RatingUp.ToString(),
                        username = Review.Username,
                        tags = Review.Tags,
                        updated_at = Review.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")
                    });
                }
            }

            var resp = new Response<List<reviews>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<reviews> { new reviews {
                    page = page,
                    row_start = pageStart,
                    row_end = pageEnd,
                    total_pages = totalPages,
                    total = Reviews.Count,
                    ReviewList = ReviewList
                } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_reviews.xml")]
        public IActionResult Create(int player_creation_id, string content, int? player_id, string tags)
        {
            var user = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            var Review = this.database.PlayerCreationReviews.FirstOrDefault(match => match.PlayerCreationId == player_creation_id && match.PlayerId == player_id);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            if (Review == null)
            {
                this.database.PlayerCreationReviews.Add(new PlayerCreationReview
                {
                    Content = content,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    PlayerId = user.UserId,
                    PlayerCreationId = player_creation_id,
                    Tags = tags
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
        [Route("player_creation_reviews/{id}.xml")]
        public IActionResult Remove(int id)
        {
            var user = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            var Review = this.database.PlayerCreationReviews.FirstOrDefault(match => match.Id == id && match.PlayerId == user.UserId);

            if (user == null || Review == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            this.database.PlayerCreationReviews.Remove(Review);
            this.database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_review_ratings.xml")]
        public IActionResult Rate(int player_creation_review_id, bool rating)
        {
            var user = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            var review = this.database.PlayerCreationReviews.FirstOrDefault(match => match.Id == player_creation_review_id);

            if (user == null || review == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var Rating = this.database.PlayerCreationReviewRatings.FirstOrDefault(match => match.PlayerCreationReviewId == player_creation_review_id && match.PlayerId == user.UserId);

            if (!rating && Rating != null)
                this.database.PlayerCreationReviewRatings.Remove(Rating);

            if (Rating == null && rating)
                this.database.PlayerCreationReviewRatings.Add(new PlayerCreationReviewRatingData
                {
                    PlayerCreationReviewId = player_creation_review_id,
                    PlayerId = user.UserId,
                    Type = RatingType.YAY,
                    RatedAt = DateTime.UtcNow
                });
            this.database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }

    public class FavoritePlayerCreationsController : Controller
    {
        private readonly Database database;

        public FavoritePlayerCreationsController(Database database)
        {
            this.database = database;
        }

        [HttpPost]
        [Route("favorite_player_creations.xml")]
        public IActionResult Create(int player_creation_id)
        {
            var user = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
            var Creation = this.database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == player_creation_id);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            if (Creation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            if (!Creation.IsHeartedByMe(user.UserId))
            {
                this.database.HeartedPlayerCreations.Add(new HeartedPlayerCreation
                {
                    HeartedPlayerCreationId = Creation.PlayerCreationId,
                    UserId = user.UserId,
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
        [Route("favorite_player_creations/{id}.xml")]
        public IActionResult Remove(int id)
        {
            var user = this.database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var HeartedCreation = this.database.HeartedPlayerCreations.FirstOrDefault(match => match.HeartedPlayerCreationId == id && match.UserId == user.UserId);

            if (HeartedCreation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            this.database.HeartedPlayerCreations.Remove(HeartedCreation);
            this.database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("favorite_player_creations.xml")]
        public IActionResult Get(string player_id_or_username)
        {
            int id = -1;
            Int32.TryParse(player_id_or_username, out id);
            var user = this.database.Users.FirstOrDefault(match => match.Username == player_id_or_username || match.UserId == id);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var favoriteCrations = this.database.HeartedPlayerCreations.Where(match => match.UserId == user.UserId).ToList();
            List<favorite_player_creation> favoriteCreationsList = new List<favorite_player_creation> {};

            foreach (var Creation in favoriteCrations)
            {
                var creation = this.database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == Creation.HeartedPlayerCreationId);
                if (creation != null)
                {
                    favoriteCreationsList.Add(new favorite_player_creation
                    {
                        player_creation_id = Creation.HeartedPlayerCreationId,
                        player_creation_name = creation.Name
                    });
                }
            }

            var resp = new Response<List<favorite_player_creations>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<favorite_player_creations> { new favorite_player_creations { 
                    total = favoriteCreationsList.Count, 
                    PlayerCreations = favoriteCreationsList 
                } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }

    public class PhotosController : Controller
    {
        private readonly Database database;

        public PhotosController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("photos/search.xml")]
        public IActionResult Search(int? track_id, string username, string associated_usernames, int page, int per_page)
        {
            List<PlayerCreationData> Photos = new List<PlayerCreationData> {};
            int player_id = 0;
            var user = this.database.Users.FirstOrDefault(match => match.Username == username);
            if (user != null)
                player_id = user.UserId;

            if (associated_usernames != null)
                Photos = this.database.PlayerCreations.Where(match => match.Type == PlayerCreationType.PHOTO && match.AssociatedUsernames.Contains(associated_usernames)).ToList();
            if (username != null)
                Photos = this.database.PlayerCreations.Where(match => match.Type == PlayerCreationType.PHOTO && match.PlayerId == player_id).ToList();
            if (track_id != null)
                Photos = this.database.PlayerCreations.Where(match => match.Type == PlayerCreationType.PHOTO && match.TrackId == track_id).ToList();
            if (track_id != null && username != null)
                Photos = this.database.PlayerCreations.Where(match => match.Type == PlayerCreationType.PHOTO && match.TrackId == track_id && match.PlayerId == player_id).ToList();

            Photos.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));

            int pageEnd = per_page * page;
            int pageStart = pageEnd-per_page;
            int totalPages = Photos.Count/per_page;

            if (pageEnd > Photos.Count)
                pageEnd = Photos.Count;

            var PhotoList = new List<photo> {};
            for (int i = pageStart; i < pageEnd; i++)
            {
                var Photo = Photos[i];
                PhotoList.Add(new photo { 
                    associated_usernames = Photo.AssociatedUsernames,
                    id = Photo.PlayerCreationId,
                    track_id = Photo.TrackId,
                    username = Photo.Username
                });
            }

            var resp = new Response<List<photos>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<photos> {
                    new photos { 
                        total = Photos.Count, 
                        current_page = page,
                        row_start = pageStart,
                        row_end = pageEnd,
                        total_pages = totalPages,
                        PhotoList = PhotoList
                    }
                }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("photos/{id}.xml")]
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
            var Photo = this.database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == id && match.PlayerId == user.UserId && match.Type == PlayerCreationType.PHOTO);

            if (Photo == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            this.database.PlayerCreations.Remove(Photo);
            this.database.SaveChanges();

            UserGeneratedContentUtils.RemovePlayerCreation(id);

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("photos/create.xml")]
        public IActionResult Create(Photo photo)
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
                CreatedAt = DateTime.UtcNow,
                FirstPublished = DateTime.UtcNow,
                LastPublished = DateTime.UtcNow,
                PlayerId = user.UserId,
                UpdatedAt = DateTime.UtcNow,
                Name = photo.name,
                Description = photo.description,
                Platform = photo.platform,
                AssociatedUsernames = photo.associated_usernames,
                AssociatedCoordinates = photo.associated_coordinates,
                TrackId = photo.track_id,
                Type = PlayerCreationType.PHOTO,
                Version = 1
            });

            this.database.SaveChanges();

            UserGeneratedContentUtils.SavePlayerPhoto(id, Request.Form.Files.GetFile("photo[data]").OpenReadStream());

            var resp = new Response<List<player_creation>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_creation> { new player_creation { id = id } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }

    public class PlayerCreationsController : Controller
    {
        private readonly Database database;

        public PlayerCreationsController(Database database)
        {
            this.database = database;
        }

        [HttpPost]
        [Route("player_creations/verify.xml")]
        public IActionResult Verify(List<int> id, List<int> offline_id) 
        {
            List<PlayerCreationToVerify> creations = new List<PlayerCreationToVerify> {};        
            foreach (int item in id)
            {
                var creation = this.database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == item);
                if (creation != null)
                {
                    creations.Add(new PlayerCreationToVerify
                    {
                        id = item,
                        type = creation.Type.ToString(),
                        suggested_action = "allow"
                    });
                }
            }
            var resp = new Response<List<PlayerCreationVerify>> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<PlayerCreationVerify> {
                    new PlayerCreationVerify { total = creations.Count, PlayerCreationsList = creations }
                }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("player_creations/{id}/{file}")]
        public IActionResult GetData(int id, string file)
        {
            var data = UserGeneratedContentUtils.LoadPlayerCreation(id, file);
            if (data == null)
                return NotFound();
            string contentType = "application/octet-stream";
            if (file.EndsWith(".png"))
                contentType = "image/png";
            if (file.EndsWith(".jpg"))
                contentType = "image/jpeg";
            return File(data, contentType);
        }
    }
}
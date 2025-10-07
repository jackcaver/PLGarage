using GameServer.Models.Response;
using GameServer.Models;
using System.Collections.Generic;
using GameServer.Utils;
using System.Linq;
using GameServer.Models.PlayerData.PlayerCreations;
using System;
using GameServer.Models.PlayerData;
using GameServer.Implementation.Common;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Implementation.Player_Creation
{
    public class PlayerCreationReviews
    {
        public static string ListReviews(Database database, Guid SessionID, int player_creation_id, int page, int per_page, int player_id = 0, bool byPlayer = false)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var reviewsQuery = database.PlayerCreationReviews
                .AsSplitQuery()
                .Include(r => r.User)
                .Include(r => r.Creation)
                .ThenInclude(c => c.Author)
                .Include(r => r.ReviewRatings)
                .AsQueryable();

            reviewsQuery = byPlayer ? reviewsQuery.Where(match => match.PlayerId == player_id) :
                reviewsQuery.Where(match => match.PlayerCreationId == player_creation_id);

            reviewsQuery = reviewsQuery.OrderBy(r => r.CreatedAt);

            if (byPlayer && !database.Users.Any(match => match.UserId == player_id))
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var ReviewList = new List<Review> { };

            var total = reviewsQuery.Count();

            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, total);

            if (pageEnd > total)
                pageEnd = total;

            var reviews = reviewsQuery.Skip(pageStart).Take(per_page).ToList();

            foreach (var review in reviews)
            {
                ReviewList.Add(new Review
                {
                    id = review.Id,
                    content = review.Content,
                    mine = user != null ? review.IsMine(user.UserId).ToString().ToLower() : "false",
                    player_creation_id = review.PlayerCreationId,
                    player_creation_name = review.PlayerCreationName,
                    player_creation_username = review.PlayerCreationUsername,
                    player_id = review.PlayerId,
                    rated_by_me = user != null ? review.IsRatedByMe(user.UserId).ToString().ToLower() : "false",
                    rating_down = review.RatingDown.ToString(),
                    rating_up = review.RatingUp.ToString(),
                    username = review.Username,
                    tags = review.Tags,
                    updated_at = review.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")
                });
            }

            var resp = new Response<List<Reviews>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new Reviews {
                    page = page,
                    row_start = pageStart,
                    row_end = pageEnd,
                    total_pages = totalPages,
                    total = total,
                    ReviewList = ReviewList
                } ]
            };
            return resp.Serialize();
        }

        public static string CreateReview(Database database, Guid SessionID, int player_creation_id, string content, int? player_id, string tags)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (user == null || !database.PlayerCreations.Any(match => match.PlayerCreationId == player_creation_id))
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var Review = database.PlayerCreationReviews.FirstOrDefault(match => match.PlayerCreationId == player_creation_id && match.PlayerId == user.UserId);

            if (Review == null)
            {
                database.PlayerCreationReviews.Add(new PlayerCreationReview
                {
                    Content = content,
                    CreatedAt = TimeUtils.Now,
                    UpdatedAt = TimeUtils.Now,
                    PlayerId = user.UserId,
                    PlayerCreationId = player_creation_id,
                    Tags = tags
                });
                database.SaveChanges();
                if (!session.IsMNR)
                {
                    database.ActivityLog.Add(new ActivityEvent
                    {
                        AuthorId = user.UserId,
                        Type = ActivityType.player_creation_event,
                        List = ActivityList.activity_log,
                        Topic = "player_creation_reviewed",
                        Description = content,
                        PlayerCreationId = player_creation_id,
                        CreatedAt = TimeUtils.Now,
                        AllusionId = database.PlayerCreationReviews.OrderBy(e => e.CreatedAt).LastOrDefault(match => match.PlayerCreationId == player_creation_id && match.PlayerId == user.UserId).Id,
                        AllusionType = "PlayerCreation::Review",
                        Tags = tags
                    });
                    database.SaveChanges();
                }
            }
            else
            {
                Review.UpdatedAt = TimeUtils.Now;
                Review.Tags = tags;
                Review.Content = content;
                database.SaveChanges();
            }

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static string RemoveReview(Database database, Guid SessionID, int id)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var Review = database.PlayerCreationReviews.FirstOrDefault(match => match.Id == id && match.PlayerId == user.UserId);

            if (user == null || Review == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.PlayerCreationReviews.Remove(Review);
            database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static string RateReview(Database database, Guid SessionID, int id, bool rating)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (user == null || !database.PlayerCreationReviews.Any(match => match.Id == id))
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var Rating = database.PlayerCreationReviewRatings.FirstOrDefault(match => match.PlayerCreationReviewId == id && match.PlayerId == user.UserId);

            if (!rating && Rating != null)
                database.PlayerCreationReviewRatings.Remove(Rating);

            if (Rating == null && rating)
                database.PlayerCreationReviewRatings.Add(new PlayerCreationReviewRatingData
                {
                    PlayerCreationReviewId = id,
                    PlayerId = user.UserId,
                    Type = RatingType.YAY,
                    RatedAt = TimeUtils.Now
                });
            database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }
    }
}

using GameServer.Models.Response;
using GameServer.Models;
using System.Collections.Generic;
using GameServer.Utils;
using System.Linq;
using GameServer.Models.PlayerData.PlayerCreations;
using System;
using GameServer.Models.PlayerData;
using GameServer.Implementation.Common;
using GameServer.Models.Common;
using GameServer.Utils.Extensions;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Implementation.Player_Creation
{
    public class PlayerCreationReviewsImpl
    {
        public static string ListReviews(Database database, Guid SessionID, int player_creation_id, int page, int per_page, int player_id = 0, bool byPlayer = false)
        {
            var session = SessionImpl.GetSession(SessionID);
            var requestedBy = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (byPlayer && !database.Users.Any(match => match.UserId == player_id))
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var reviewsQuery = database.PlayerCreationReviews
                .Include(x => x.User)
                .Include(x => x.Creation)
                .Include(x => x.Creation.Author)
                .Include(x => x.ReviewRatings)
                .Where(match => byPlayer ? match.User.UserId == player_id : match.Creation.Id == player_creation_id)
                .OrderByDescending(match => match.CreatedAt);

            //calculating pages
            var pageStart = PageCalculator.GetPageStart(page, per_page);
            var pageEnd = PageCalculator.GetPageStart(page, per_page);
            var total = reviewsQuery.Count();
            var totalPages = PageCalculator.GetTotalPages(total, per_page);

            var reviews = reviewsQuery
                .Take(pageStart)
                .Skip(per_page)
                .ProjectTo<Review>(database.MapperConfig, new { requestedBy })
                .ToList();

            var resp = new Response<List<Reviews>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new Reviews {
                    Page = page,
                    RowStart = pageStart,
                    RowEnd = pageEnd,
                    TotalPages = totalPages,
                    Total = total,
                    ReviewList = reviews
                } ]
            };
            return resp.Serialize();
        }

        public static string CreateReview(Database database, Guid SessionID, int player_creation_id, string content, int? player_id, string tags)
        {
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (user == null || !database.PlayerCreations.Any(match => match.Id == player_creation_id))
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var creation = database.PlayerCreations
                .FirstOrDefault(match => match.Id == player_creation_id);
            var review = database.PlayerCreationReviews
                .Include(x => x.User)
                .Include(x => x.Creation)
                .FirstOrDefault(match => match.Creation.Id == player_creation_id && match.User.UserId == user.UserId);

            if (review == null)
            {
                var newReview = new PlayerCreationReview
                {
                    Content = content,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    User = user,
                    Creation = creation,
                    Tags = tags
                };
                database.PlayerCreationReviews.Add(newReview);
                // TODO: dbsave needed?
                database.ActivityLog.Add(new ActivityEvent
                {
                    Author = user,
                    Type = ActivityType.player_creation_event,
                    List = ActivityList.activity_log,
                    Topic = "player_creation_reviewed",
                    Description = content,
                    Creation = creation,
                    CreatedAt = DateTime.UtcNow,
                    AllusionId = newReview.Id,
                    AllusionType = "PlayerCreation::Review",
                    Tags = tags
                });
                database.SaveChanges();
            }
            else
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
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
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            var review = database.PlayerCreationReviews.FirstOrDefault(match => match.Id == id && match.User.UserId == user.UserId);

            if (user == null || review == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.PlayerCreationReviews.Remove(review);
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
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            var review = database.PlayerCreationReviews.FirstOrDefault(match => match.Id == id);

            if (user == null || review == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var dbRating = database.PlayerCreationReviewRatings
                .Include(x => x.Player)
                .Include(x => x.Review)
                .FirstOrDefault(match => match.Review.Id == id && match.Player.UserId == user.UserId);

            if (!rating && dbRating != null)
                database.PlayerCreationReviewRatings.Remove(dbRating);

            if (dbRating == null && rating)
                database.PlayerCreationReviewRatings.Add(new PlayerCreationReviewRatingData
                {
                    Review = review,
                    Player = user,
                    Type = RatingType.YAY,
                    RatedAt = DateTime.UtcNow
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

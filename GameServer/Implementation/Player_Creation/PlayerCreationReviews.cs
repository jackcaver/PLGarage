using GameServer.Models.Response;
using GameServer.Models;
using System.Collections.Generic;
using GameServer.Utils;
using System.Linq;
using GameServer.Models.PlayerData.PlayerCreations;
using System;
using GameServer.Models.PlayerData;
using GameServer.Implementation.Common;

namespace GameServer.Implementation.Player_Creation
{
    public class PlayerCreationReviews
    {
        public static string ListReviews(Database database, Guid SessionID, int player_creation_id, int page, int per_page, int player_id = 0, bool byPlayer = false)
        {
            var session = Session.GetSession(SessionID);
            var requestedBy = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var user = database.Users.FirstOrDefault(match => match.UserId == player_id);
            var Reviews = byPlayer ? database.PlayerCreationReviews.Where(match => match.PlayerId == player_id).ToList() :
                database.PlayerCreationReviews.Where(match => match.PlayerCreationId == player_creation_id).ToList();

            Reviews.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));

            if (requestedBy == null || byPlayer && user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var ReviewList = new List<review> { };

            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, Reviews.Count);

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
            return resp.Serialize();
        }

        public static string CreateReview(Database database, Guid SessionID, int player_creation_id, string content, int? player_id, string tags)
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

            var Review = database.PlayerCreationReviews.FirstOrDefault(match => match.PlayerCreationId == player_creation_id && match.PlayerId == user.UserId);

            if (Review == null)
            {
                database.PlayerCreationReviews.Add(new PlayerCreationReview
                {
                    Content = content,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    PlayerId = user.UserId,
                    PlayerCreationId = player_creation_id,
                    Tags = tags
                });
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

            var Rating = database.PlayerCreationReviewRatings.FirstOrDefault(match => match.PlayerCreationReviewId == id && match.PlayerId == user.UserId);

            if (!rating && Rating != null)
                database.PlayerCreationReviewRatings.Remove(Rating);

            if (Rating == null && rating)
                database.PlayerCreationReviewRatings.Add(new PlayerCreationReviewRatingData
                {
                    PlayerCreationReviewId = id,
                    PlayerId = user.UserId,
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

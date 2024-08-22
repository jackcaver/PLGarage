using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Models;
using GameServer.Utils;
using System.Collections.Generic;
using System.Linq;
using System;
using GameServer.Implementation.Common;

namespace GameServer.Implementation.Player
{
    public class PlayerComments
    {
        public static string ListComments(Database database, Guid SessionID, int page, int per_page, int limit, SortColumn sort_column,
            Platform platform, string PlayerIDFilter, string AuthorIDFilter)
        {
            var Comments = new List<PlayerCommentData> { };
            var session = Session.GetSession(SessionID);
            var requestedBy = database.Users.FirstOrDefault(match => match.Username == session.Username);

            foreach (string id in PlayerIDFilter.Split(','))
            {
                Comments.AddRange(database.PlayerComments.Where(match => match.PlayerId.ToString() == id).ToList());
            }

            //sorting
            if (sort_column == SortColumn.created_at)
                Comments.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));

            var commentsList = new List<player_comment> { };

            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, Comments.Count);

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
                        rated_by_me = requestedBy != null ? Comment.IsRatedByMe(requestedBy.UserId) : false
                    });
                }
            }

            var resp = new Response<List<player_comments>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new player_comments {
                    page = page,
                    row_start = pageStart,
                    row_end = pageEnd,
                    total = Comments.Count,
                    total_pages = totalPages,
                    PlayerCommentList = commentsList
                } ]
            };

            return resp.Serialize();
        }

        public static string CreateComment(Database database, Guid SessionID, PlayerComment player_comment)
        {
            var session = Session.GetSession(SessionID);
            var author = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var user = database.Users.FirstOrDefault(match => match.UserId == player_comment.player_id);

            if (user == null || author == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.PlayerComments.Add(new PlayerCommentData
            {
                AuthorId = author.UserId,
                Body = player_comment.body,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Platform = Platform.PS3,
                PlayerId = player_comment.player_id
            });
            database.SaveChanges();
            database.ActivityLog.Add(new ActivityEvent
            {
                AuthorId = author.UserId,
                Type = ActivityType.player_event,
                List = ActivityList.activity_log,
                Topic = "player_authored_comment",
                Description = player_comment.body,
                PlayerId = user.UserId,
                PlayerCreationId = 0,
                CreatedAt = DateTime.UtcNow,
                AllusionId = database.PlayerComments.OrderBy(e => e.CreatedAt).LastOrDefault(match => match.AuthorId == author.UserId && match.PlayerId == user.UserId).Id,
                AllusionType = "PlayerComment"
            });
            database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static string RemoveComment(Database database, Guid SessionID, int id)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var comment = database.PlayerComments.FirstOrDefault(match => match.Id == id);

            if (user == null || comment == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (comment.AuthorId != user.UserId && comment.PlayerId != user.UserId)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.PlayerComments.Remove(comment);
            database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static string RateComment(Database database, Guid SessionID, PlayerCommentRating player_comment_rating)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var comment = database.PlayerComments.FirstOrDefault(match => match.Id == player_comment_rating.player_comment_id);

            if (user == null || comment == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var rating = database.PlayerCommentRatings.FirstOrDefault(match => match.PlayerCommentId == player_comment_rating.player_comment_id
                && match.PlayerId == user.UserId);

            if (rating == null)
            {
                database.PlayerCommentRatings.Add(new PlayerCommentRatingData
                {
                    PlayerCommentId = player_comment_rating.player_comment_id,
                    PlayerId = user.UserId,
                    Type = RatingType.YAY,
                    RatedAt = DateTime.UtcNow
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
    }
}

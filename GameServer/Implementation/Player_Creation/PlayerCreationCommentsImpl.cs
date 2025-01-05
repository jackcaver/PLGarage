using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Models;
using System.Collections.Generic;
using GameServer.Utils;
using GameServer.Models.PlayerData;
using System.Linq;
using System;
using GameServer.Implementation.Common;
using GameServer.Models.Common;

namespace GameServer.Implementation.Player_Creation
{
    public class PlayerCreationCommentsImpl
    {
        public static string ListComments(Database database, Guid SessionID, int page, int per_page, SortColumn sort_column, SortOrder sort_order, int limit, Platform platform, string PlayerCreationIDFilter, string AuthorIDFilter)
        {
            var Comments = new List<PlayerCreationCommentData> { };
            var session = SessionImpl.GetSession(SessionID);
            var requestedBy = database.Users.FirstOrDefault(match => match.Username == session.Username);

            foreach (string id in PlayerCreationIDFilter.Split(','))
            {
                Comments = database.PlayerCreationComments.Where(match => match.PlayerCreationId == int.Parse(id)).ToList();
            }

            //sorting
            if (sort_column == SortColumn.created_at)
                Comments.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));

            var CommentsList = new List<player_creation_comment> { };

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
                        rated_by_me = requestedBy != null ? Comment.IsRatedByMe(requestedBy.UserId) : false
                    });
                }
            }

            var resp = new Response<List<player_creation_comments>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new player_creation_comments {
                    page = page,
                    row_start = pageStart,
                    row_end = pageEnd,
                    total = Comments.Count,
                    total_pages = totalPages,
                    PlayerCreationCommentList = CommentsList
                } ]
            };

            return resp.Serialize();
        }

        public static string CreateComment(Database database, Guid SessionID, PlayerCreationComment player_creation_comment)
        {
            var session = SessionImpl.GetSession(SessionID);
            var author = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var Creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == player_creation_comment.player_creation_id);

            if (author == null || Creation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.PlayerCreationComments.Add(new PlayerCreationCommentData
            {
                PlayerId = author.UserId,
                Body = player_creation_comment.body,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Platform = Platform.PS3,
                PlayerCreationId = player_creation_comment.player_creation_id
            });
            database.SaveChanges();
            database.ActivityLog.Add(new ActivityEvent
            {
                AuthorId = author.UserId,
                Type = ActivityType.player_creation_event,
                List = ActivityList.activity_log,
                Topic = "player_creation_commented_on",
                Description = player_creation_comment.body,
                PlayerId = 0,
                PlayerCreationId = Creation.PlayerCreationId,
                CreatedAt = DateTime.UtcNow,
                AllusionId = database.PlayerCreationComments.OrderBy(e => e.CreatedAt).LastOrDefault(match => match.PlayerCreationId == Creation.PlayerCreationId && match.PlayerId == author.UserId).Id,
                AllusionType = "PlayerCreation::Comment"
            });
            database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static string DeleteComment(Database database, Guid SessionID, int id)
        {
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var comment = database.PlayerCreationComments.FirstOrDefault(match => match.Id == id);

            if (user == null || comment == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == comment.PlayerCreationId);

            if (creation != null)
            {
                if (creation.PlayerId != user.UserId && comment.PlayerId != user.UserId)
                {
                    var errorResp = new Response<EmptyResponse>
                    {
                        status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                        response = new EmptyResponse { }
                    };
                    return errorResp.Serialize();
                }
            }

            database.PlayerCreationComments.Remove(comment);
            database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static string RateComment(Database database, Guid SessionID, PlayerCreationCommentRating player_creation_comment_rating)
        {
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var comment = database.PlayerCreationComments.FirstOrDefault(match => match.Id == player_creation_comment_rating.player_creation_comment_id);

            if (user == null || comment == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var rating = database.PlayerCreationCommentRatings.FirstOrDefault(match =>
                match.PlayerCreationCommentId == player_creation_comment_rating.player_creation_comment_id && match.PlayerId == user.UserId);

            if (rating == null)
            {
                database.PlayerCreationCommentRatings.Add(new PlayerCreationCommentRatingData
                {
                    PlayerCreationCommentId = player_creation_comment_rating.player_creation_comment_id,
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

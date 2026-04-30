using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Models;
using GameServer.Utils;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Implementation.Player
{
    public class PlayerComments
    {
        public static string ListComments(Database database, User requestedBy, int page, int per_page, int limit, SortColumn sort_column,
            Platform platform, string PlayerIDFilter, string AuthorIDFilter)
        {
            var playerIDs = PlayerIDFilter.Split(',');

            var commentsQuery = database.PlayerComments
                    .AsSplitQuery()
                    .Include(c => c.Author)
                    .Include(c => c.Player)
                    .Include(c => c.CommentRating)
                    .Where(match => playerIDs.Contains(match.PlayerId.ToString()));

            //sorting
            if (sort_column == SortColumn.created_at)
                commentsQuery = commentsQuery.OrderBy(c => c.CreatedAt);

            var commentsList = new List<player_comment> { };

            var total = commentsQuery.Count();

            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, total);

            if (pageEnd > total)
                pageEnd = total;

            var comments = commentsQuery.Skip(pageStart).Take(per_page).ToList();

            foreach (var comment in comments)
            {
                commentsList.Add(new player_comment
                {
                    author_id = comment.AuthorId,
                    author_username = comment.AuthorUsername,
                    body = comment.Body,
                    created_at = comment.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    updated_at = comment.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    id = comment.Id,
                    platform = comment.Platform.ToString(),
                    player_id = comment.PlayerId,
                    username = comment.Username,
                    rating_down = comment.RatingDown,
                    rating_up = comment.RatingUp,
                    rated_by_me = requestedBy != null ? comment.IsRatedByMe(requestedBy.UserId) : false
                });
            }

            var resp = new Response<List<player_comments>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new player_comments {
                    page = page,
                    row_start = pageStart,
                    row_end = pageEnd,
                    total = total,
                    total_pages = totalPages,
                    PlayerCommentList = commentsList
                } ]
            };

            return resp.Serialize();
        }

        public static string CreateComment(Database database, SessionData session, PlayerComment player_comment)
        {
            var author = session.User;
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

            var comment = new PlayerCommentData
            {
                AuthorId = author.UserId,
                Body = player_comment.body,
                CreatedAt = TimeUtils.Now,
                UpdatedAt = TimeUtils.Now,
                Platform = Platform.PS3,
                PlayerId = player_comment.player_id
            };
            database.PlayerComments.Add(comment);
            database.SaveChanges();

            if (!session.IsMNR)
            {
                database.ActivityLog.Add(new ActivityEvent
                {
                    AuthorId = author.UserId,
                    Type = ActivityType.player_event,
                    List = ActivityList.activity_log,
                    Topic = "player_authored_comment",
                    Description = player_comment.body,
                    PlayerId = user.UserId,
                    PlayerCreationId = 0,
                    CreatedAt = TimeUtils.Now,
                    AllusionId = comment.Id,
                    AllusionType = "PlayerComment"
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

        public static string RemoveComment(Database database, User user, int id)
        {
            var comment = database.PlayerComments.FirstOrDefault(match => match.Id == id);

            if (user == null || comment == null 
                || (comment.AuthorId != user.UserId && comment.PlayerId != user.UserId))
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

        public static string RateComment(Database database, User user, PlayerCommentRating player_comment_rating)
        {
            if (user == null 
                || !database.PlayerComments.Any(match => match.Id == player_comment_rating.player_comment_id))
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }


            if (!database.PlayerCommentRatings.Any(match => match.PlayerCommentId == player_comment_rating.player_comment_id
                && match.PlayerId == user.UserId))
            {
                database.PlayerCommentRatings.Add(new PlayerCommentRatingData
                {
                    PlayerCommentId = player_comment_rating.player_comment_id,
                    PlayerId = user.UserId,
                    Type = RatingType.YAY,
                    RatedAt = TimeUtils.Now
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

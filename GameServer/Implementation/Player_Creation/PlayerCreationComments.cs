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
using Microsoft.EntityFrameworkCore;

namespace GameServer.Implementation.Player_Creation
{
    public class PlayerCreationComments
    {
        public static string ListComments(Database database, Guid SessionID, int page, int per_page, SortColumn sort_column, SortOrder sort_order, int limit, Platform platform, string PlayerCreationIDFilter, string AuthorIDFilter)
        {
            var session = Session.GetSession(SessionID);
            var requestedBy = database.Users.FirstOrDefault(match => match.Username == session.Username);

            var creationIDs = PlayerCreationIDFilter.Split(',');

            var commentsQuery = database.PlayerCreationComments
                .AsSplitQuery()
                .Include(c => c.Player)
                .Include(c => c.CommentRatings)
                .Where(match => creationIDs.Contains(match.PlayerCreationId.ToString()));

            //sorting
            if (sort_column == SortColumn.created_at)
                commentsQuery = commentsQuery.OrderBy(c => c.CreatedAt);

            var CommentsList = new List<player_creation_comment> { };

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
                CommentsList.Add(new player_creation_comment
                {
                    body = comment.Body,
                    created_at = comment.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    updated_at = comment.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    id = comment.Id,
                    platform = comment.Platform.ToString(),
                    player_creation_id = comment.PlayerCreationId,
                    player_id = comment.PlayerId,
                    username = comment.Username,
                    rating_down = comment.RatingDown,
                    rating_up = comment.RatingUp,
                    rated_by_me = requestedBy != null ? comment.IsRatedByMe(requestedBy.UserId) : false
                });
            }

            var resp = new Response<List<player_creation_comments>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new player_creation_comments {
                    page = page,
                    row_start = pageStart,
                    row_end = pageEnd,
                    total = total,
                    total_pages = totalPages,
                    PlayerCreationCommentList = CommentsList
                } ]
            };

            return resp.Serialize();
        }

        public static string CreateComment(Database database, Guid SessionID, PlayerCreationComment player_creation_comment)
        {
            var session = Session.GetSession(SessionID);
            var author = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (author == null || !database.PlayerCreations.Any(match => match.PlayerCreationId == player_creation_comment.player_creation_id))
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
                CreatedAt = TimeUtils.Now,
                UpdatedAt = TimeUtils.Now,
                Platform = Platform.PS3,
                PlayerCreationId = player_creation_comment.player_creation_id
            });
            database.SaveChanges();
            if (!session.IsMNR)
            {
                database.ActivityLog.Add(new ActivityEvent
                {
                    AuthorId = author.UserId,
                    Type = ActivityType.player_creation_event,
                    List = ActivityList.activity_log,
                    Topic = "player_creation_commented_on",
                    Description = player_creation_comment.body,
                    PlayerId = 0,
                    PlayerCreationId = player_creation_comment.player_creation_id,
                    CreatedAt = TimeUtils.Now,
                    AllusionId = database.PlayerCreationComments.OrderBy(e => e.CreatedAt).LastOrDefault(match => match.PlayerCreationId == player_creation_comment.player_creation_id && match.PlayerId == author.UserId).Id,
                    AllusionType = "PlayerCreation::Comment"
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

        public static string DeleteComment(Database database, Guid SessionID, int id)
        {
            var session = Session.GetSession(SessionID);
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
            var session = Session.GetSession(SessionID);
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

            if (!database.PlayerCreationCommentRatings.Any(match =>
                    match.PlayerCreationCommentId == player_creation_comment_rating.player_creation_comment_id 
                    && match.PlayerId == user.UserId))
            {
                database.PlayerCreationCommentRatings.Add(new PlayerCreationCommentRatingData
                {
                    PlayerCreationCommentId = player_creation_comment_rating.player_creation_comment_id,
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

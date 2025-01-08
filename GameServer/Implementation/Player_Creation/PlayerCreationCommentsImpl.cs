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
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Implementation.Player_Creation
{
    public class PlayerCreationCommentsImpl
    {
        public static string ListComments(Database database, Guid SessionID, int page, int per_page, SortColumn sort_column, SortOrder sort_order, int limit, Platform platform, string playerCreationIDFilter, string authorIDFilter)
        {
            var Comments = new List<PlayerCreationCommentData> { };
            var session = SessionImpl.GetSession(SessionID);
            var requestedBy = database.Users.FirstOrDefault(match => match.Username == session.Username);

            var playerCreationIds = playerCreationIDFilter
                .Split(',')
                .Select(x => int.Parse(x))
                .ToArray();

            var commentsQuery = database.PlayerCreationComments
                .Include(x => x.Creation)
                .Where(match => playerCreationIds.Contains(match.Creation.Id));

            //sorting
            if (sort_column == SortColumn.created_at)
                commentsQuery =
                        sort_order == SortOrder.asc ?
                            commentsQuery.OrderBy(match => match.CreatedAt) :
                            commentsQuery.OrderByDescending(match => match.CreatedAt);

            //calculating pages
            var pageStart = PageCalculator.GetPageStart(page, per_page);
            var pageEnd = PageCalculator.GetPageStart(page, per_page);
            var total = commentsQuery.Count();
            var totalPages = PageCalculator.GetTotalPages(total, per_page);

            var comments = commentsQuery
                .Skip(pageStart)
                .Take(per_page)
                .ProjectTo<Models.Response.PlayerCreationComment>(database.MapperConfig, new { requestedBy })
                .ToList();

            var resp = new Response<List<PlayerCreationComments>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new PlayerCreationComments {
                    Page = page,
                    RowStart = pageStart,
                    RowEnd = pageEnd,
                    Total = total,
                    TotalPages = totalPages,
                    PlayerCreationCommentList = comments
                } ]
            };

            return resp.Serialize();
        }

        public static string CreateComment(Database database, Guid SessionID, Models.Request.PlayerCreationComment player_creation_comment)
        {
            var session = SessionImpl.GetSession(SessionID);
            var author = database.Users.FirstOrDefault(match => match.Username == session.Username);

            var creation = database.PlayerCreations
                .FirstOrDefault(match => match.Id == player_creation_comment.player_creation_id);

            if (author == null || creation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var dbComment = new PlayerCreationCommentData
            {
                Player = author,
                Body = player_creation_comment.body,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Platform = Platform.PS3,
                Creation = creation
            };
            database.PlayerCreationComments.Add(dbComment);
            // TODO: Is database.SaveChanges needed?
            database.ActivityLog.Add(new ActivityEvent
            {
                Author = author,
                Type = ActivityType.player_creation_event,
                List = ActivityList.activity_log,
                Topic = "player_creation_commented_on",
                Description = player_creation_comment.body,
                Creation = creation,
                CreatedAt = DateTime.UtcNow,
                AllusionId = dbComment.Id,
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

            var comment = database.PlayerCreationComments
                .Include(x => x.Player)
                .Include(x => x.Creation)
                .Include(x => x.Creation.Author)
                .FirstOrDefault(match => match.Id == id);

            if (user == null || comment == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (comment.Creation != null)
            {
                if (comment.Creation.Author.UserId != user.UserId && comment.Player.UserId != user.UserId)
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

            var comment = database.PlayerCreationComments
                .FirstOrDefault(match => match.Id == player_creation_comment_rating.player_creation_comment_id);

            if (user == null || comment == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var rating = database.PlayerCreationCommentRatings
                .Include(x => x.Player)
                .Include(x => x.Comment)
                .FirstOrDefault(match => match.Comment.Id == player_creation_comment_rating.player_creation_comment_id && match.Player.UserId == user.UserId);

            if (rating == null)
            {
                database.PlayerCreationCommentRatings.Add(new PlayerCreationCommentRatingData
                {
                    Comment = comment,
                    Player = user,
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

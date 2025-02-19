using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Models;
using GameServer.Utils;
using System.Collections.Generic;
using System.Linq;
using System;
using GameServer.Implementation.Common;
using GameServer.Models.Common;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.FileProviders.Embedded;

namespace GameServer.Implementation.Player
{
    public class PlayerCommentsImpl
    {
        public static string ListComments(Database database, Guid SessionID, int page, int per_page, int limit, SortColumn sort_column,
            Platform platform, string playerIDFilter, string authorIDFilter)
        {
            var Comments = new List<PlayerCommentData> { };
            var session = SessionImpl.GetSession(SessionID);
            var requestedBy = database.Users.FirstOrDefault(match => match.Username == session.Username);

            var playerIds = playerIDFilter.Split(',').Select(x => int.Parse(x));
            var commentsQuery = database.PlayerComments
                .Include(x => x.Player)
                .Where(match => playerIds.Contains(match.Player.UserId));

            //sorting
            if (sort_column == SortColumn.created_at)
                commentsQuery = commentsQuery.OrderByDescending(match => match.CreatedAt);

            //calculating pages
            var pageStart = PageCalculator.GetPageStart(page, per_page);
            var pageEnd = PageCalculator.GetPageStart(page, per_page);
            var total = commentsQuery.Count();
            var totalPages = PageCalculator.GetTotalPages(total, per_page);

            var comments = commentsQuery
                .Skip(pageStart)
                .Take(per_page)
                .ProjectTo<Models.Response.PlayerComment>(database.MapperConfig, new { requestedBy })
                .ToList();

            var resp = new Response<List<PlayerComments>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new PlayerComments {
                    Page = page,
                    RowStart = pageStart,
                    RowEnd = pageEnd,
                    Total = Comments.Count,
                    TotalPages = totalPages,
                    PlayerCommentList = comments
                } ]
            };

            return resp.Serialize();
        }

        public static string CreateComment(Database database, Guid SessionID, Models.Request.PlayerComment player_comment)
        {
            var session = SessionImpl.GetSession(SessionID);
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

            var comment = new PlayerCommentData
            {
                Author = author,
                Body = player_comment.body,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Platform = Platform.PS3,
                Player = user
            };
            database.PlayerComments.Add(comment);
            //database.SaveChanges();   // TODO: Is this needed?
            database.ActivityLog.Add(new ActivityEvent
            {
                Author = author,
                Type = ActivityType.player_event,
                List = ActivityList.activity_log,
                Topic = "player_authored_comment",
                Description = player_comment.body,
                Player = user,
                CreatedAt = DateTime.UtcNow,
                AllusionId = comment.Id,
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
            var session = SessionImpl.GetSession(SessionID);
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

            if (comment.Author.UserId != user.UserId && comment.Player.UserId != user.UserId)
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
            var session = SessionImpl.GetSession(SessionID);
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

            var rating = database.PlayerCommentRatings
                .Include(x => x.Comment)
                .FirstOrDefault(match => match.Comment.Id == player_comment_rating.player_comment_id
                    && match.Player.UserId == user.UserId);

            if (rating == null)
            {
                database.PlayerCommentRatings.Add(new PlayerCommentRatingData
                {
                    Comment = rating.Comment,
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

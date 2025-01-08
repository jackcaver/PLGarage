using GameServer.Models.Response;
using GameServer.Models;
using System.Collections.Generic;
using GameServer.Utils;
using System.Linq;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.PlayerData;
using System;
using GameServer.Models.Request;
using GameServer.Implementation.Common;
using Microsoft.EntityFrameworkCore;
using GameServer.Models.Common;
using AutoMapper.QueryableExtensions;

namespace GameServer.Implementation.Player_Creation
{
    public class PlayerCreationRatingsImpl  // TODO: !!! IMPORTANT !!!: Can we modify these impls to inherit a base class with the session object?
    {
        public static string View(Database database, Guid SessionID, int player_creation_id, int player_id)
        {
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);  // TODO: Store in session!

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var rating = database.PlayerCreationRatings
                .Include(x => x.Player)
                .Include(x => x.Creation)
                .Where(match => match.Creation.Id == player_creation_id && match.Player.UserId == user.UserId)
                .ProjectTo<Models.Response.PlayerCreationRating>(database.MapperConfig)
                .FirstOrDefault();
            
            var resp = new Response<List<Models.Response.PlayerCreationRating>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ rating != null ? rating : new Models.Response.PlayerCreationRating {
                    Comments = "",
                    Rating = session.IsMNR ? (rating != null ? rating.Rating.ToString() : "0") : (rating != null ? "true" : "false"),
                } ]
            };
            return resp.Serialize();
        }

        public static string List(Database database, int player_creation_id, int page, int per_page)
        {
            var ratingsQuery = database.PlayerCreationRatings
                .Include(x => x.Player)
                .Include(x => x.Creation)
                .Where(match => match.Creation.Id == player_creation_id);

            //calculating pages
            var pageStart = PageCalculator.GetPageStart(page, per_page);
            var pageEnd = PageCalculator.GetPageStart(page, per_page);
            var total = ratingsQuery.Count();
            var totalPages = PageCalculator.GetTotalPages(total, per_page);

            var ratings = ratingsQuery
                .Skip(pageStart)
                .Take(per_page)
                .ProjectTo<Models.Response.PlayerCreationRating>(database.MapperConfig)
                .ToList();

            var resp = new Response<List<PlayerCreationRatings>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [
                    new PlayerCreationRatings
                    {
                        Page = page,
                        RowEnd = pageEnd,
                        RowStart = pageStart,
                        Total = total,
                        TotalPages = totalPages,
                        PlayerCreationRatingList = ratings,
                    }
                ]
            };
            return resp.Serialize();
        }

        public static string Create(Database database, Guid SessionID, Models.Request.PlayerCreationRating player_creation_rating)
        {
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            var creation = database.PlayerCreations
                .Include(x => x.Author)
                .FirstOrDefault(match => match.Id == player_creation_rating.player_creation_id);

            if (user == null || creation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var rating = database.PlayerCreationRatings
                .Include(x => x.Player)
                .Include(x => x.Creation)
                .FirstOrDefault(match => match.Creation.Id == player_creation_rating.player_creation_id && match.Player.UserId == user.UserId);

            if (rating == null)
            {
                database.PlayerCreationRatings.Add(new PlayerCreationRatingData
                {
                    Creation = creation,
                    Player = user,
                    Type = RatingType.YAY,
                    RatedAt = DateTime.UtcNow,
                    Rating = player_creation_rating.rating,
                    Comment = player_creation_rating.comments
                });
                database.ActivityLog.Add(new ActivityEvent
                {
                    Author = user,
                    Type = ActivityType.player_creation_event,
                    List = ActivityList.activity_log,
                    Topic = "player_creation_rated_up",
                    Description = "",
                    Creation = creation,
                    CreatedAt = DateTime.UtcNow,
                    AllusionId = creation.Id,
                    AllusionType = "PlayerCreation::Track"
                });
                database.SaveChanges();
            }

            if (player_creation_rating.comments != null && (rating == null || rating.Comment == null))
            {
                database.PlayerCreationPoints.Add(new PlayerCreationPoint
                {
                    Creation = creation,
                    Player = creation.Author,
                    Platform = creation.Platform,
                    Type = creation.Type,
                    CreatedAt = DateTime.UtcNow,
                    Amount = 20
                });
                database.MailMessages.Add(new MailMessageData
                {
                    Body = player_creation_rating.comments,
                    Subject = creation.Name,
                    RecipientList = creation.Author.Username,
                    Type = MailMessageType.ALERT,
                    Recipient = creation.Author,
                    Sender = user,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                database.SaveChanges();
            }

            if (player_creation_rating.rating != 0 && (rating == null || rating.Rating == 0))
            {
                database.PlayerCreationPoints.Add(new PlayerCreationPoint
                {
                    Creation = creation,
                    Player = creation.Author,
                    Platform = creation.Platform,
                    Type = creation.Type,
                    CreatedAt = DateTime.UtcNow,
                    Amount = 20
                });
                database.SaveChanges();
            }

            if (rating != null)
            {
                rating.Rating = player_creation_rating.rating;
                rating.Comment = player_creation_rating.comments;
                database.SaveChanges();
            }

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static string Clear(Database database, Guid SessionID, int player_creation_id)
        {
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            
            var rating = database.PlayerCreationRatings
                .Include(x => x.Creation)
                .FirstOrDefault(match => match.Player.UserId == user.UserId && match.Creation.Id == player_creation_id);

            if (user == null || rating == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.PlayerCreationRatings.Remove(rating);
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

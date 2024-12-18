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

namespace GameServer.Implementation.Player_Creation
{
    public class PlayerCreationRatings
    {
        public static string View(Database database, Guid SessionID, int player_creation_id, int player_id)
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

            var rating = database.PlayerCreationRatings.FirstOrDefault(match => match.PlayerCreationId == player_creation_id && match.PlayerId == user.UserId);
            
            var resp = new Response<List<player_creation_rating>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new player_creation_rating {
                    comments = rating != null ? rating.Comment : "",
                    rating = session.IsMNR ? (rating != null ? rating.Rating.ToString() : "0") : (rating != null ? "true" : "false"),
                } ]
            };
            return resp.Serialize();
        }

        public static string List(Database database, int player_creation_id, int page, int per_page)
        {
            var ratings = database.PlayerCreationRatings.Where(match => match.PlayerCreationId == player_creation_id).ToList();

            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, ratings.Count);

            if (pageEnd > ratings.Count)
                pageEnd = ratings.Count;

            var ratingsList = new List<player_creation_rating>();

            for (int i = pageStart; i < pageEnd; i++)
            {
                var rating = ratings[i];
                ratingsList.Add(new player_creation_rating
                {
                    comments = rating.Comment,
                    rating = rating.Rating.ToString(),
                    player_id = rating.PlayerId.ToString(),
                    username = database.Users.FirstOrDefault(match => match.UserId == rating.PlayerId).Username
                });
            }

            var resp = new Response<List<player_creation_ratings>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [
                    new player_creation_ratings
                    {
                        page = page,
                        row_end = pageEnd,
                        row_start = pageStart,
                        total = ratings.Count,
                        total_pages = totalPages,
                        PlayerCreationRatingList = ratingsList,
                    }
                ]
            };
            return resp.Serialize();
        }

        public static string Create(Database database, Guid SessionID, PlayerCreationRating player_creation_rating)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var Creation = database.PlayerCreations
                .Include(x => x.Author)
                .FirstOrDefault(match => match.PlayerCreationId == player_creation_rating.player_creation_id);

            if (user == null || Creation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var rating = database.PlayerCreationRatings.FirstOrDefault(match => match.PlayerCreationId == player_creation_rating.player_creation_id && match.PlayerId == user.UserId);

            if (rating == null)
            {
                database.PlayerCreationRatings.Add(new PlayerCreationRatingData
                {
                    PlayerCreationId = player_creation_rating.player_creation_id,
                    PlayerId = user.UserId,
                    Type = RatingType.YAY,
                    RatedAt = DateTime.UtcNow,
                    Rating = player_creation_rating.rating,
                    Comment = player_creation_rating.comments
                });
                database.ActivityLog.Add(new ActivityEvent
                {
                    AuthorId = user.UserId,
                    Type = ActivityType.player_creation_event,
                    List = ActivityList.activity_log,
                    Topic = "player_creation_rated_up",
                    Description = "",
                    PlayerId = 0,
                    PlayerCreationId = Creation.PlayerCreationId,
                    CreatedAt = DateTime.UtcNow,
                    AllusionId = Creation.PlayerCreationId,
                    AllusionType = "PlayerCreation::Track"
                });
                database.SaveChanges();
            }

            if (player_creation_rating.comments != null && (rating == null || rating.Comment == null))
            {
                database.PlayerCreationPoints.Add(new PlayerCreationPoint { PlayerCreationId = Creation.PlayerCreationId, PlayerId = Creation.PlayerId, Platform = Creation.Platform, Type = Creation.Type, CreatedAt = DateTime.UtcNow, Amount = 20 });
                database.MailMessages.Add(new MailMessageData
                {
                    Body = player_creation_rating.comments,
                    Subject = Creation.Name,
                    RecipientList = Creation.Author.Username,
                    Type = MailMessageType.ALERT,
                    RecipientId = Creation.PlayerId,
                    SenderId = user.UserId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                database.SaveChanges();
            }

            if (player_creation_rating.rating != 0 && (rating == null || rating.Rating == 0))
            {
                database.PlayerCreationPoints.Add(new PlayerCreationPoint { PlayerCreationId = Creation.PlayerCreationId, PlayerId = Creation.PlayerId, Platform = Creation.Platform, Type = Creation.Type, CreatedAt = DateTime.UtcNow, Amount = 20 });
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
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var rating = database.PlayerCreationRatings.FirstOrDefault(match => match.PlayerId == user.UserId && match.PlayerCreationId == player_creation_id);

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

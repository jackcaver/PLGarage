using GameServer.Implementation.Common;
using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
                    rating = session.IsMNR ? (rating != null ? rating.Rating.ToString("0.0", CultureInfo.InvariantCulture) : "0") : (rating != null ? "true" : "false"),
                } ]
            };
            return resp.Serialize();
        }

        public static string List(Database database, int player_creation_id, int page, int per_page)
        {
            var ratingsQuery = database.PlayerCreationRatings
                .Include(r => r.Player)
                .Where(match => match.PlayerCreationId == player_creation_id);

            var total = ratingsQuery.Count();

            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, total);

            if (pageEnd > total)
                pageEnd = total;

            var ratings = ratingsQuery.Skip(pageStart).Take(per_page).ToList();

            var ratingsList = new List<player_creation_rating>();

            foreach (var rating in ratings)
            {
                ratingsList.Add(new player_creation_rating
                {
                    comments = rating.Comment,
                    rating = rating.Rating.ToString("0.0", CultureInfo.InvariantCulture),
                    player_id = rating.PlayerId.ToString(),
                    username = rating.Username
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
                        total = total,
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
                    RatedAt = TimeUtils.Now,
                    Rating = player_creation_rating.rating,
                    Comment = player_creation_rating.comments
                });
                if (!session.IsMNR)
                {
                    database.ActivityLog.Add(new ActivityEvent
                    {
                        AuthorId = user.UserId,
                        Type = ActivityType.player_creation_event,
                        List = ActivityList.activity_log,
                        Topic = "player_creation_rated_up",
                        Description = "",
                        PlayerId = 0,
                        PlayerCreationId = Creation.PlayerCreationId,
                        CreatedAt = TimeUtils.Now,
                        AllusionId = Creation.PlayerCreationId,
                        AllusionType = "PlayerCreation::Track"
                    });
                }
                database.SaveChanges();
            }

            if (player_creation_rating.comments != null && (rating == null || rating.Comment == null))
            {
                database.PlayerCreationPoints.Add(new PlayerCreationPoint { PlayerCreationId = Creation.PlayerCreationId, PlayerId = Creation.PlayerId, Platform = Creation.Platform, Type = Creation.Type, CreatedAt = TimeUtils.Now, Amount = 20 });
                if (session.IsMNR && session.Platform == Platform.PS3)
                {
                    database.MailMessages.Add(new MailMessageData
                    {
                        Body = player_creation_rating.comments,
                        Subject = Creation.Name,
                        RecipientList = Creation.Username,
                        Type = MailMessageType.ALERT,
                        RecipientId = Creation.PlayerId,
                        SenderId = user.UserId,
                        CreatedAt = TimeUtils.Now,
                        UpdatedAt = TimeUtils.Now
                    });
                }
                database.SaveChanges();
            }

            if (player_creation_rating.rating != 0 && (rating == null || rating.Rating == 0))
            {
                database.PlayerCreationPoints.Add(new PlayerCreationPoint { PlayerCreationId = Creation.PlayerCreationId, PlayerId = Creation.PlayerId, Platform = Creation.Platform, Type = Creation.Type, CreatedAt = TimeUtils.Now, Amount = 20 });
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

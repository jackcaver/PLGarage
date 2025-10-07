using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Models;
using System;
using GameServer.Utils;
using System.Linq;
using System.Collections.Generic;
using GameServer.Implementation.Common;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Implementation.Player_Creation
{
    public class PlayerCreationBookmarks
    {
        public static string ListBookmarks(Database database, Guid SessionID, int page, int per_page, SortColumn sort_column, SortOrder sort_order, string keyword, int limit, Platform platform, Filters filters)
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

            string idFilter = "";
            var BookmarkedCreations = database.PlayerCreationBookmarks
                .OrderBy(b => b.BookmarkedAt)
                .Where(match => match.UserId == user.UserId).ToList();

            foreach (var bookmark in BookmarkedCreations)
            {
                idFilter += $"{bookmark.BookmarkedPlayerCreationId},";
            }

            filters.id = idFilter.Split(',');

            return PlayerCreations.SearchPlayerCreations(database, SessionID, page, per_page, sort_column, sort_order, limit, platform, filters, keyword);
        }

        public static string CreateBookmark(Database database, Guid SessionID, int id)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var Creation = database.PlayerCreations
                .Include(x => x.Bookmarks)
                .FirstOrDefault(match => match.PlayerCreationId == id);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (Creation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (!Creation.IsBookmarkedByMe(user.UserId))
            {
                database.PlayerCreationBookmarks.Add(new PlayerCreationBookmark
                {
                    BookmarkedPlayerCreationId = Creation.PlayerCreationId,
                    UserId = user.UserId,
                    BookmarkedAt = TimeUtils.Now,
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

        public static string RemoveBookmark(Database database, Guid SessionID, int id)
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

            var BookmarkedCreation = database.PlayerCreationBookmarks.FirstOrDefault(match => match.BookmarkedPlayerCreationId == id && match.UserId == user.UserId);

            if (BookmarkedCreation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.PlayerCreationBookmarks.Remove(BookmarkedCreation);
            database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static string Tally(Database database, Guid SessionID)
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

            var resp = new Response<List<PlayerCreationBookmarksCount>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [new PlayerCreationBookmarksCount { 
                    total = database.PlayerCreationBookmarks.Count(match => match.UserId == user.UserId)
                }]
            };
            return resp.Serialize();
        }
    }
}

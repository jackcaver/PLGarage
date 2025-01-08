using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Models;
using System;
using GameServer.Utils;
using System.Linq;
using System.Collections.Generic;
using GameServer.Implementation.Common;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using GameServer.Models.Common;

namespace GameServer.Implementation.Player_Creation
{
    public class PlayerCreationBookmarksImpl
    {
        public static string ListBookmarks(Database database, Guid SessionID, int page, int per_page, SortColumn sort_column, SortOrder sort_order, string keyword, int limit, Platform platform, Filters filters)
        {
            var session = SessionImpl.GetSession(SessionID);
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

            // TODO: Is sort_column/sort_order even used here?
            var bookmarkedCreations = database.PlayerCreationBookmarks
                .Include(x => x.User)
                .Include(x => x.BookmarkedCreation)
                .Where(match => match.User.UserId == user.UserId)
                .OrderByDescending(match => match.BookmarkedAt)
                .Select(match => match.BookmarkedCreation.Id)
                .ToList();

            filters.id = bookmarkedCreations
                .Select(x => x.ToString())
                .ToArray();

            return PlayerCreationsImpl.SearchPlayerCreations(database, SessionID, page, per_page, sort_column, sort_order, limit, platform, filters, keyword);
        }

        public static string CreateBookmark(Database database, Guid SessionID, int id)
        {
            var session = SessionImpl.GetSession(SessionID);
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

            var creation = database.PlayerCreations
                .Include(x => x.Bookmarks)
                .FirstOrDefault(match => match.Id == id);

            if (creation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (!creation.Bookmarks.Any(match => match.User.UserId == user.UserId))
            {
                database.PlayerCreationBookmarks.Add(new PlayerCreationBookmark
                {
                    BookmarkedCreation = creation,
                    User = user,
                    BookmarkedAt = DateTime.UtcNow,
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
            var session = SessionImpl.GetSession(SessionID);
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

            var bookmarkedCreation = database.PlayerCreationBookmarks
                .Include(x => x.User)
                .Include(x => x.BookmarkedCreation)
                .FirstOrDefault(match => match.BookmarkedCreation.Id == id && match.User.UserId == user.UserId);

            if (bookmarkedCreation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.PlayerCreationBookmarks.Remove(bookmarkedCreation);
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
            var session = SessionImpl.GetSession(SessionID);
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

            var bookmarkedCreationsCount = database.PlayerCreationBookmarks
                .Include(x => x.User)
                .Where(match => match.User.UserId == user.UserId)
                .Count();

            var resp = new Response<List<PlayerCreationBookmarksCount>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [new PlayerCreationBookmarksCount { Total = bookmarkedCreationsCount }]
            };
            return resp.Serialize();
        }
    }
}

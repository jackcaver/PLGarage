using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Models;
using System;
using GameServer.Utils;
using System.Linq;
using System.Collections.Generic;

namespace GameServer.Implementation.Player_Creation
{
    public class PlayerCreationBookmarks
    {
        public static string ListBookmarks(Database database, int page, int per_page, SortColumn sort_column, SortOrder sort_order, string keyword, int limit, Platform platform, Filters filters, string username)
        {
            var user = database.Users.FirstOrDefault(match => match.Username == username);
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
            var BookmarkedCreations = database.PlayerCreationBookmarks.Where(match => match.UserId == user.UserId).ToList();
            BookmarkedCreations.Sort((curr, prev) => prev.BookmarkedAt.CompareTo(curr.BookmarkedAt));

            foreach (var bookmark in BookmarkedCreations)
            {
                idFilter += $"{bookmark.BookmarkedPlayerCreationId},";
            }

            filters.id = idFilter.Split(',');

            return PlayerCreations.SearchPlayerCreations(database, page, per_page, sort_column, sort_order, limit, platform, filters, keyword);
        }

        public static string CreateBookmark(Database database, string username, int id)
        {
            var user = database.Users.FirstOrDefault(match => match.Username == username);
            var Creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == id);

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

        public static string RemoveBookmark(Database database, string username, int id)
        {
            var user = database.Users.FirstOrDefault(match => match.Username == username);

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

        public static string Tally(Database database, string username)
        {
            var user = database.Users.FirstOrDefault(match => match.Username == username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var BookmarkedCreations = database.PlayerCreationBookmarks.Where(match => match.UserId == user.UserId).ToList();

            var resp = new Response<List<PlayerCreationBookmarksCount>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<PlayerCreationBookmarksCount> { new PlayerCreationBookmarksCount { total = BookmarkedCreations.Count } }
            };
            return resp.Serialize();
        }
    }
}

using GameServer.Implementation.Common;
using GameServer.Implementation.Player_Creation;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GameServer.Controllers.Player_Creation
{
    public class PlayerCreationBookmarksController(Database database) : Controller
    {
        [HttpGet]
        [Authorize]
        [Route("player_creation_bookmarks.xml")]
        public IActionResult Get(int page, int per_page, SortColumn sort_column, SortOrder sort_order, string keyword, int limit, Platform platform, [FromQuery]Filters filters)
        {
            var user = Session.GetUser(database, User);
            string playerCreationTypeString = Request.Query["filters[player_creation_type]"];
            if (Enum.TryParse(playerCreationTypeString, out PlayerCreationType playerCreationType))
                filters.player_creation_type = playerCreationType;
            filters.race_type = Request.Query["filters[race_type]"];
            filters.tags = Request.Query["filters[tags]"];
            return Content(PlayerCreationBookmarks.ListBookmarks(database, user, page, per_page, sort_column, sort_order, keyword, limit, platform, 
                filters), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("player_creation_bookmarks.xml")]
        public IActionResult Create(int player_creation_id)
        {
            var user = Session.GetUser(database, User);
            return Content(PlayerCreationBookmarks.CreateBookmark(database, user, player_creation_id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("player_creation_bookmarks/remove.xml")]
        public IActionResult Remove(int player_creation_id)
        {
            var user = Session.GetUser(database, User);
            return Content(PlayerCreationBookmarks.RemoveBookmark(database, user, player_creation_id), "application/xml;charset=utf-8");
        }

        [Authorize]
        [Route("player_creation_bookmarks/tally.xml")]
        public IActionResult Tally()
        {
            var user = Session.GetUser(database, User);
            return Content(PlayerCreationBookmarks.Tally(database, user), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
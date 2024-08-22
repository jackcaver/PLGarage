using GameServer.Implementation.Player_Creation;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GameServer.Controllers.Player_Creation
{
    public class PlayerCreationBookmarksController : Controller
    {
        private readonly Database database;

        public PlayerCreationBookmarksController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("player_creation_bookmarks.xml")]
        public IActionResult Get(int page, int per_page, SortColumn sort_column, SortOrder sort_order, string keyword, int limit, Platform platform, Filters filters)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            string playerCreationTypeString = Request.Query["filters[player_creation_type]"];
            PlayerCreationType playerCreationType = PlayerCreationType.PHOTO;
            Enum.TryParse(playerCreationTypeString, out playerCreationType);
            filters.player_creation_type = playerCreationType;
            filters.race_type = Request.Query["filters[race_type]"];
            filters.tags = Request.Query["filters[tags]"];
            return Content(PlayerCreationBookmarks.ListBookmarks(database, SessionID, page, per_page, sort_column, sort_order, keyword, limit, platform, 
                filters), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_bookmarks.xml")]
        public IActionResult Create(int player_creation_id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreationBookmarks.CreateBookmark(database, SessionID, player_creation_id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creation_bookmarks/remove.xml")]
        public IActionResult Remove(int player_creation_id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreationBookmarks.RemoveBookmark(database, SessionID, player_creation_id), "application/xml;charset=utf-8");
        }

        [Route("player_creation_bookmarks/tally.xml")]
        public IActionResult Tally()
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreationBookmarks.Tally(database, SessionID), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
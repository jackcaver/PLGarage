using GameServer.Implementation.Player_Creation;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GameServer.Controllers.Player_Creation
{
    public class TracksController : Controller
    {
        private readonly Database database;

        public TracksController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("tracks/{id}/profile.xml")]
        public IActionResult GetProfile(int id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreations.GetTrackProfile(database, SessionID, id), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("tracks/{id}.xml")]
        public IActionResult Get(int id, bool is_counted)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreations.GetPlayerCreation(database, SessionID, id, is_counted), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("tracks/{id}/download.xml")]
        public IActionResult Download(int id, bool is_counted)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreations.GetPlayerCreation(database, SessionID, id, is_counted, true), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("tracks/friends_published.xml")]
        public IActionResult FriendsPublished(Platform platform)
        {
            return Content(PlayerCreations.PlayerCreationsFriendsPublished(database, Request.Query["filters[username]"], PlayerCreationType.TRACK),
                "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("tracks/lucky_dip.xml")]
        public IActionResult LuckyDip(int page, int per_page, string keyword, int limit, Platform platform, Filters filters)
        {
            filters.race_type = Request.Query["filters[race_type]"];
            filters.username = Request.Query.Keys.Contains("filters[username]") ? Request.Query["filters[username]"].ToString().Split(',') : null;
            filters.tags = Request.Query.Keys.Contains("filters[tags]") ? Request.Query["filters[tags]"].ToString().Split(',') : null;
            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            filters.player_creation_type = PlayerCreationType.TRACK;
            return Content(PlayerCreations.SearchPlayerCreations(database, page, per_page, SortColumn.created_at, SortOrder.desc, limit, platform, filters, keyword, false, true), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("tracks/ufg_picks.xml")]
        public IActionResult GetTeamPicks(int page, int per_page, SortColumn sort_column, SortOrder sort_order, string keyword, int limit, Platform platform, Filters filters)
        {
            filters.race_type = Request.Query["filters[race_type]"];
            filters.username = Request.Query.Keys.Contains("filters[username]") ? Request.Query["filters[username]"].ToString().Split(',') : null;
            filters.tags = Request.Query.Keys.Contains("filters[tags]") ? Request.Query["filters[tags]"].ToString().Split(',') : null;
            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            filters.player_creation_type = PlayerCreationType.TRACK;
            return Content(PlayerCreations.SearchPlayerCreations(database, page, per_page, sort_column, sort_order, limit, platform, filters, keyword, true), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("tracks.xml")]
        public IActionResult Search(int page, int per_page, SortColumn sort_column, SortOrder sort_order, string keyword, int limit, Platform platform, Filters filters)
        {
            filters.race_type = Request.Query["filters[race_type]"];
            filters.username = Request.Query.Keys.Contains("filters[username]") ? Request.Query["filters[username]"].ToString().Split(',') : null;
            filters.tags = Request.Query.Keys.Contains("filters[tags]") ? Request.Query["filters[tags]"].ToString().Split(',') : null;
            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            filters.player_creation_type = PlayerCreationType.TRACK;
            return Content(PlayerCreations.SearchPlayerCreations(database, page, per_page, sort_column, sort_order, limit, platform, filters, keyword),
                "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("tracks.xml")]
        public IActionResult Create(PlayerCreation player_creation)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            player_creation.data = Request.Form.Files.GetFile("player_creation[data]");
            player_creation.preview = Request.Form.Files.GetFile("player_creation[preview]");
            return Content(PlayerCreations.CreatePlayerCreation(database, SessionID, player_creation));
        }

        [HttpPost]
        [Route("tracks/{id}/update.xml")]
        public IActionResult Update(int id, PlayerCreation player_creation)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            player_creation.data = Request.Form.Files.GetFile("player_creation[data]");
            player_creation.preview = Request.Form.Files.GetFile("player_creation[preview]");
            return Content(PlayerCreations.UpdatePlayerCreation(database, SessionID, player_creation, id),
                "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("tracks/{id}.xml")]
        public IActionResult Delete(int id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreations.RemovePlayerCreation(database, SessionID, id), "application/xml;charset=utf-8");
        }
    }
}
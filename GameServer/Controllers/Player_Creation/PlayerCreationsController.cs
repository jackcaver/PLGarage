using System.Collections.Generic;
using GameServer.Implementation.Player_Creation;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using GameServer.Implementation.Common;

namespace GameServer.Controllers.Player_Creation
{
    public class PlayerCreationsController : Controller
    {
        private readonly Database database;

        public PlayerCreationsController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("player_creations/{id}.xml")]
        public IActionResult Get(int id, bool is_counted)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            var resp = PlayerCreations.GetPlayerCreation(database, SessionID, id, is_counted);
            //Response.Headers.Add("ETag", $"\"{UserGeneratedContentUtils.CalculateMD5(resp)}\"");
            //Response.Headers.Add("Cache-Control", "private, max-age=0, must-revalidate");
            return Content(resp, "application/xml;charset=utf-8");
        }

        [HttpDelete]
        [Route("player_creations/{id}.xml")]
        public IActionResult Delete(int id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreations.RemovePlayerCreation(database, SessionID, id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creations/{id}/download.xml")]
        public IActionResult Download(int id, bool is_counted)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreations.GetPlayerCreation(database, SessionID, id, is_counted, true), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("player_creations/{platform}/surprise_me.xml")]
        public IActionResult SurpriseMeOnPlatform(int page, int per_page, SortColumn sort_column, SortOrder sort_order, int limit, Platform platform, Filters filters)
        {
            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            filters.player_id = Request.Query.Keys.Contains("filters[player_id]") ? Request.Query["filters[player_id]"].ToString().Split(',') : null;
            PlayerCreationType TypeFilter = PlayerCreationType.TRACK;
            Enum.TryParse(Request.Query["filters[player_creation_type]"], out TypeFilter);
            filters.player_creation_type = TypeFilter;

            return Content(PlayerCreations.SearchPlayerCreations(database, page, per_page, sort_column, sort_order, limit, platform, filters, null, false, true, true),
                "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("player_creations/surprise_me.xml")]
        public IActionResult SurpriseMe(int page, int per_page, SortColumn sort_column, SortOrder sort_order, int limit, Platform platform, Filters filters)
        {
            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            filters.player_id = Request.Query.Keys.Contains("filters[player_id]") ? Request.Query["filters[player_id]"].ToString().Split(',') : null;
            PlayerCreationType TypeFilter = PlayerCreationType.TRACK;
            Enum.TryParse(Request.Query["filters[player_creation_type]"], out TypeFilter);
            filters.player_creation_type = TypeFilter;
            if (Request.Cookies.ContainsKey("session_id") && platform == Platform.PS2)
                platform = Session.GetSession(Guid.Parse(Request.Cookies["session_id"])).Platform;

            return Content(PlayerCreations.SearchPlayerCreations(database, page, per_page, sort_column, sort_order, limit, platform, filters, null, false, true, true),
                "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("player_creations/team_picks.xml")]
        public IActionResult TeamPicks(int page, int per_page, SortColumn sort_column, SortOrder sort_order, int limit, Platform platform, Filters filters)
        {
            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            filters.player_id = Request.Query.Keys.Contains("filters[player_id]") ? Request.Query["filters[player_id]"].ToString().Split(',') : null;
            PlayerCreationType TypeFilter = PlayerCreationType.TRACK;
            Enum.TryParse(Request.Query["filters[player_creation_type]"], out TypeFilter);
            filters.player_creation_type = TypeFilter;

            return Content(PlayerCreations.SearchPlayerCreations(database, page, per_page, sort_column, sort_order, limit, platform, filters, null, true, false, true),
                "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("player_creations/friends_view.xml")]
        [Route("player_creations/friends_and_favorites_view.xml")]
        public IActionResult FriendsView(int page, int per_page, SortColumn sort_column, SortOrder sort_order, int limit, Platform platform, Filters filters)
        {
            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            filters.player_id = Request.Query.Keys.Contains("filters[player_id]") ? Request.Query["filters[player_id]"].ToString().Split(',') : null;
            PlayerCreationType TypeFilter = PlayerCreationType.TRACK;
            Enum.TryParse(Request.Query["filters[player_creation_type]"], out TypeFilter);
            filters.player_creation_type = TypeFilter;
            filters.username = Request.Query.Keys.Contains("filters[username]") ? Request.Query["filters[username]"].ToString().Split(',') : null;

            return Content(PlayerCreations.SearchPlayerCreations(database, page, per_page, sort_column, sort_order, limit, platform, filters, null, false, false, true),
                "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("player_creations/search.xml")]
        [Route("player_creations.xml")]
        public IActionResult Search(int page, int per_page, SortColumn sort_column, SortOrder sort_order, string search, string search_tags, 
            string username, PlayerCreationType? player_creation_type, bool? is_remixable, bool? ai, bool? auto_reset, int limit, 
            Platform platform, Filters filters)
        {
            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            filters.player_id = Request.Query.Keys.Contains("filters[player_id]") ? Request.Query["filters[player_id]"].ToString().Split(',') : null;
            PlayerCreationType TypeFilter = PlayerCreationType.TRACK;
            Enum.TryParse(Request.Query["filters[player_creation_type]"], out TypeFilter);
            if (player_creation_type != null)
                Enum.TryParse(player_creation_type.ToString(), out TypeFilter);
            filters.player_creation_type = TypeFilter;
            filters.tags = search_tags?.Split(',');
            filters.username = username?.Split(',');
            filters.ai = ai;
            filters.is_remixable = is_remixable;
            filters.auto_reset = auto_reset;

            return Content(PlayerCreations.SearchPlayerCreations(database, page, per_page, sort_column, sort_order, limit, platform, filters, search, false, false, true),
                "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("player_creations/mine.xml")]
        public IActionResult Mine(int page, int per_page, SortColumn sort_column, SortOrder sort_order, string keyword, int limit, Filters filters)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            PlayerCreationType TypeFilter = PlayerCreationType.TRACK;
            Enum.TryParse(Request.Query["filters[player_creation_type]"], out TypeFilter);
            filters.player_creation_type = TypeFilter;

            return Content(PlayerCreations.Mine(database, SessionID, page, per_page, sort_column, sort_order, limit, filters, keyword),
                "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("player_creations/{platform}/mine.xml")]
        public IActionResult MineOnPlatform(int page, int per_page, SortColumn sort_column, SortOrder sort_order, string keyword, int limit, Filters filters, Platform platform)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            PlayerCreationType TypeFilter = PlayerCreationType.TRACK;
            Enum.TryParse(Request.Query["filters[player_creation_type]"], out TypeFilter);
            filters.player_creation_type = TypeFilter;

            return Content(PlayerCreations.Mine(database, SessionID, page, per_page, sort_column, sort_order, limit, filters, keyword, platform),
                "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("player_creations.xml")]
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
        [Route("player_creations/verify.xml")]
        public IActionResult Verify(List<int> id, List<int> offline_id)
        {
            return Content(PlayerCreations.VerifyPlayerCreations(database, id, offline_id), "application/xml;charset=utf-8");
        }

        private List<string> AcceptedTypes = new List<string> {
            "data.bin", "data.jpg", "preview_image.png", "preview_image_128x128.png", "preview_image_64x64.png"
        };

        [HttpGet]
        [Route("player_creations/{id}/{file}")]
        public IActionResult GetData(int id, string file)
        {
            if (!AcceptedTypes.Contains(file)) return NotFound();
            var data = UserGeneratedContentUtils.LoadPlayerCreation(id, file);
            if (data == null)
                return NotFound();
            Response.Headers.Add("ETag", $"\"{UserGeneratedContentUtils.CalculateMD5(data)}\"");
            Response.Headers.Add("Accept-Ranges", "bytes");
            Response.Headers.Add("Cache-Control", "private, max-age=0, must-revalidate");
            string contentType = "application/octet-stream";
            if (file.EndsWith(".png"))
                contentType = "image/png";
            if (file.EndsWith(".jpg"))
                contentType = "image/jpeg";
            return File(data, contentType);
        }
    }
}
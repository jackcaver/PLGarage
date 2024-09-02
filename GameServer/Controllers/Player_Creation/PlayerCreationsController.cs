using System.Collections.Generic;
using GameServer.Implementation.Player_Creation;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using GameServer.Implementation.Common;
using Microsoft.AspNetCore.Http;
using GameServer.Models.Config;
using System.IO;

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
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);

            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            filters.player_id = Request.Query.Keys.Contains("filters[player_id]") ? Request.Query["filters[player_id]"].ToString().Split(',') : null;
            PlayerCreationType TypeFilter = PlayerCreationType.TRACK;
            Enum.TryParse(Request.Query["filters[player_creation_type]"], out TypeFilter);
            filters.player_creation_type = TypeFilter;

            return Content(PlayerCreations.SearchPlayerCreations(database, SessionID, page, per_page, sort_column, sort_order, limit, platform, filters, null, false, true, true),
                "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("player_creations/surprise_me.xml")]
        public IActionResult SurpriseMe(int page, int per_page, SortColumn sort_column, SortOrder sort_order, int limit, Platform platform, Filters filters)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);

            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            filters.player_id = Request.Query.Keys.Contains("filters[player_id]") ? Request.Query["filters[player_id]"].ToString().Split(',') : null;
            PlayerCreationType TypeFilter = PlayerCreationType.TRACK;
            Enum.TryParse(Request.Query["filters[player_creation_type]"], out TypeFilter);
            filters.player_creation_type = TypeFilter;
            if (SessionID != Guid.Empty && platform == Platform.PS2)
                platform = Session.GetSession(SessionID).Platform;

            return Content(PlayerCreations.SearchPlayerCreations(database, SessionID, page, per_page, sort_column, sort_order, limit, platform, filters, null, false, true, true),
                "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("player_creations/team_picks.xml")]
        public IActionResult TeamPicks(int page, int per_page, SortColumn sort_column, SortOrder sort_order, int limit, Platform platform, Filters filters)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);

            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            filters.player_id = Request.Query.Keys.Contains("filters[player_id]") ? Request.Query["filters[player_id]"].ToString().Split(',') : null;
            PlayerCreationType TypeFilter = PlayerCreationType.TRACK;
            Enum.TryParse(Request.Query["filters[player_creation_type]"], out TypeFilter);
            filters.player_creation_type = TypeFilter;

            return Content(PlayerCreations.SearchPlayerCreations(database, SessionID, page, per_page, sort_column, sort_order, limit, platform, filters, null, true, false, true),
                "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("player_creations/friends_view.xml")]
        [Route("player_creations/friends_and_favorites_view.xml")]
        public IActionResult FriendsView(int page, int per_page, SortColumn sort_column, SortOrder sort_order, int limit, Platform platform, Filters filters)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);

            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            filters.player_id = Request.Query.Keys.Contains("filters[player_id]") ? Request.Query["filters[player_id]"].ToString().Split(',') : null;
            PlayerCreationType TypeFilter = PlayerCreationType.TRACK;
            Enum.TryParse(Request.Query["filters[player_creation_type]"], out TypeFilter);
            filters.player_creation_type = TypeFilter;
            filters.username = Request.Query.Keys.Contains("filters[username]") ? Request.Query["filters[username]"].ToString().Split(',') : null;

            return Content(PlayerCreations.SearchPlayerCreations(database, SessionID, page, per_page, sort_column, sort_order, limit, platform, filters, null, false, false, true),
                "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("player_creations/search.xml")]
        [Route("player_creations.xml")]
        public IActionResult Search(int page, int per_page, SortColumn sort_column, SortOrder sort_order, string search, string search_tags, 
            string username, PlayerCreationType? player_creation_type, bool? is_remixable, bool? ai, bool? auto_reset, int limit, 
            Platform platform, Filters filters)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);

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

            return Content(PlayerCreations.SearchPlayerCreations(database, SessionID, page, per_page, sort_column, sort_order, limit, platform, filters, search, false, false, true),
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

        private List<string> AcceptedTypes = [
            "data.bin", "data.jpg", "preview_image.png", "preview_image_128x128.png", "preview_image_64x64.png"
        ];

        [HttpGet]
        [Route("player_creations/{id}/{file}")]
        public IActionResult GetData(int id, string file)
        {
            if (!AcceptedTypes.Contains(file)) return NotFound();
            var data = UserGeneratedContentUtils.LoadPlayerCreation(id, file);
            
            if (data == null && ServerConfig.Instance.EnablePlaceholderImage)
            {
                Stream placeholder;
                
                if (!System.IO.File.Exists("./placeholder.png"))
                    System.IO.File.WriteAllBytes("./placeholder.png", Convert.FromBase64String("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"));

                placeholder = System.IO.File.OpenRead("./placeholder.png");

                if (file == AcceptedTypes[2])
                    data = UserGeneratedContentUtils.Resize(placeholder, 256, 256);

                if (file == AcceptedTypes[3])
                    data = UserGeneratedContentUtils.Resize(placeholder, 128, 128);

                if (file == AcceptedTypes[4])
                    data = UserGeneratedContentUtils.Resize(placeholder, 64, 64);
            }

            if (data == null)
                return NotFound();

            Response.Headers.Append("ETag", $"\"{UserGeneratedContentUtils.CalculateMD5(data)}\"");
            Response.Headers.Append("Accept-Ranges", "bytes");
            Response.Headers.Append("Cache-Control", "private, max-age=0, must-revalidate");

            string contentType = "application/octet-stream";

            if (file.EndsWith(".png"))
                contentType = "image/png";
            if (file.EndsWith(".jpg"))
                contentType = "image/jpeg";

            return File(data, contentType);
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Implementation.Common;
using GameServer.Implementation.Player_Creation;
using GameServer.Models;
using GameServer.Models.Config;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player_Creation
{
    public class PlayerCreationsController(Database database, IUGCStorage storage) : Controller
    {
        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("player_creations/{id}.xml")]
        public IActionResult Get(int id, bool is_counted)
        {
            var session = Session.GetSession(database, User);
            return Content(PlayerCreations.GetPlayerCreation(database, storage, session, id, is_counted), "application/xml;charset=utf-8");
        }

        [Authorize]
        [HttpDelete]
        [Route("player_creations/{id}.xml")]
        public IActionResult Delete(int id)
        {
            var user = Session.GetUser(database, User);
            return Content(PlayerCreations.RemovePlayerCreation(database, storage, user, id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [AllowAnonymous]
        [Route("player_creations/{id}/download.xml")]
        public IActionResult Download(int id, bool is_counted)
        {
            var session = Session.GetSession(database, User);
            return Content(PlayerCreations.GetPlayerCreation(database, storage, session, id, is_counted, true), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("player_creations/{platform}/surprise_me.xml")]
        public IActionResult SurpriseMeOnPlatform(int page, int per_page, SortColumn sort_column, SortOrder sort_order, int limit, Platform platform, [FromQuery]Filters filters)
        {
            var user = Session.GetUser(database, User);

            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            filters.player_id = Request.Query.Keys.Contains("filters[player_id]") ? Request.Query["filters[player_id]"].ToString().Split(',') : null;
            if (Enum.TryParse(Request.Query["filters[player_creation_type]"], out PlayerCreationType TypeFilter))
                filters.player_creation_type = TypeFilter;

            return Content(PlayerCreations.SearchPlayerCreations(database, user, page, per_page, sort_column, sort_order, limit, platform, filters, null, false, true, true),
                "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("player_creations/surprise_me.xml")]
        public IActionResult SurpriseMe(int page, int per_page, SortColumn sort_column, SortOrder sort_order, int limit, Platform platform, [FromQuery]Filters filters)
        {
            var session = Session.GetSession(database, User);

            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            filters.player_id = Request.Query.Keys.Contains("filters[player_id]") ? Request.Query["filters[player_id]"].ToString().Split(',') : null;
            if (Enum.TryParse(Request.Query["filters[player_creation_type]"], out PlayerCreationType TypeFilter))
                filters.player_creation_type = TypeFilter;
            if (session.SessionId != Guid.Empty && platform == Platform.PS2)
                platform = session.Platform;

            return Content(PlayerCreations.SearchPlayerCreations(database, session.User, page, per_page, sort_column, sort_order, limit, platform, filters, null, false, true, true),
                "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("player_creations/team_picks.xml")]
        public IActionResult TeamPicks(int page, int per_page, SortColumn sort_column, SortOrder sort_order, int limit, Platform platform, [FromQuery]Filters filters)
        {
            var user = Session.GetUser(database, User);

            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            filters.player_id = Request.Query.Keys.Contains("filters[player_id]") ? Request.Query["filters[player_id]"].ToString().Split(',') : null;
            if (Enum.TryParse(Request.Query["filters[player_creation_type]"], out PlayerCreationType TypeFilter))
                filters.player_creation_type = TypeFilter;

            return Content(PlayerCreations.SearchPlayerCreations(database, user, page, per_page, sort_column, sort_order, limit, platform, filters, null, true, false, true),
                "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("player_creations/friends_view.xml")]
        [Route("player_creations/friends_and_favorites_view.xml")]
        public IActionResult FriendsView(int page, int per_page, SortColumn sort_column, SortOrder sort_order, int limit, Platform platform, [FromQuery]Filters filters)
        {
            var user = Session.GetUser(database, User);

            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            filters.player_id = Request.Query.Keys.Contains("filters[player_id]") ? Request.Query["filters[player_id]"].ToString().Split(',') : null;
            if (Enum.TryParse(Request.Query["filters[player_creation_type]"], out PlayerCreationType TypeFilter))
                filters.player_creation_type = TypeFilter;
            filters.username = Request.Query.Keys.Contains("filters[username]") ? Request.Query["filters[username]"].ToString().Split(',') : null;

            return Content(PlayerCreations.SearchPlayerCreations(database, user, page, per_page, sort_column, sort_order, limit, platform, filters, null, false, false, true),
                "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("player_creations/search.xml")]
        [Route("player_creations.xml")]
        public IActionResult Search(int page, int per_page, SortColumn sort_column, SortOrder sort_order, string search, string search_tags, 
            string username, PlayerCreationType? player_creation_type, bool? is_remixable, bool? ai, bool? auto_reset, int limit, 
            Platform platform, [FromQuery]Filters filters)
        {
            var user = Session.GetUser(database, User);

            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            filters.player_id = Request.Query.Keys.Contains("filters[player_id]") ? Request.Query["filters[player_id]"].ToString().Split(',') : null;
            Enum.TryParse(Request.Query["filters[player_creation_type]"], out PlayerCreationType TypeFilter);
            if (player_creation_type != null)
                TypeFilter = player_creation_type.Value;
            filters.player_creation_type = TypeFilter;
            filters.tags = search_tags?.Split(',');
            filters.username = username?.Split(',');
            filters.ai = ai;
            filters.is_remixable = is_remixable;
            filters.auto_reset = auto_reset;

            if (Request.Query.Keys.Contains("filters[ai]"))
                filters.ai = bool.Parse(Request.Query["filters[ai]"].ToString());
            if (Request.Query.Keys.Contains("filters[is_remixable]"))
                filters.is_remixable = bool.Parse(Request.Query["filters[is_remixable]"].ToString());
            if (Request.Query.Keys.Contains("filters[auto_reset]"))
                filters.auto_reset = bool.Parse(Request.Query["filters[auto_reset]"].ToString());

            return Content(PlayerCreations.SearchPlayerCreations(database, user, page, per_page, sort_column, sort_order, limit, platform, filters, search, false, false, true),
                "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [Route("player_creations/mine.xml")]
        public IActionResult Mine(int page, int per_page, SortColumn sort_column, SortOrder sort_order, string keyword, int limit, [FromQuery]Filters filters)
        {
            var session = Session.GetSession(database, User);
            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            if (Enum.TryParse(Request.Query["filters[player_creation_type]"], out PlayerCreationType TypeFilter))
                filters.player_creation_type = TypeFilter;

            return Content(PlayerCreations.Mine(database, session, page, per_page, sort_column, sort_order, limit, filters, keyword),
                "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [Route("player_creations/{platform}/mine.xml")]
        public IActionResult MineOnPlatform(int page, int per_page, SortColumn sort_column, SortOrder sort_order, string keyword, int limit, [FromQuery]Filters filters, Platform platform)
        {
            var session = Session.GetSession(database, User);
            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            if (Enum.TryParse(Request.Query["filters[player_creation_type]"], out PlayerCreationType TypeFilter))
                filters.player_creation_type = TypeFilter;

            return Content(PlayerCreations.Mine(database, session, page, per_page, sort_column, sort_order, limit, filters, keyword, platform),
                "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("player_creations.xml")]
        public IActionResult Create(PlayerCreation player_creation)
        {
            var session = Session.GetSession(database, User);
            player_creation.data = Request.Form.Files.GetFile("player_creation[data]");
            player_creation.preview = Request.Form.Files.GetFile("player_creation[preview]");
            return Content(PlayerCreations.CreatePlayerCreation(database, storage, session, player_creation));
        }

        [HttpPost]
        [Route("player_creations/verify.xml")]
        public IActionResult Verify(string id, string offline_id)
        {
            var ids = id != null ? id.Split(',').Select(x => int.Parse(x)).ToList() : [];
            var offline_ids = offline_id != null ? offline_id.Split(',').Select(x => int.Parse(x)).ToList() : [];
            return Content(PlayerCreations.VerifyPlayerCreations(database, ids, offline_ids), "application/xml;charset=utf-8");
        }

        private List<string> AcceptedTypes = [
            "data.bin", "data.jpg", "preview_image.png", "preview_image_128x128.png", "preview_image_64x64.png"
        ];

        [HttpGet]
        [Route("player_creations/{id}/{file}")]
        public IActionResult GetData(int id, string file)
        {
            if (!AcceptedTypes.Contains(file)) return NotFound();
            var data = storage.LoadPlayerCreation(id, file);
            
            if (data == null && ServerConfig.Instance.EnablePlaceholderImage)
            {
                if (file == AcceptedTypes[2])
                    data = System.IO.File.OpenRead("./placeholder.png");

                if (file == AcceptedTypes[3])
                    data = System.IO.File.OpenRead("./placeholder_128x128.png");

                if (file == AcceptedTypes[4])
                    data = System.IO.File.OpenRead("./placeholder_64x64.png");
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
using GameServer.Implementation.Common;
using GameServer.Implementation.Player_Creation;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player_Creation
{
    public class TracksController(Database database) : Controller
    {
        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("tracks/{id}/profile.xml")]
        public IActionResult GetProfile(int id)
        {
            var user = Session.GetUser(database, User);
            return Content(PlayerCreations.GetTrackProfile(database, user, id), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("tracks/{id}.xml")]
        public IActionResult Get(int id, bool is_counted)
        {
            var session = Session.GetSession(database, User);
            return Content(PlayerCreations.GetPlayerCreation(database, session, id, is_counted), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [AllowAnonymous]
        [Route("tracks/{id}/download.xml")]
        public IActionResult Download(int id, bool is_counted)
        {
            var session = Session.GetSession(database, User);
            return Content(PlayerCreations.GetPlayerCreation(database, session, id, is_counted, true), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("tracks/friends_published.xml")]
        public IActionResult FriendsPublished(Platform platform)
        {
            return Content(PlayerCreations.PlayerCreationsFriendsPublished(database, Request.Query["filters[username]"], PlayerCreationType.TRACK),
                "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("tracks/lucky_dip.xml")]
        public IActionResult LuckyDip(int page, int per_page, string keyword, int limit, Platform platform, [FromQuery]Filters filters)
        {
            var user = Session.GetUser(database, User);

            filters.race_type = Request.Query["filters[race_type]"];
            filters.username = Request.Query.Keys.Contains("filters[username]") ? Request.Query["filters[username]"].ToString().Split(',') : null;
            filters.tags = Request.Query.Keys.Contains("filters[tags]") ? Request.Query["filters[tags]"].ToString().Split(',') : null;
            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            filters.player_creation_type = PlayerCreationType.TRACK;
            return Content(PlayerCreations.SearchPlayerCreations(database, user, page, per_page, SortColumn.created_at, SortOrder.desc, limit, platform, filters, keyword, false, true), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("tracks/ufg_picks.xml")]
        public IActionResult GetTeamPicks(int page, int per_page, SortColumn sort_column, SortOrder sort_order, string keyword, int limit, Platform platform, [FromQuery]Filters filters)
        {
            var user = Session.GetUser(database, User);

            filters.race_type = Request.Query["filters[race_type]"];
            filters.username = Request.Query.Keys.Contains("filters[username]") ? Request.Query["filters[username]"].ToString().Split(',') : null;
            filters.tags = Request.Query.Keys.Contains("filters[tags]") ? Request.Query["filters[tags]"].ToString().Split(',') : null;
            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            filters.player_creation_type = PlayerCreationType.TRACK;
            return Content(PlayerCreations.SearchPlayerCreations(database, user, page, per_page, sort_column, sort_order, limit, platform, filters, keyword, true), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("tracks.xml")]
        public IActionResult Search(int page, int per_page, SortColumn sort_column, SortOrder sort_order, string keyword, int limit, Platform platform, [FromQuery]Filters filters)
        {
            var user = Session.GetUser(database, User);

            filters.race_type = Request.Query["filters[race_type]"];
            filters.username = Request.Query.Keys.Contains("filters[username]") ? Request.Query["filters[username]"].ToString().Split(',') : null;
            filters.tags = Request.Query.Keys.Contains("filters[tags]") ? Request.Query["filters[tags]"].ToString().Split(',') : null;
            filters.id = Request.Query.Keys.Contains("filters[id]") ? Request.Query["filters[id]"].ToString().Split(',') : null;
            filters.player_creation_type = PlayerCreationType.TRACK;
            return Content(PlayerCreations.SearchPlayerCreations(database, user, page, per_page, sort_column, sort_order, limit, platform, filters, keyword),
                "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("tracks.xml")]
        public IActionResult Create(PlayerCreation player_creation)
        {
            var session = Session.GetSession(database, User);
            player_creation.data = Request.Form.Files.GetFile("player_creation[data]");
            player_creation.preview = Request.Form.Files.GetFile("player_creation[preview]");
            return Content(PlayerCreations.CreatePlayerCreation(database, session, player_creation));
        }

        [HttpPost]
        [Authorize]
        [Route("tracks/{id}/update.xml")]
        public IActionResult Update(int id, PlayerCreation player_creation)
        {
            var session = Session.GetSession(database, User);
            player_creation.data = Request.Form.Files.GetFile("player_creation[data]");
            player_creation.preview = Request.Form.Files.GetFile("player_creation[preview]");
            return Content(PlayerCreations.UpdatePlayerCreation(database, session, player_creation, id),
                "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("tracks/{id}.xml")]
        public IActionResult Delete(int id)
        {
            var user = Session.GetUser(database, User);
            return Content(PlayerCreations.RemovePlayerCreation(database, user, id), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
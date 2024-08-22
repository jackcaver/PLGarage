using Microsoft.AspNetCore.Mvc;
using GameServer.Implementation.Player;
using GameServer.Models.Request;
using System.Globalization;
using System;
using GameServer.Utils;

namespace GameServer.Controllers.Player
{
    public class ModMileController : Controller
    {
        private readonly Database database;

        public ModMileController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("mod_mile/travel_awards.xml")]
        public IActionResult TravelAwards(int per_page, int page)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(ModMile.TravelAwards(database, SessionID, per_page, page), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("mod_mile/featured_cities.xml")]
        public IActionResult FeaturedCities(int per_page, int page)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(ModMile.FeaturedCities(database, SessionID, per_page, page), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("mod_mile/pois.xml")]
        public IActionResult POIList(int per_page, int page, int city_id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(ModMile.POIList(database, SessionID, per_page, page, city_id), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("mod_mile/pois/{id}.xml")]
        public IActionResult POIShow(int id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(ModMile.POIShow(database, SessionID, id), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("mod_mile/checkins/{id}.xml")]
        public IActionResult CheckinStatus(int id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(ModMile.CheckinStatus(database, SessionID, id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("mod_mile/checkins.xml")]
        public IActionResult CheckinCreate(float latitude, float longitude)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            string FormLatitude = Request.Form["latitude"];
            string FormLongitude = Request.Form["longitude"];
            if (FormLatitude != null)
                latitude = float.Parse(FormLatitude, CultureInfo.InvariantCulture.NumberFormat);
            if (FormLongitude != null)
                longitude = float.Parse(FormLongitude, CultureInfo.InvariantCulture.NumberFormat);

            return Content(ModMile.CheckinCreate(database, SessionID, latitude, longitude), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("mod_mile/leaderboards/cities.xml")]
        public IActionResult LeaderboardCities(int page, int per_page, Timespan timespan, SortColumn sort_column, SortOrder sort_order)
        {
            return Content(ModMile.LeaderboardCities(database, page, per_page, timespan, sort_column, sort_order), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("mod_mile/leaderboards/destinations.xml")]
        public IActionResult LeaderboardDestinations(int page, int per_page, Timespan timespan, SortColumn sort_column, SortOrder sort_order)
        {
            return Content(ModMile.LeaderboardDestinations(database, page, per_page, timespan, sort_column, sort_order), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("mod_mile/leaderboards/players.xml")]
        public IActionResult LeaderboardPlayers(int page, int per_page, Timespan timespan, SortColumn sort_column, SortOrder sort_order, string username)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(ModMile.LeaderboardPlayers(database, SessionID, page, per_page, timespan, sort_column, sort_order, username), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
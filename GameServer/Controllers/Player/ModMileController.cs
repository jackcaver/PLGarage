using GameServer.Implementation.Common;
using Microsoft.AspNetCore.Mvc;
using GameServer.Implementation.Player;
using GameServer.Models.Request;
using System.Globalization;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;

namespace GameServer.Controllers.Player
{
    public class ModMileController(Database database) : Controller
    {
        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("mod_mile/travel_awards.xml")]
        public IActionResult TravelAwards(int per_page, int page)
        {
            var session = Session.GetSession(database, User);
            return Content(ModMile.TravelAwards(database, session, per_page, page), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("mod_mile/featured_cities.xml")]
        public IActionResult FeaturedCities(int per_page, int page)
        {
            var user = Session.GetUser(database, User);
            return Content(ModMile.FeaturedCities(database, user, per_page, page), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("mod_mile/pois.xml")]
        public IActionResult POIList(int per_page, int page, int city_id)
        {
            var user = Session.GetUser(database, User);
            return Content(ModMile.POIList(database, user, per_page, page, city_id), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("mod_mile/pois/{id}.xml")]
        public IActionResult POIShow(int id)
        {
            var user = Session.GetUser(database, User);
            return Content(ModMile.POIShow(database, user, id), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("mod_mile/checkins/{id}.xml")]
        public IActionResult CheckinStatus(int id)
        {
            var session = Session.GetSession(database, User);
            return Content(ModMile.CheckinStatus(database, session, id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("mod_mile/checkins.xml")]
        public IActionResult CheckinCreate(float latitude, float longitude)
        {
            var session = Session.GetSession(database, User);
            string FormLatitude = Request.Form["latitude"];
            string FormLongitude = Request.Form["longitude"];
            if (FormLatitude != null)
                latitude = float.Parse(FormLatitude, CultureInfo.InvariantCulture.NumberFormat);
            if (FormLongitude != null)
                longitude = float.Parse(FormLongitude, CultureInfo.InvariantCulture.NumberFormat);

            return Content(ModMile.CheckinCreate(database, session, latitude, longitude), "application/xml;charset=utf-8");
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
        [Authorize]
        [AllowAnonymous]
        [Route("mod_mile/leaderboards/players.xml")]
        public IActionResult LeaderboardPlayers(int page, int per_page, Timespan timespan, SortColumn sort_column, SortOrder sort_order, string username)
        {
            var user = Session.GetUser(database, User);
            return Content(ModMile.LeaderboardPlayers(database, user, page, per_page, timespan, sort_column, sort_order, username), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
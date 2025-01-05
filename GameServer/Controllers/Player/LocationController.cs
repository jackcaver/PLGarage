using GameServer.Models.Response;
using GameServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using GameServer.Utils;
using System.Linq;
using GameServer.Implementation.Common;
using System.Globalization;

namespace GameServer.Controllers.Player
{
    public class LocationController : Controller
    {
        private readonly Database database;

        public LocationController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("locations.xml")]
        public IActionResult Lookup(float latitude, float longitude)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            var scores = database.Scores.Where(match => match.PlayerId == user.UserId
                && match.Platform == session.Platform).ToList();

            var score = scores.FirstOrDefault(match => match.LocationTag != null
                && match.Latitude.ToString("0.000", CultureInfo.InvariantCulture) == latitude.ToString("0.000", CultureInfo.InvariantCulture) //gps isn't 100% accurate so here is my way to get around it
                && match.Longitude.ToString("0.000", CultureInfo.InvariantCulture) == longitude.ToString("0.000", CultureInfo.InvariantCulture));

            var resp = new Response<List<Location>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response =
                [
                    new Location 
                    {
                        latitude = latitude,
                        longitude = longitude,
                        tag = score != null ? score.LocationTag : "",
                        is_tagged = score != null
                    }
                ]
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpDelete]
        [Route("location/delete.xml")]
        public IActionResult Delete(float latitude, float longitude)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            var session = SessionImpl.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            string FormLatitude = Request.Form["latitude"];
            string FormLongitude = Request.Form["longitude"];
            if (FormLatitude != null)
                latitude = float.Parse(FormLatitude, CultureInfo.InvariantCulture.NumberFormat);
            if (FormLongitude != null)
                longitude = float.Parse(FormLongitude, CultureInfo.InvariantCulture.NumberFormat);

            //gps isn't 100% accurate so here is my way to get around it
            latitude = float.Parse(latitude.ToString("0.000", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
            longitude = float.Parse(longitude.ToString("0.000", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

            var scores = database.Scores.Where(match => match.PlayerId == user.UserId && match.Platform == session.Platform).ToList();
            scores = scores.Where(match => match.LocationTag != null && match.Latitude == latitude 
                && match.Longitude == longitude).ToList();

            foreach (var score in scores)
            {
                score.Latitude = 0;
                score.Longitude = 0;
                score.LocationTag = null;
                var personalLeaderboard = database.Scores.Where(match => match.PlayerId == score.PlayerId
                    && match.SubKeyId == score.SubKeyId && match.SubGroupId == score.SubGroupId
                    && match.Platform == score.Platform).ToList();
                personalLeaderboard.Sort((curr, prev) => curr.BestLapTime.CompareTo(prev.BestLapTime));
                if (personalLeaderboard.FindIndex(match => match.Id == score.Id) != 0)
                    database.Scores.Remove(score);
            }

            database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}

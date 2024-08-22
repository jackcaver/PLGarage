using System;
using GameServer.Implementation.Player;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player
{
    public class ActivityLogController : Controller
    {
        private readonly Database database;

        public ActivityLogController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("announcements/{file}.png")]
        public IActionResult GetImage(Guid file)
        {
            var image = UserGeneratedContentUtils.LoadAnnouncementImage($"{file}.png");
            if (image == null)
                return NotFound();
            return File(image, "image/png");
        }

        [Route("news_feed/tally.xml")]
        public IActionResult NewsFeedTally()
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(ActivityLog.NewsFeedTally(database, SessionID), "application/xml;charset=utf-8");
        }

        [Route("news_feed.xml")]
        public IActionResult NewsFeed(int page, int per_page)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(ActivityLog.GetActivityLog(database, SessionID, page, per_page), "application/xml;charset=utf-8");
        }

        [Route("activity_log.xml")]
        public IActionResult GetActivityLog(int page, int per_page, int? player_id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(ActivityLog.GetActivityLog(database, SessionID, page, per_page, ActivityList.activity_log, player_id), "application/xml;charset=utf-8");
        }

        [Route("track_feed.xml")]
        public IActionResult TrackFeed(int? player_creation_id, int page, int per_page)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(ActivityLog.GetActivityLog(database, SessionID, page, per_page, ActivityList.activity_log, null, player_creation_id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("event.xml")]
        public IActionResult CreateEvent(string topic, ActivityList list_name, int creator_id, PlayerEvent @event)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            ActivityType activityType;
            Enum.TryParse(topic.Split("\0")[0], out activityType);
            return Content(ActivityLog.CreateEvent(database, SessionID, activityType, creator_id, @event, list_name), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
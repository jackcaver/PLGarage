using GameServer.Implementation.Common;
using System;
using GameServer.Implementation.Player;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player
{
    public class ActivityLogController(Database database) : Controller
    {
        [HttpGet]
        [Route("announcements/{file}.png")]
        public IActionResult GetImage(Guid file)
        {
            var image = UserGeneratedContentUtils.LoadAnnouncementImage($"{file}.png");
            if (image == null)
                return NotFound();
            return File(image, "image/png");
        }

        [Authorize]
        [AllowAnonymous]
        [Route("news_feed/tally.xml")]
        public IActionResult NewsFeedTally()
        {
            var user = Session.GetUser(database, User);
            return Content(ActivityLog.NewsFeedTally(database, user), "application/xml;charset=utf-8");
        }

        [Authorize]
        [AllowAnonymous]
        [Route("news_feed.xml")]
        public IActionResult NewsFeed(int page, int per_page)
        {
            var user = Session.GetUser(database, User);
            return Content(ActivityLog.GetActivityLog(database, user, page, per_page), "application/xml;charset=utf-8");
        }

        [Authorize]
        [AllowAnonymous]
        [Route("activity_log.xml")]
        public IActionResult GetActivityLog(int page, int per_page, int? player_id)
        {
            var user = Session.GetUser(database, User);
            return Content(ActivityLog.GetActivityLog(database, user, page, per_page, ActivityList.activity_log, player_id), "application/xml;charset=utf-8");
        }

        [Authorize]
        [AllowAnonymous]
        [Route("track_feed.xml")]
        public IActionResult TrackFeed(int? player_creation_id, int page, int per_page)
        {
            var user = Session.GetUser(database, User);
            return Content(ActivityLog.GetActivityLog(database, user, page, per_page, ActivityList.activity_log, null, player_creation_id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("event.xml")]
        public IActionResult CreateEvent(string topic, ActivityList list_name, int creator_id, PlayerEvent @event)
        {
            var user = Session.GetUser(database, User);
            if (!Enum.TryParse(topic.TrimEnd('\0'), out ActivityType activityType))
                activityType = ActivityType.player_event;
            return Content(ActivityLog.CreateEvent(database, user, activityType, creator_id, @event, list_name), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
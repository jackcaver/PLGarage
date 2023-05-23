using System;
using System.Collections.Generic;
using GameServer.Models;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player
{
    public class ActivityLogController : Controller
    {
        [HttpGet]
        [Route("announcements/{file}")]
        public IActionResult GetImage(string file)
        {
            var image = UserGeneratedContentUtils.LoadAnnouncementImage(file);
            if (image == null)
                return NotFound();
            return File(image, "image/png");
        }

        [Route("news_feed/tally.xml")]
        public IActionResult NewsFeedTally()
        {
            var resp = new Response<List<NewsFeedTally>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<NewsFeedTally> { new NewsFeedTally { total = 1 } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [Route("news_feed.xml")]
        public IActionResult NewsFeed(int page, int per_page)
        {
            var resp = new Response<List<activities>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<activities> { new activities { total = 1, page = page, row_end = 1, row_start = 0, total_pages = 1,
                    ActivityList = new List<activity> {
                        new activity
                        {
                            type = "system_activity",
                            events = new List<Event> {
                                new Event
                                {
                                    topic = "system_event",
                                    type = "Comming Soon",
                                    creator_id = 1,
                                    creator_username = "",
                                    details = "This feature is not implemented yet...",
                                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:sszzz"),
                                    seconds_ago = 0,
                                    tags = "",
                                    subject = "",
                                    image_url = "",
                                    image_md5 = ""
                                }
                            }
                        }
                    }
                } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [Route("activity_log.xml")]
        public IActionResult ActivityLog(int page, int per_page, int player_id)
        {
            var resp = new Response<List<activities>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<activities> { new activities { total = 0 } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [Route("track_feed.xml")]
        public IActionResult TrackFeed(int player_creation_id, int page, int per_page)
        {
            var resp = new Response<List<activities>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<activities> { new activities { total = 0 } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("event.xml")]
        public IActionResult CreateEvent(string topic, string list_name, int creator_id)
        {
            var resp = new Response<List<activities>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<activities> { new activities { total = 0 } }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
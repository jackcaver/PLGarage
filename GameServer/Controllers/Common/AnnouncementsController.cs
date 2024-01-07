using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Controllers.Common
{
    public class AnnouncementsController : Controller
    {
        private readonly Database database;

        public AnnouncementsController(Database database)
        {
            this.database = database;
        }

        [Route("announcements.xml")]
        public IActionResult List(Platform platform)
        {
            var AnnouncementList = new List<Announcement>();

            foreach (var announcement in database.Announcements.Where(match => match.Platform == platform))
            {
                AnnouncementList.Add(new Announcement
                {
                    id = announcement.Id,
                    language_code = announcement.LanguageCode,
                    created_at = announcement.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    subject = announcement.Subject,
                    text = announcement.Text,
                });
            }

            var resp = new Response<List<Announcements>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<Announcements> {
                    new Announcements
                    {
                        total = AnnouncementList.Count,
                        AnnouncementList = AnnouncementList
                    }
                }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}

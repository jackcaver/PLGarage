using GameServer.Implementation.Common;
using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Controllers.Common
{
    public class AnnouncementsController(Database database) : Controller
    {
        [Authorize]
        [AllowAnonymous]
        [Route("announcements.xml")]
        public IActionResult List(Platform? platform)
        {
            var session = Session.GetSession(database, User);

            if (platform == null)
                platform = session.Platform;

            var announcementList = database.Announcements.Where(match => match.Platform == platform)
                .Select(announcement => new Announcement
                {
                    id = announcement.Id,
                    language_code = announcement.LanguageCode,
                    created_at = announcement.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    subject = announcement.Subject,
                    text = announcement.Text,
                }).ToList();
            
            var resp = new Response<List<Announcements>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [
                    new Announcements
                    {
                        total = announcementList.Count,
                        AnnouncementList = announcementList
                    }
                ]
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

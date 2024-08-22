using System;
using GameServer.Implementation.Common;
using GameServer.Models;
using GameServer.Models.Config;
using GameServer.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Common
{
    public class ContentURLsController : Controller
    {
        [Route("content_urls.xml")]
        public IActionResult Get()
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            var session = Session.GetSession(SessionID);
            string protocol = Request.IsHttps ? "https://" : "http://";
            string serverURL = ServerConfig.Instance.ExternalURL.Replace("auto", $"{protocol}{Request.Host.Host}", System.StringComparison.CurrentCultureIgnoreCase);
            var resp = new Response<ContentURLsResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new ContentURLsResponse
                {
                    content_urls = new ContentURLs
                    {
                        total = 7,
                        server_uuid = "c139bd86-26a7-11e2-910b-02163e142639",
                        ContentURLList = [
                            new ContentURL { name = "s3_bucket", formats = "", url = $"{serverURL}/" },
                            new ContentURL { name = "player_avatars", formats = ".png", url = session.IsMNR ? $"{serverURL}/player_avatars/MNR/" : $"{serverURL}/player_avatars/" },
                            new ContentURL { name = "announcements", formats = "png", url = $"{serverURL}/announcements/" },
                            new ContentURL { name = "player_creations", formats = "data.bin, preview_image.png, data.jpg", url = $"{serverURL}/player_creations/" },
                            new ContentURL { name = "ps3_player_creations", formats = "data.bin, preview_image.png", url = $"{serverURL}/player_creations/" },
                            new ContentURL { name = "content_updates", formats = "data.bin", url = $"{serverURL}/content_updates/" },
                            new ContentURL { name = "ghost_car_data", formats = "data.bin", url = $"{serverURL}/ghost_car_data/" }
                        ]
                    },
                    magic_moment = new MagicMoment { scea = true, scee = true, sceasia = true, scej = true }
                }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
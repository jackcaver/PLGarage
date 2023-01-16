using System.Collections.Generic;
using GameServer.Models;
using GameServer.Models.Config;
using GameServer.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers
{
    public class ContentURLsController : Controller
    {
        [Route("content_urls.xml")]
        public IActionResult Get()
        {
            string protocol = Request.IsHttps ? "https://" : "http://";
            string serverURL = $"{protocol}{Request.Host.Value}:10050";
            var resp = new Response<ContentURLsResponse> {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new ContentURLsResponse {
                    content_urls = new ContentURLs {
                        total = 4, server_uuid = "c139bd86-26a7-11e2-910b-02163e142639",
                        ContentURLList = new List<ContentURL> {
                            new ContentURL { name = "s3_bucket", formats = "", url = $"{serverURL}/" },
                            new ContentURL { name = "player_avatars", formats = ".png", url = $"{serverURL}/player_avatars/" },
                            new ContentURL { name = "announcements", formats = "png", url = $"{serverURL}/announcements/" },
                            new ContentURL { name = "player_creations", formats = "data.bin, preview_image.png, data.jpg", url = $"{serverURL}/player_creations/" }
                        }
                    },
                    magic_moment = new MagicMoment { scea = true, scee = true, sceasia = true, scej = true }
                }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
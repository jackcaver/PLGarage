using GameServer.Implementation.Common;
using GameServer.Models.Config;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GameServer.Controllers.Common
{
    public class ContentUpdateController : Controller
    {
        private readonly Database database;

        public ContentUpdateController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("content_updates/latest.xml")]
        public IActionResult Latest(Platform platform, ContentUpdateType content_update_type)
        {
            string protocol = Request.IsHttps ? "https://" : "http://";
            string serverURL = ServerConfig.Instance.ExternalURL.Replace("auto", $"{protocol}{Request.Host.Host}", System.StringComparison.CurrentCultureIgnoreCase);
            return Content(ContentUpdates.GetLatest(database, platform, content_update_type, serverURL), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("content_updates/{id}/{file}")]
        public IActionResult GetData(int id, string file)
        {
            if (id == 10541 && file == "data.bin")
                return File(Convert.FromBase64String("PGRsY19zdGF0dWVzPiAKICA8ZGxjX3N0YXR1ZSBpZD0iMTIyNiIgdHlwZT0iQ0hBUkFDVEVSIiAvPiAKICA8ZGxjX3N0YXR1ZSBpZD0iMTE3IiB0eXBlPSJLQVJUIiAvPiAKPC9kbGNfc3RhdHVlcz4A"), "application/octet-stream");
            return NotFound();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameServer.Implementation.Common;
using GameServer.Models;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player
{
    public class PlayerAvatarsController : Controller
    {
        private readonly Database database;

        public PlayerAvatarsController(Database database)
        {
            this.database = database;
        }

        [HttpPut]
        [HttpPost]
        [Route("player_avatars/update.xml")]
        public IActionResult Upload(PlayerAvatar player_avatar)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            if (user != null)
            {
                Stream stream = Request.Form.Files.GetFile("player_avatar[avatar]").OpenReadStream();
                UserGeneratedContentUtils.SaveAvatar(user.UserId, player_avatar, stream, session.IsMNR);
            }
            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        private List<string> AcceptedTypes = new List<string> { 
            "primary.png", "primary_128x128.png", "secondary.png", "secondary_128x128.png", "frowny.png", "smiley.png"
        };

        [HttpGet]
        [Route("player_avatars/{id}/{file}")]
        public IActionResult GetData(int id, string file)
        {
            if (!AcceptedTypes.Contains(file)) return NotFound();
            var avatar = UserGeneratedContentUtils.LoadPlayerAvatar(id, file.ToLower());
            if (avatar == null)
                return NotFound();
            Response.Headers.Append("ETag", $"\"{UserGeneratedContentUtils.CalculateMD5(avatar)}\"");
            Response.Headers.Append("Accept-Ranges", "bytes");
            Response.Headers.Append("Cache-Control", "private, max-age=0, must-revalidate");
            return File(avatar, "image/png");
        }

        [HttpGet]
        [Route("player_avatars/MNR/{id}/{file}")]
        public IActionResult GetMNRData(int id, string file)
        {
            if (!AcceptedTypes.Contains(file)) return NotFound();
            var avatar = UserGeneratedContentUtils.LoadPlayerAvatar(id, file.ToLower(), true);
            if (avatar == null)
                return NotFound();
            Response.Headers.Append("ETag", $"\"{UserGeneratedContentUtils.CalculateMD5(avatar)}\"");
            Response.Headers.Append("Accept-Ranges", "bytes");
            Response.Headers.Append("Cache-Control", "private, max-age=0, must-revalidate");
            return File(avatar, "image/png");
        }
    }
}
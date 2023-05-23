using System;
using System.IO;
using System.Linq;
using GameServer.Implementation.Common;
using GameServer.Models;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
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
                UserGeneratedContentUtils.SaveAvatar(user.UserId, player_avatar, stream);
            }
            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("player_avatars/{id}/{file}")]
        public IActionResult GetData(int id, string file)
        {
            var avatar = UserGeneratedContentUtils.LoadPlayerAvatar(id, file.ToLower());
            if (avatar == null)
                return NotFound();
            return File(avatar, "image/png");
        }
    }
}
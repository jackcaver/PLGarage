using System.IO;
using System.Linq;
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
            var user = database.Users.FirstOrDefault(match => match.Username == Request.Cookies["username"]);
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
            var avatar = UserGeneratedContentUtils.LoadPlayerAvatar(id, file);
            if (avatar == null)
                return NotFound();
            return File(avatar, "image/png");
        }
    }
}
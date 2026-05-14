using GameServer.Implementation.Common;
using GameServer.Implementation.Player_Creation;
using GameServer.Models;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player_Creation
{
    public class PhotosController(Database database, IUGCStorage storage) : Controller
    {
        [HttpGet]
        [Route("photos/search.xml")]
        public IActionResult Search(int? track_id, string username, string associated_usernames, int page, int per_page)
        {
            return Content(PlayerCreations.SearchPhotos(database, track_id, username, associated_usernames, page, per_page), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("photos/{id}.xml")]
        public IActionResult Delete(int id)
        {
            var user = Session.GetUser(database, User);
            return Content(PlayerCreations.RemovePlayerCreation(database, storage, user, id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("photos/create.xml")]
        public IActionResult Create([FromForm]PlayerCreation photo)
        {
            var session = Session.GetSession(database, User);
            photo.data = Request.Form.Files.GetFile("photo[data]");
            return Content(PlayerCreations.CreatePlayerCreation(database, storage, session, photo));
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
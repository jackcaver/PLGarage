using GameServer.Implementation.Player_Creation;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GameServer.Controllers.Player_Creation
{
    public class PhotosController : Controller
    {
        private readonly Database database;

        public PhotosController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("photos/search.xml")]
        public IActionResult Search(int? track_id, string username, string associated_usernames, int page, int per_page)
        {
            return Content(PlayerCreations.SearchPhotos(database, track_id, username, associated_usernames, page, per_page), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("photos/{id}.xml")]
        public IActionResult Delete(int id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(PlayerCreations.RemovePlayerCreation(database, SessionID, id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("photos/create.xml")]
        public IActionResult Create(PlayerCreation photo)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            photo.data = Request.Form.Files.GetFile("photo[data]");
            return Content(PlayerCreations.CreatePlayerCreation(database, SessionID, photo));
        }
    }
}
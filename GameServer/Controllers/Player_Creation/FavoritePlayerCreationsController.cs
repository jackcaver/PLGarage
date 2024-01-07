using GameServer.Implementation.Player_Creation;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GameServer.Controllers.Player_Creation
{
    public class FavoritePlayerCreationsController : Controller
    {
        private readonly Database database;

        public FavoritePlayerCreationsController(Database database)
        {
            this.database = database;
        }

        [HttpPost]
        [Route("favorite_player_creations.xml")]
        public IActionResult Create(int player_creation_id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(FavoritePlayerCreations.AddToFavorites(database, SessionID, player_creation_id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("favorite_player_creations/{id}.xml")]
        public IActionResult Remove(int id)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(FavoritePlayerCreations.RemoveFromFavorites(database, SessionID, id), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("favorite_player_creations.xml")]
        public IActionResult Get(string player_id_or_username)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(FavoritePlayerCreations.ListFavorites(database, SessionID, player_id_or_username), "application/xml;charset=utf-8");
        }
    }
}
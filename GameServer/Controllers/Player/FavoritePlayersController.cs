using GameServer.Implementation.Player;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GameServer.Controllers.Player
{
    public class FavoritePlayersController : Controller
    {
        private readonly Database database;

        public FavoritePlayersController(Database database)
        {
            this.database = database;
        }

        [HttpPost]
        [Route("favorite_players.xml")]
        public IActionResult Create(FavoritePlayer favorite_player)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(FavoritePlayers.AddToFavorites(database, SessionID, favorite_player), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("favorite_players/remove.xml")]
        public IActionResult Remove(FavoritePlayer favorite_player)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(FavoritePlayers.RemoveFromFavorites(database, SessionID, favorite_player), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("favorite_players.xml")]
        public IActionResult Get(string player_id_or_username)
        {
            Guid SessionID = Guid.Empty;
            if (Request.Cookies.ContainsKey("session_id"))
                SessionID = Guid.Parse(Request.Cookies["session_id"]);
            return Content(FavoritePlayers.ListFavorites(database, SessionID, player_id_or_username), "application/xml;charset=utf-8");
        }
    }
}
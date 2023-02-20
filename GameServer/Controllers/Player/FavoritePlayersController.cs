using GameServer.Implementation.Player;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;

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
            return Content(FavoritePlayers.AddToFavorites(database, Request.Cookies["username"], favorite_player), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("favorite_players/remove.xml")]
        public IActionResult Remove(FavoritePlayer favorite_player)
        {
            return Content(FavoritePlayers.RemoveFromFavorites(database, Request.Cookies["username"], favorite_player), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("favorite_players.xml")]
        public IActionResult Get(string player_id_or_username)
        {
            return Content(FavoritePlayers.ListFavorites(database, Request.Cookies["username"], player_id_or_username), "application/xml;charset=utf-8");
        }
    }
}
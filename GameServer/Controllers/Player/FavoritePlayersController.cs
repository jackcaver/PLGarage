using GameServer.Implementation.Common;
using GameServer.Implementation.Player;
using GameServer.Models.Request;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player
{
    public class FavoritePlayersController(Database database) : Controller
    {
        [HttpPost]
        [Authorize]
        [Route("favorite_players.xml")]
        public IActionResult Create(FavoritePlayer favorite_player)
        {
            var session = Session.GetSession(database, User);
            return Content(FavoritePlayers.AddToFavorites(database, session, favorite_player), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [HttpDelete]
        [Route("favorite_players/remove.xml")]
        public IActionResult Remove(FavoritePlayer favorite_player)
        {
            var session = Session.GetSession(database, User);
            return Content(FavoritePlayers.RemoveFromFavorites(database, session, favorite_player), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("favorite_players.xml")]
        public IActionResult Get(string player_id_or_username, int? player_id)
        {
            var session = Session.GetSession(database, User);
            if (player_id != null)
                player_id_or_username = player_id.ToString();
            return Content(FavoritePlayers.ListFavorites(database, session, player_id_or_username), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
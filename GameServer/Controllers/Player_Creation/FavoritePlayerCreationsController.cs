using GameServer.Implementation.Player_Creation;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;

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
            return Content(FavoritePlayerCreations.AddToFavorites(database, Request.Cookies["username"], player_creation_id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Route("favorite_player_creations/{id}.xml")]
        public IActionResult Remove(int id)
        {
            return Content(FavoritePlayerCreations.RemoveFromFavorites(database, Request.Cookies["username"], id), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Route("favorite_player_creations.xml")]
        public IActionResult Get(string player_id_or_username)
        {
            return Content(FavoritePlayerCreations.ListFavorites(database, player_id_or_username), "application/xml;charset=utf-8");
        }
    }
}
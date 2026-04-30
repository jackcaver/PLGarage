using GameServer.Implementation.Common;
using GameServer.Implementation.Player_Creation;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Player_Creation
{
    public class FavoritePlayerCreationsController(Database database) : Controller
    {
        [HttpPost]
        [Authorize]
        [Route("favorite_player_creations.xml")]
        public IActionResult Create(int player_creation_id)
        {
            var session = Session.GetSession(database, User);
            return Content(FavoritePlayerCreations.AddToFavorites(database, session, player_creation_id), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("favorite_player_creations/{id}.xml")]
        public IActionResult Remove(int id)
        {
            var user = Session.GetUser(database, User);
            return Content(FavoritePlayerCreations.RemoveFromFavorites(database, user, id), "application/xml;charset=utf-8");
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("favorite_player_creations.xml")]
        public IActionResult Get(string player_id_or_username)
        {
            var session = Session.GetSession(database, User);
            return Content(FavoritePlayerCreations.ListFavorites(database, session, player_id_or_username), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
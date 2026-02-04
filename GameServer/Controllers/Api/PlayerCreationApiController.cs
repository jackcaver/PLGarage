using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Api
{
    public class PlayerCreationApiController : Controller
    {
        private readonly Database database;

        public PlayerCreationApiController(Database database)
        {
            this.database = database;
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
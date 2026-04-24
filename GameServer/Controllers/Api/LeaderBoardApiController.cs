using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Api
{
    public class LeaderBoardApiController : Controller
    {
        private readonly Database database;

        public LeaderBoardApiController(Database database)
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
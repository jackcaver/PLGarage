using GameServer.Implementation.Common;
using GameServer.Implementation.Player;
using GameServer.Models.Config;
using GameServer.Models.PlayerData;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace GameServer.Controllers.Player
{
    public class PlayerController(Database database) : Controller
    {
        [Route("players/to_id.xml")]
        public IActionResult ToID(string username)
        {
            return Content(PlayerProfiles.GetPlayerID(database, username), "application/xml;charset=utf-8");
        }

        [Authorize]
        [AllowAnonymous]
        [Route("players/{id}/info.xml")]
        public IActionResult GetPlayerInfo(int id, Platform platfom)
        {
            var session = Session.GetSession(database, User);
            return Content(PlayerProfiles.GetPlayerInfo(database, id, session), "application/xml;charset=utf-8");
        }

        [Authorize]
        [AllowAnonymous]
        [Route("players/skill_levels.xml")]
        public IActionResult GetSkillLevel()
        {
            var session = Session.GetSession(database, User);
<<<<<<< HEAD

            // Logs show that this request causes commas to get converted to %2c so levels disappear when there is more than one in the lobby
            int[] ids = string.Join(",", Request.Query["id"].ToArray())
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(value => int.TryParse(value, out int parsed) ? parsed : -1)
                .Where(parsed => parsed > 0)
                .ToArray();

            return Content(PlayerProfiles.GetSkillLevel(database, session, ids), "application/xml;charset=utf-8");
=======
            string[] id = Request.Query.Keys.Contains("id") ? Request.Query["id"].ToString().Split(',') : [];
            return Content(PlayerProfiles.GetSkillLevel(database, session, id), "application/xml;charset=utf-8");
>>>>>>> upstream/dev
        }

        [Route("skill_levels.xml")]
        public IActionResult GetSkillLevels()
        {
            return Content(SkillConfig.Instance.GetSkillLevelList(), "application/xml;charset=utf-8");
        }

        [HttpPost]
        [Authorize]
        [Route("player_experience_points.xml")]
        public IActionResult IncrementRaceXP(int delta)
        {
            var session = Session.GetSession(database, User);
            return Content(PlayerProfiles.IncrementRaceXP(database, session, delta), "application/xml;charset=utf-8");
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
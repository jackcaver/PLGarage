using GameServer.Implementation.Common;
using System.Collections.Generic;
using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GameServer.Controllers.Player
{
    public class BuddiesController(Database database) : Controller
    {
        [HttpPost]
        [Authorize]
        [Route("buddies/replicate.xml")]
        public IActionResult Replicate(string usernames, string blocked_usernames)
        {
            var user = Session.GetUser(database, User);
            
            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return Content(errorResp.Serialize(), "application/xml;charset=utf-8");
            }

            database.Buddies.Where(match => match.UserId == user.UserId).ExecuteDelete();
            database.BlockedUsers.Where(match => match.UserId == user.UserId).ExecuteDelete();

            database.SaveChanges();
            
            List<string> usernamesList = [];
            List<string> blockedUsernamesList = [];
            
            if (usernames != null)
                usernamesList.AddRange(usernames.Split(','));
            
            if (blocked_usernames != null)
                blockedUsernamesList.AddRange(blocked_usernames.Split(','));
            
            var buddies = database.Users
                .AsNoTracking()
                .Where(match => usernames.Contains(match.Username))
                .Select(u => new Buddy
                {
                    UserId = user.UserId,
                    BuddyUserId = u.UserId
                })
                .ToList();
            
            var blockedUsers = database.Users
                .AsNoTracking()
                .Where(match => blocked_usernames.Contains(match.Username))
                .Select(u => new BlockedUser
                {
                    UserId = user.UserId,
                    BlockedUserId = u.UserId
                })
                .ToList();
            
            database.Buddies.AddRange(buddies);
            database.BlockedUsers.AddRange(blockedUsers);
            database.SaveChanges();
            
            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return Content(resp.Serialize(), "application/xml;charset=utf-8");
        }
    }
}
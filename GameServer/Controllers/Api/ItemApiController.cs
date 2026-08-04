using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Utils;
using GameServer.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GameServer.Controllers.Api
{
    [ApiController]
    public class ItemApiController(Database database) : Controller
    {
        [HttpGet]
        [Route("/api/item/{id}")]
        public IActionResult GetItemById(int id)
        {
            var item = database.PlayerCreations
                .AsNoTracking()
                .Include(c => c.Author)
                .FirstOrDefault(c => c.Type == PlayerCreationType.ITEM 
                && c.PlayerCreationId == id);

            if (item == null)
                return NotFound(new { error = "error_item_not_found" });

            return Json(new
            {
                item.PlayerCreationId,
                item.AssociatedUsernames,
                item.TrackId,
                AuthorUsername = item.Author.Username,
                item.ModerationStatus,
                item.CreatedAt
            });
        }

        [HttpGet]
        [Route("/api/items/username/{username}")]
        public IActionResult GetItemsByUsername(string username, int page = 1, int perPage = 10, SortOrder? sortOrder = null)
        {
            if (page < 1) page = 1;
            if (perPage < 1) perPage = 10;
            if (perPage > 10) perPage = 10;

            var query = database.PlayerCreations
                .AsNoTracking()
                .Where(c => c.Type == PlayerCreationType.ITEM 
                && c.Author.Username == username
                && c.ModerationStatus != ModerationStatus.BANNED
                && c.ModerationStatus != ModerationStatus.ILLEGAL);

            var total = query.Count();
            var orderedQuery = ((sortOrder ?? SortOrder.desc) == SortOrder.asc)
                ? query.OrderBy(p => p.CreatedAt)
                : query.OrderByDescending(p => p.CreatedAt);

            if (total == 0)
                return NotFound(new { error = "error_items_not_found"});

            var items = orderedQuery
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .Select(c => new
                {
                    c.PlayerCreationId,
                    c.AssociatedUsernames,
                    c.TrackId,
                    c.CreatedAt
                })
                .ToList();

            return Json(new { total, items });
        }
        [HttpGet]
        [Route("/api/items")]
        public IActionResult GetItems(int page = 1, int perPage = 10, SortOrder? sortOrder = null)
        {
            if (page < 1) page = 1;
            if (perPage < 1) perPage = 10;
            if (perPage > 10) perPage = 10;

            var query = database.PlayerCreations
                .AsNoTracking()
                .Where(c => c.Type == PlayerCreationType.ITEM
                && c.ModerationStatus != ModerationStatus.BANNED
                && c.ModerationStatus != ModerationStatus.ILLEGAL);

            var orderedQuery = ((sortOrder ?? SortOrder.desc) == SortOrder.asc)
                ? query.OrderBy(p => p.CreatedAt)
                : query.OrderByDescending(p => p.CreatedAt);

            var total = query.Count();

            if (total == 0)
                return NotFound(new { error = "error_items_not_found" });

            var items = orderedQuery
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .Select(c => new
                {
                    c.PlayerCreationId,
                    c.AssociatedUsernames,
                    c.TrackId,
                    c.CreatedAt
                })
                .ToList();

            return Json(new {total, items});
        }
        
        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
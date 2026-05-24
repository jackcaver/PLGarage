using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GameServer.Controllers.Api
{
    [ApiController]
    public class PhotoApiController(Database database) : Controller
    {
        [HttpGet]
        [Route("/api/photos/{id}")]
        public IActionResult GetPhotoById(int id)
        {
            var photo = database.PlayerCreations
                .AsNoTracking()
                .Include(c => c.Author)
                .FirstOrDefault(c => c.Type == PlayerCreationType.PHOTO 
                && c.PlayerCreationId == id
                && c.ModerationStatus != ModerationStatus.BANNED
                && c.ModerationStatus != ModerationStatus.ILLEGAL);

            if (photo == null)
                return NotFound(new { error = "error_photo_not_found" });

            return Json(new
            {
                photo.PlayerCreationId,
                photo.AssociatedUsernames,
                photo.TrackId,
                AuthorUsername = photo.Author.Username,
                photo.CreatedAt
            });
        }

        [HttpGet]
        [Route("/api/photos/username/{username}")]
        public IActionResult GetPhotosByUsername(string username, int page = 1, int perPage = 10)
        {
            if (page < 1) page = 1;
            if (perPage < 1) perPage = 10;
            if (perPage > 10) perPage = 10;

            var query = database.PlayerCreations
                .AsNoTracking()
                .Where(c => c.Type == PlayerCreationType.PHOTO 
                && c.Author.Username == username
                && c.ModerationStatus != ModerationStatus.BANNED
                && c.ModerationStatus != ModerationStatus.ILLEGAL)
                .OrderByDescending(p => p.CreatedAt);

            var total = query.Count();

            if (total == 0)
                return NotFound(new { error = "error_photos_not_found"});

            var photos = query
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

            return Json(new { total, photos });
        }

        [HttpGet]
        [Route("/api/photos/track/{trackId}")]
        public IActionResult GetPhotosByTrackId(int trackId, int page = 1, int perPage = 10)
        {
            if (page < 1) page = 1;
            if (perPage < 1) perPage = 10;
            if (perPage > 10) perPage = 10;

            var query = database.PlayerCreations
                .AsNoTracking()
                .Where(c => c.Type == PlayerCreationType.PHOTO 
                && c.TrackId == trackId
                && c.ModerationStatus != ModerationStatus.BANNED
                && c.ModerationStatus != ModerationStatus.ILLEGAL)
                .OrderByDescending(p => p.CreatedAt);

            var total = query.Count();

            if (total == 0)
                return NotFound(new { error = "error_photos_not_found", trackId });

            var photos = query
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .Select(c => new
                {
                    c.PlayerCreationId,
                    c.AssociatedUsernames,
                    AuthorUsername = c.Author.Username,
                    c.CreatedAt
                })
                .ToList();

            return Json(new { total, photos });
        }

        [HttpGet]
        [Route("/api/photos/recent")]
        public IActionResult GetRecentPhotos(int page = 1, int perPage = 10)
        {
            if (page < 1) page = 1;
            if (perPage < 1) perPage = 10;
            if (perPage > 10) perPage = 10;

            var query = database.PlayerCreations
                .AsNoTracking()
                .Where(c => c.Type == PlayerCreationType.PHOTO
                && c.ModerationStatus != ModerationStatus.BANNED
                && c.ModerationStatus != ModerationStatus.ILLEGAL)
                .OrderByDescending(p => p.CreatedAt);

            var total = query.Count();

            if (total == 0)
                return NotFound(new { error = "error_photos_not_found" });

            var photos = query
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .Select(c => new
                {
                    c.PlayerCreationId,
                    c.AssociatedUsernames,
                    c.TrackId,
                    AuthorUsername = c.Author.Username,
                    c.CreatedAt
                })
                .ToList();

            return Json(new {total, photos});
        }
        
        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}
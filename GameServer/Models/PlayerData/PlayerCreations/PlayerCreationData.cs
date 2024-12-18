using GameServer.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.IO;
using System.Linq;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class PlayerCreationData
    {
        private Database _database;
        private Database database
        {
            get
            {
                if (_database != null) return _database;
                return _database = new Database();
            }
            set => _database = value;
        }

        [Key]
        public int PlayerCreationId { get; set; }
        public string Name { get; set; }
        public PlayerCreationType Type { get; set; }
        public Platform Platform { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public string AutoTags { get; set; }
        public string UserTags { get; set; }
        public bool RequiresDLC { get; set; }
        public string DLCKeys { get; set; }
        public bool IsRemixable { get; set; }
        public float LongestHangTime { get; set; }
        public float LongestDrift { get; set; }
        public List<PlayerCreationRaceStarted> DbplayerCreationRacesStarted { get; set; }
        public int RacesStarted => this.DbplayerCreationRacesStarted.Count();
        public int RacesWon { get; set; }
        public int Votes => this.DbRatings.Count(match => !IsMNR || match.Rating != 0);
        public int RacesStartedThisWeek => this.DbplayerCreationRacesStarted.Count(match => match.StartedAt >= DateTime.UtcNow.AddDays(-7) && match.StartedAt <= DateTime.UtcNow);
        public int RacesStartedThisMonth => this.DbplayerCreationRacesStarted.Count(match => match.StartedAt >= DateTime.UtcNow.AddMonths(-1) && match.StartedAt <= DateTime.UtcNow);
        public int RacesFinished { get; set; }
        public int TrackTheme { get; set; }
        public bool AutoReset { get; set; }
        public bool AI { get; set; }
        public int NumLaps { get; set; }
        public PlayerCreationSpeed Speed { get; set; }
        public RaceType RaceType { get; set; }
        public string WeaponSet { get; set; }
        public PlayerCreationDifficulty Difficulty { get; set; }
        public int BattleKillCount { get; set; }
        public int BattleTimeLimit { get; set; }
        public bool BattleFriendlyFire { get; set; }
        public int NumRacers { get; set; }
        public int MaxHumans { get; set; }
        public List<PlayerCreationUniqueRacer> DbPlayerCreationUniqueRacers { get; set; }
        public int UniqueRacerCount => this.DbPlayerCreationUniqueRacers.Count();
        public string AssociatedItemIds { get; set; }
        public bool IsTeamPick { get; set; }
        public int LevelMode { get; set; }
        public int ScoreboardMode { get; set; }
        public string AssociatedUsernames { get; set; }
        public string AssociatedCoordinates { get; set; }
        public float Coolness => (RatingUp-RatingDown)+((RacesStarted+RacesFinished)/2)+Hearts;
        public DateTime CreatedAt { get; set; }
        public List<PlayerCreationDownload> DbPlayerCreationDownloads { get; set; }
        public int Downloads => this.DbPlayerCreationDownloads.Count();
        public int DownloadsLastWeek => this.DbPlayerCreationDownloads.Count(match => match.DownloadedAt >= DateTime.UtcNow.AddDays(-14) && match.DownloadedAt <= DateTime.UtcNow.AddDays(-7));
        public int DownloadsThisWeek => this.DbPlayerCreationDownloads.Count(match => match.DownloadedAt >= DateTime.UtcNow.AddDays(-7) && match.DownloadedAt <= DateTime.UtcNow);
        public DateTime FirstPublished { get; set; }
        public List<HeartedPlayerCreation> DbHeartedPlayerCreations { get; set; }
        public int Hearts => this.DbHeartedPlayerCreations.Count();
        public int HeartsThisWeek => this.DbHeartedPlayerCreations.Count(match => match.HeartedAt >= DateTime.UtcNow.AddDays(-7) && match.HeartedAt <= DateTime.UtcNow);
        public int HeartsThisMonth => this.DbHeartedPlayerCreations.Count(match => match.HeartedAt >= DateTime.UtcNow.AddMonths(-1) && match.HeartedAt <= DateTime.UtcNow);
        public DateTime LastPublished { get; set; }
        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public User Author { get; set; }

        public int Rank => GetRank();
        public List<PlayerCreationRatingData> DbRatings { get; set; }
        public int RatingDown => this.DbRatings.Count(match => match.Type == RatingType.BOO);
        public int RatingUp => this.DbRatings.Count(match => match.Type == RatingType.YAY);
        public int RatingUpThisWeek => this.DbRatings.Count(match => match.Type == RatingType.YAY && match.RatedAt >= DateTime.UtcNow.AddDays(-7) && match.RatedAt <= DateTime.UtcNow);
        public int RatingUpThisMonth => this.DbRatings.Count(match => match.Type == RatingType.YAY && match.RatedAt >= DateTime.UtcNow.AddMonths(-1) && match.RatedAt <= DateTime.UtcNow);
        public DateTime UpdatedAt { get; set; }
        public string Username => this.Author.Username;
        public int Version { get; set; }
        public List<PlayerCreationView> DbViews { get; set; }
        public int Views => this.DbViews.Count(match => match.PlayerCreationId == PlayerCreationId);
        public int ViewsLastWeek => this.DbViews.Count(match => match.PlayerCreationId == PlayerCreationId && match.ViewedAt >= DateTime.UtcNow.AddDays(-14) && match.ViewedAt <= DateTime.UtcNow.AddDays(-7));
        public int ViewsThisWeek => this.DbViews.Count(match => match.PlayerCreationId == PlayerCreationId && match.ViewedAt >= DateTime.UtcNow.AddDays(-7) && match.ViewedAt <= DateTime.UtcNow);
        public int TrackId { get; set; }
        public ModerationStatus ModerationStatus { get; set; }
        //MNR
        public bool IsMNR { get; set; }
        public List<PlayerCreationPoint> DbPoints { get; set; }
        public float Points => this.DbPoints.Sum(p => p.Amount);
        public float PointsLastWeek => this.DbPoints.Where(match => match.CreatedAt >= DateTime.UtcNow.AddDays(-14) && match.CreatedAt <= DateTime.UtcNow.AddDays(-7)).Sum(p => p.Amount);
        public float PointsThisWeek => this.DbPoints.Where(match => match.CreatedAt >= DateTime.UtcNow.AddDays(-7) && match.CreatedAt <= DateTime.UtcNow).Sum(p => p.Amount);
        public float PointsToday => this.DbPoints.Where(match => match.CreatedAt >= DateTime.UtcNow.Date && match.CreatedAt <= DateTime.UtcNow).Sum(p => p.Amount);
        public float PointsYesterday => this.DbPoints.Where(match => match.CreatedAt >= DateTime.UtcNow.AddDays(-1).Date && match.CreatedAt <= DateTime.UtcNow.Date).Sum(p => p.Amount);
        public float Rating => this.DbRatings.Count(match => match.PlayerCreationId == PlayerCreationId) != 0 ? (float)this.database.PlayerCreationRatings.Where(match => match.PlayerCreationId == PlayerCreationId).Average(r => r.Rating) : 0;
        public string StarRating => this.Rating.ToString("0.0", CultureInfo.InvariantCulture);
        public int ParentCreationId { get; set; }
        public int ParentPlayerId { get; set; }
        public int OriginalPlayerId { get; set; }
        public float BestLapTime { get; set; }
        public bool HasPreview => File.Exists($"UGC/PlayerCreations/{PlayerCreationId}/preview_image.png");

        public bool IsHeartedByMe(int id)
        {
            var entry = this.DbHeartedPlayerCreations.FirstOrDefault(match => match.HeartedPlayerCreationId == this.PlayerCreationId && match.UserId == id);
            return entry != null;
        }

        public bool IsBookmarkedByMe(int id)
        {
            var entry = this.database.PlayerCreationBookmarks.FirstOrDefault(match => match.BookmarkedPlayerCreationId == this.PlayerCreationId && match.UserId == id);
            return entry != null;
        }

        public bool IsReviewedByMe(int id)
        {
            var entry = this.database.PlayerCreationReviews.FirstOrDefault(match => match.PlayerCreationId == this.PlayerCreationId && match.PlayerId == id);
            return entry != null;
        }

        public int GetRank()
        {
            var creations = this.database.PlayerCreations.Include(x => x.DbPoints).Where(match => match.Type == this.Type).ToList();
            creations.Sort((curr, prev) => prev.Points.CompareTo(curr.Points));
            return creations.FindIndex(match => match == this) + 1;
        }
    }
}

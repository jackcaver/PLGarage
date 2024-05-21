using GameServer.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
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
        public int RacesStarted => this.database.PlayerCreationRacesStarted.Count(match => match.PlayerCreationId == PlayerCreationId);
        public int RacesWon { get; set; }
        public int Votes => this.database.PlayerCreationRatings.Count(match => match.PlayerCreationId == PlayerCreationId && (!IsMNR || match.Rating != 0));
        public int RacesStartedThisWeek => this.database.PlayerCreationRacesStarted.Count(match => match.PlayerCreationId == PlayerCreationId && match.StartedAt >= DateTime.UtcNow.AddDays(-7) && match.StartedAt <= DateTime.UtcNow);
        public int RacesStartedThisMonth => this.database.PlayerCreationRacesStarted.Count(match => match.PlayerCreationId == PlayerCreationId && match.StartedAt >= DateTime.UtcNow.AddMonths(-1) && match.StartedAt <= DateTime.UtcNow);
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
        public int UniqueRacerCount => this.database.PlayerCreationUniqueRacers.Count(match => match.PlayerCreationId == this.PlayerCreationId);
        public string AssociatedItemIds { get; set; }
        public bool IsTeamPick { get; set; }
        public int LevelMode { get; set; }
        public int ScoreboardMode { get; set; }
        public string AssociatedUsernames { get; set; }
        public string AssociatedCoordinates { get; set; }
        public float Coolness => (RatingUp-RatingDown)+((RacesStarted+RacesFinished)/2)+Hearts;
        public DateTime CreatedAt { get; set; }
        public int Downloads => this.database.PlayerCreationDownloads.Count(match => match.PlayerCreationId == PlayerCreationId);
        public int DownloadsLastWeek => this.database.PlayerCreationDownloads.Count(match => match.PlayerCreationId == PlayerCreationId && match.DownloadedAt >= DateTime.UtcNow.AddDays(-14) && match.DownloadedAt <= DateTime.UtcNow.AddDays(-7));
        public int DownloadsThisWeek => this.database.PlayerCreationDownloads.Count(match => match.PlayerCreationId == PlayerCreationId && match.DownloadedAt >= DateTime.UtcNow.AddDays(-7) && match.DownloadedAt <= DateTime.UtcNow);
        public DateTime FirstPublished { get; set; }
        public int Hearts => this.database.HeartedPlayerCreations.Count(match => match.HeartedPlayerCreationId == PlayerCreationId);
        public int HeartsThisWeek => this.database.HeartedPlayerCreations.Count(match => match.HeartedPlayerCreationId == PlayerCreationId && match.HeartedAt >= DateTime.UtcNow.AddDays(-7) && match.HeartedAt <= DateTime.UtcNow);
        public int HeartsThisMonth => this.database.HeartedPlayerCreations.Count(match => match.HeartedPlayerCreationId == PlayerCreationId && match.HeartedAt >= DateTime.UtcNow.AddMonths(-1) && match.HeartedAt <= DateTime.UtcNow);
        public DateTime LastPublished { get; set; }
        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public User Author { get; set; }

        public int Rank { get; set; }
        public int RatingDown => this.database.PlayerCreationRatings.Count(match => match.PlayerCreationId == PlayerCreationId && match.Type == RatingType.BOO);
        public int RatingUp => this.database.PlayerCreationRatings.Count(match => match.PlayerCreationId == PlayerCreationId && match.Type == RatingType.YAY);
        public int RatingUpThisWeek => this.database.PlayerCreationRatings.Count(match => match.PlayerCreationId == PlayerCreationId && match.Type == RatingType.YAY && match.RatedAt >= DateTime.UtcNow.AddDays(-7) && match.RatedAt <= DateTime.UtcNow);
        public int RatingUpThisMonth => this.database.PlayerCreationRatings.Count(match => match.PlayerCreationId == PlayerCreationId && match.Type == RatingType.YAY && match.RatedAt >= DateTime.UtcNow.AddMonths(-1) && match.RatedAt <= DateTime.UtcNow);
        public DateTime UpdatedAt { get; set; }
        public string Username => this.database.Users.FirstOrDefault(match => match.UserId == this.PlayerId).Username;
        public int Version { get; set; }
        public int Views => this.database.PlayerCreationViews.Count(match => match.PlayerCreationId == PlayerCreationId);
        public int ViewsLastWeek => this.database.PlayerCreationViews.Count(match => match.PlayerCreationId == PlayerCreationId && match.ViewedAt >= DateTime.UtcNow.AddDays(-14) && match.ViewedAt <= DateTime.UtcNow.AddDays(-7));
        public int ViewsThisWeek => this.database.PlayerCreationViews.Count(match => match.PlayerCreationId == PlayerCreationId && match.ViewedAt >= DateTime.UtcNow.AddDays(-7) && match.ViewedAt <= DateTime.UtcNow);
        public int TrackId { get; set; }
        public ModerationStatus ModerationStatus { get; set; }
        //MNR
        public bool IsMNR { get; set; }
        public float Points => this.database.PlayerCreationPoints.Where(match => match.PlayerCreationId == PlayerCreationId).Sum(p => p.Amount);
        public float PointsLastWeek => this.database.PlayerCreationPoints.Where(match => match.PlayerCreationId == PlayerCreationId && match.CreatedAt >= DateTime.UtcNow.AddDays(-14) && match.CreatedAt <= DateTime.UtcNow.AddDays(-7)).Sum(p => p.Amount);
        public float PointsThisWeek => this.database.PlayerCreationPoints.Where(match => match.PlayerCreationId == PlayerCreationId && match.CreatedAt >= DateTime.UtcNow.AddDays(-7) && match.CreatedAt <= DateTime.UtcNow).Sum(p => p.Amount);
        public float PointsToday => this.database.PlayerCreationPoints.Where(match => match.PlayerCreationId == PlayerCreationId && match.CreatedAt >= DateTime.UtcNow.Date && match.CreatedAt <= DateTime.UtcNow).Sum(p => p.Amount);
        public float PointsYesterday => this.database.PlayerCreationPoints.Where(match => match.PlayerCreationId == PlayerCreationId && match.CreatedAt >= DateTime.UtcNow.AddDays(-1).Date && match.CreatedAt <= DateTime.UtcNow.Date).Sum(p => p.Amount);
        public float Rating => this.database.PlayerCreationRatings.Count(match => match.PlayerCreationId == PlayerCreationId) != 0 ? (float)this.database.PlayerCreationRatings.Where(match => match.PlayerCreationId == PlayerCreationId).Average(r => r.Rating) : 0;
        public string StarRating => this.Rating.ToString("0.0", CultureInfo.InvariantCulture);
        public int ParentCreationId { get; set; }
        public int ParentPlayerId { get; set; }
        public int OriginalPlayerId { get; set; }
        public float BestLapTime { get; set; }

        public bool IsHeartedByMe(int id)
        {
            var entry = this.database.HeartedPlayerCreations.FirstOrDefault(match => match.HeartedPlayerCreationId == this.PlayerCreationId && match.UserId == id);
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
    }
}

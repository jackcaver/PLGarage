using GameServer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class PlayerCreationData
    {
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
        public int RacesWon { get; set; }
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
        public string AssociatedItemIds { get; set; }
        public bool IsTeamPick { get; set; }
        public int LevelMode { get; set; }
        public int ScoreboardMode { get; set; }
        public string AssociatedUsernames { get; set; }
        public string AssociatedCoordinates { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime FirstPublished { get; set; }
        public DateTime LastPublished { get; set; }
        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public User Author { get; set; }

        public DateTime UpdatedAt { get; set; }
        public int Version { get; set; }
        public int TrackId { get; set; }
        public ModerationStatus ModerationStatus { get; set; }
        public bool IsMNR { get; set; }
        public int ParentCreationId { get; set; }
        public int ParentPlayerId { get; set; }
        public int OriginalPlayerId { get; set; }
        public float BestLapTime { get; set; }
        public bool HasPreview { get; set; } = true;    // Set as true by default as all creations uploaded via server will

        //DB stuff
        public List<PlayerCreationRaceStarted> RacesStarted { get; set; }
        public List<PlayerCreationUniqueRacer> UniqueRacers { get; set; }
        public List<PlayerCreationDownload> Downloads { get; set; }
        public List<HeartedPlayerCreation> Hearts { get; set; }
        public List<PlayerCreationRatingData> Ratings { get; set; }
        public List<PlayerCreationView> Views { get; set; }
        public List<PlayerCreationPoint> Points { get; set; }
        public List<PlayerCreationCommentData> Comments { get; set; }
        public List<PlayerCreationBookmark> Bookmarks { get; set; }
        public List<PlayerCreationReview> Reviews { get; set; }
        public List<Score> Scores { get; set; }

        public int RacesStartedCount => RacesStarted.Count;
        public int Votes => Ratings.Count(match => !IsMNR || match.Rating != 0);
        public int RacesStartedThisWeek => RacesStarted.Count(match => match.StartedAt >= TimeUtils.ThisWeekStart);
        public int RacesStartedThisMonth => RacesStarted.Count(match => match.StartedAt >= TimeUtils.ThisMonthStart);
        public int UniqueRacerCount => UniqueRacers.Count;
        public float Coolness => (RatingUp - RatingDown) + ((RacesStartedCount + RacesFinished) / 2) + HeartsCount;
        public int DownloadsCount => Downloads.Count;
        public int DownloadsLastWeek => Downloads.Count(match => match.DownloadedAt >= TimeUtils.LastWeekStart && match.DownloadedAt < TimeUtils.ThisWeekStart);
        public int DownloadsThisWeek => Downloads.Count(match => match.DownloadedAt >= TimeUtils.ThisWeekStart);
        public int HeartsCount => Hearts.Count;
        public int HeartsThisWeek => Hearts.Count(match => match.HeartedAt >= TimeUtils.ThisWeekStart);
        public int HeartsThisMonth => Hearts.Count(match => match.HeartedAt >= TimeUtils.ThisMonthStart);
        public int Rank => GetRank();
        public int RatingDown => Ratings.Count(match => match.Type == RatingType.BOO);
        public int RatingUp => Ratings.Count(match => match.Type == RatingType.YAY);
        public int RatingUpThisWeek => Ratings.Count(match => match.Type == RatingType.YAY && match.RatedAt >= TimeUtils.ThisWeekStart);
        public int RatingUpThisMonth => Ratings.Count(match => match.Type == RatingType.YAY && match.RatedAt >= TimeUtils.ThisMonthStart);
        public string Username => Author.Username;
        public int ViewsCount => Views.Count;
        public int ViewsLastWeek => Views.Count(match => match.ViewedAt >= TimeUtils.LastWeekStart && match.ViewedAt < TimeUtils.ThisWeekStart);
        public int ViewsThisWeek => Views.Count(match => match.ViewedAt >= TimeUtils.ThisWeekStart);
        public float PointsAmount => Points.Sum(p => p.Amount);
        public float PointsLastWeek => Points.Where(match => match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        public float PointsThisWeek => Points.Where(match => match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        public float PointsToday => Points.Where(match => match.CreatedAt >= TimeUtils.DayStart).Sum(p => p.Amount);
        public float PointsYesterday => Points.Where(match => match.CreatedAt >= TimeUtils.YesterdayStart && match.CreatedAt < TimeUtils.DayStart).Sum(p => p.Amount);
        public float Rating => Ratings.Count != 0 ? (float)Ratings.Average(r => r.Rating) : 0;
        public string StarRating => Rating.ToString("0.0", CultureInfo.InvariantCulture);

        public bool IsHeartedByMe(int id)
        {
            return Hearts.Any(match => match.UserId == id);
        }

        public bool IsBookmarkedByMe(int id)
        {
            return Bookmarks.Any(match => match.UserId == id);
        }

        public bool IsReviewedByMe(int id)
        {
            return Reviews.Any(match => match.PlayerId == id);
        }

        public int GetRank()
        {
            using var database = new Database();
            var creations = database.PlayerCreations
                .Where(match => match.Type == this.Type)
                .OrderBy(c => c.Points.Sum(p => p.Amount))
                .Select(c => c.PlayerCreationId)
                .ToList();

            return creations.FindIndex(match => match == PlayerCreationId) + 1;
        }
    }
}

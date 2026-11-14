using EntityFrameworkCore.Projectables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    //TODO: figure out indexing on this table
    /*[Index(nameof(PlayerId))]//Hopefully will prevent ef from attempting to delete FK
    [Index(nameof(ModerationStatus), nameof(CreatedAt), Name = "ModAPISearch")]
    [Index(nameof(Platform), nameof(IsMNR), nameof(Type), nameof(Username), nameof(PlayerCreationId),
        nameof(PlayerId), nameof(ModerationStatus), nameof(Name), nameof(RaceType), nameof(Tags), nameof(AutoReset), nameof(AI), 
        nameof(IsRemixable), nameof(IsTeamPick), nameof(HasPreview), nameof(Coolness), nameof(CreatedAt), nameof(RacesStarted), 
        nameof(RacesStartedThisWeek), nameof(RacesStartedThisMonth), nameof(RatingUp), nameof(RatingUpThisWeek), 
        nameof(RatingUpThisMonth), nameof(Hearts), nameof(HeartsThisWeek), nameof(HeartsThisMonth), Name = "GameSearch1")]
    [Index(nameof(Platform), nameof(IsMNR), nameof(Type), nameof(Username), nameof(PlayerCreationId),
        nameof(PlayerId), nameof(ModerationStatus), nameof(Name), nameof(RaceType), nameof(Tags), nameof(AutoReset), nameof(AI), 
        nameof(IsRemixable), nameof(IsTeamPick), nameof(HasPreview), nameof(Rating), nameof(Points), nameof(PointsToday), 
        nameof(PointsYesterday), nameof(PointsThisWeek), nameof(PointsLastWeek), nameof(Downloads), nameof(DownloadsThisWeek), 
        nameof(DownloadsLastWeek), nameof(Views), nameof(ViewsThisWeek), nameof(ViewsLastWeek), Name = "GameSearch2")]
    [Index(nameof(Type), nameof(ModerationStatus), nameof(Platform), nameof(IsMNR), 
        nameof(Name), nameof(Username), nameof(AssociatedUsernames), nameof(TrackId), Name = "APISearch")]
    [Index(nameof(TrackId), nameof(Type), Name = "PhotoSearch")]*/
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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime FirstPublished { get; set; }
        public DateTime LastPublished { get; set; }
        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public User Author { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
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
        public List<PlayerCreationRaceStarted> RacesStartedData { get; set; }
        public List<PlayerCreationUniqueRacer> UniqueRacersData { get; set; }
        public List<PlayerCreationDownload> DownloadsData { get; set; }
        public List<HeartedPlayerCreation> HeartsData { get; set; }
        public List<PlayerCreationRatingData> RatingsData { get; set; }
        public List<PlayerCreationView> ViewsData { get; set; }
        public List<PlayerCreationPoint> PointsData { get; set; }
        public List<PlayerCreationCommentData> Comments { get; set; }
        public List<PlayerCreationBookmark> Bookmarks { get; set; }
        public List<PlayerCreationReview> Reviews { get; set; }
        public List<Score> Scores { get; set; }
        public List<ActivityEvent> ActivityLog { get; set; }

        public int RacesStarted { get; set; }
        public int Votes { get; set; }
        public int RacesStartedThisWeek { get; set; }
        public int RacesStartedThisMonth { get; set; }
        public int UniqueRacers { get; set; }
        public float Coolness { get; set; }
        public int Downloads { get; set; }
        public int DownloadsLastWeek { get; set; }
        public int DownloadsThisWeek { get; set; }
        public int Hearts { get; set; }
        public int HeartsThisWeek { get; set; }
        public int HeartsThisMonth { get; set; }
        public int RatingDown { get; set; }
        public int RatingUp { get; set; }
        public int RatingUpThisWeek { get; set; }
        public int RatingUpThisMonth { get; set; }
        public string Username { get; set; }
        public int Views { get; set; }
        public int ViewsLastWeek { get; set; }
        public int ViewsThisWeek { get; set; }
        public float Points { get; set; }
        public float PointsLastWeek { get; set; }
        public float PointsThisWeek { get; set; }
        public float PointsToday { get; set; }
        public float PointsYesterday { get; set; }
        public float Rating { get; set; }
        [Projectable]
        public string StarRating => Rating.ToString("0.0", CultureInfo.InvariantCulture);
        [Projectable]
        public bool IsHeartedByMe(int id) => HeartsData.Any(match => match.UserId == id);
        [Projectable]
        public bool IsBookmarkedByMe(int id) => Bookmarks.Any(match => match.UserId == id);
        [Projectable]
        public bool IsReviewedByMe(int id) => Reviews.Any(match => match.PlayerId == id);
    }
}

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
        public List<PlayerCreationRaceStarted> RacesStarted { get; set; }
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
        public List<PlayerCreationUniqueRacer> UniqueRacers { get; set; }
        public string AssociatedItemIds { get; set; }
        public bool IsTeamPick { get; set; }
        public int LevelMode { get; set; }
        public int ScoreboardMode { get; set; }
        public string AssociatedUsernames { get; set; }
        public string AssociatedCoordinates { get; set; }
        //public float Coolness => (RatingUp-Ratings)+((RacesStarted+RacesFinished)/2)+Hearts;
        public DateTime CreatedAt { get; set; }
        public List<PlayerCreationDownload> Downloads { get; set; }
        public DateTime FirstPublished { get; set; }
        public List<HeartedPlayerCreation> Hearts { get; set; }
        public DateTime LastPublished { get; set; }
        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public User Author { get; set; }

        //public int Rank => GetRank();
        public List<PlayerCreationRatingData> Ratings { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Version { get; set; }
        public List<PlayerCreationView> Views { get; set; }
        public int TrackId { get; set; }
        public ModerationStatus ModerationStatus { get; set; }
        //MNR
        public bool IsMNR { get; set; }
        public List<PlayerCreationPoint> Points { get; set; }
        public int ParentCreationId { get; set; }
        public int ParentPlayerId { get; set; }
        public int OriginalPlayerId { get; set; }
        public float BestLapTime { get; set; }
        public bool HasPreview { get; set; } = true;    // Set as true by default as all creations uploaded via server will

        public List<PlayerCreationCommentData> Comments { get; set; }
        public List<PlayerCreationBookmark> Bookmarks { get; set; }
        public List<PlayerCreationReview> Reviews { get; set; }
        public List<Score> Scores { get; set; }
        public List<ActivityEvent> ActivityLog { get; set; }
    }
}

using GameServer.Models.Config;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using Microsoft.EntityFrameworkCore;
using System;

namespace GameServer.Utils
{
    public class Database : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<HeartedProfile> HeartedProfiles { get; set; }
        public DbSet<PlayerCommentData> PlayerComments { get; set; }
        public DbSet<PlayerCommentRatingData> PlayerCommentRatings { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<PlayerCreationData> PlayerCreations { get; set; }
        public DbSet<PlayerCreationDownload> PlayerCreationDownloads { get; set; }
        public DbSet<PlayerCreationRaceStarted> PlayerCreationRacesStarted { get; set; }
        public DbSet<PlayerCreationView> PlayerCreationViews { get; set; }
        public DbSet<PlayerCreationCommentData> PlayerCreationComments { get; set; }
        public DbSet<PlayerCreationReview> PlayerCreationReviews { get; set; }
        public DbSet<PlayerCreationReviewRatingData> PlayerCreationReviewRatings { get; set; }
        public DbSet<PlayerCreationRatingData> PlayerCreationRatings { get; set; }
        public DbSet<PlayerCreationCommentRatingData> PlayerCreationCommentRatings { get; set; }
        public DbSet<PlayerCreationUniqueRacer> PlayerCreationUniqueRacers { get; set; }
        public DbSet<HeartedPlayerCreation> HeartedPlayerCreations { get; set; }
        public DbSet<PlayerCreationBookmark> PlayerCreationBookmarks { get; set; }
        public DbSet<GriefReportData> GriefReports { get; set; }
        public DbSet<ActivityEvent> ActivityLog { get; set; }
        public DbSet<RaceStarted> OnlineRacesStarted { get; set; }
        public DbSet<RaceFinished> OnlineRacesFinished { get; set; }
        //MNR
        public DbSet<PlayerRatingData> PlayerRatings { get; set; }
        public DbSet<PlayerCreationPoint> PlayerCreationPoints { get; set; }
        public DbSet<PlayerPoint> PlayerPoints { get; set; }
        public DbSet<PlayerExperiencePoint> PlayerExperiencePoints { get; set; }
        public DbSet<MailMessageData> MailMessages { get; set; }
        public DbSet<PlayerComplaintData> PlayerComplaints { get; set; }
        public DbSet<PlayerCreationComplaintData> PlayerCreationComplaints { get; set; }
        public DbSet<AnnouncementData> Announcements { get; set; }
        //MNR: Road Trip
        public DbSet<TravelPoint> TravelPoints { get; set; }
        public DbSet<POIVisit> POIVisits { get; set; }
        public DbSet<AwardUnlock> AwardUnlocks { get; set; }
        //Moderation
        public DbSet<Moderator> Moderators { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) =>
            options.UseMySql(ServerConfig.Instance.MysqlConnectionString, ServerVersion.AutoDetect(ServerConfig.Instance.MysqlConnectionString));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerCreationData>()
                .Property(b => b.HasPreview)
                .HasDefaultValue(true);

            base.OnModelCreating(modelBuilder);
        }
    }
}

using GameServer.Models.Config;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.Games;
using GameServer.Models.PlayerData.PlayerCreations;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Utils
{
    public class Database : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<HeartedProfile> HeartedProfiles { get; set; }
        public DbSet<PlayerCommentData> PlayerComments { get; set; }
        public DbSet<PlayerCommentRatingData> PlayerCommentRatings { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<GameData> Games { get; set; }
        public DbSet<GamePlayerData> GamePlayers { get; set; }
        public DbSet<GamePlayerStatsData> GamePlayerStats { get; set; }
        public DbSet<PlayerCreationData> PlayerCreations { get; set; }
        public DbSet<PlayerCreationDownload> PlayerCreationDownloads { get; set; }
        public DbSet<PlayerCreationView> PlayerCreationViews { get; set; }
        public DbSet<PlayerCreationCommentData> PlayerCreationComments { get; set; }
        public DbSet<PlayerCreationReview> PlayerCreationReviews { get; set; }
        public DbSet<PlayerCreationReviewRatingData> PlayerCreationReviewRatings { get; set; }
        public DbSet<PlayerCreationRatingData> PlayerCreationRatings { get; set; }
        public DbSet<PlayerCreationCommentRatingData> PlayerCreationCommentRatings { get; set; }
        public DbSet<HeartedPlayerCreation> HeartedPlayerCreations { get; set; }
        public DbSet<PlayerCreationBookmark> PlayerCreationBookmarks { get; set; }
        public DbSet<GriefReportData> GriefReports { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) =>
            options.UseMySql(ServerConfig.Instance.MysqlConnectionString, MySqlServerVersion.LatestSupportedServerVersion);
    }
}

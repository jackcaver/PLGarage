using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameServer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Presence = table.Column<int>(type: "int", nullable: false),
                    Quota = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Quote = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    OnlineRaces = table.Column<int>(type: "int", nullable: false),
                    OnlineWins = table.Column<int>(type: "int", nullable: false),
                    OnlineFinished = table.Column<int>(type: "int", nullable: false),
                    OnlineForfeit = table.Column<int>(type: "int", nullable: false),
                    OnlineDisconnected = table.Column<int>(type: "int", nullable: false),
                    WinStreak = table.Column<int>(type: "int", nullable: false),
                    LongestWinStreak = table.Column<int>(type: "int", nullable: false),
                    OnlineRacesThisWeek = table.Column<int>(type: "int", nullable: false),
                    OnlineWinsThisWeek = table.Column<int>(type: "int", nullable: false),
                    OnlineFinishedThisWeek = table.Column<int>(type: "int", nullable: false),
                    OnlineRacesLastWeek = table.Column<int>(type: "int", nullable: false),
                    OnlineWinsLastWeek = table.Column<int>(type: "int", nullable: false),
                    OnlineFinishedLastWeek = table.Column<int>(type: "int", nullable: false),
                    PolicyAccepted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GameType = table.Column<int>(type: "int", nullable: false),
                    TrackIdx = table.Column<int>(type: "int", nullable: false),
                    GameState = table.Column<int>(type: "int", nullable: false),
                    HostPlayerId = table.Column<int>(type: "int", nullable: false),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsRanked = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Users_HostPlayerId",
                        column: x => x.HostPlayerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GriefReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Context = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Reason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Comments = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BadRectTop = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BadRectBottom = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GriefReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GriefReports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HeartedProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    HeartedUserId = table.Column<int>(type: "int", nullable: false),
                    HeartedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeartedProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeartedProfiles_Users_HeartedUserId",
                        column: x => x.HeartedUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeartedProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayerComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Body = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerComments_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerComments_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayerCreations",
                columns: table => new
                {
                    PlayerCreationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tags = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AutoTags = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserTags = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RequiresDLC = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DLCKeys = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsRemixable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LongestHangTime = table.Column<float>(type: "float", nullable: false),
                    LongestDrift = table.Column<float>(type: "float", nullable: false),
                    Votes = table.Column<int>(type: "int", nullable: false),
                    TrackTheme = table.Column<int>(type: "int", nullable: false),
                    AutoReset = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AI = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    NumLaps = table.Column<int>(type: "int", nullable: false),
                    Speed = table.Column<int>(type: "int", nullable: false),
                    RaceType = table.Column<int>(type: "int", nullable: false),
                    WeaponSet = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Difficulty = table.Column<int>(type: "int", nullable: false),
                    BattleKillCount = table.Column<int>(type: "int", nullable: false),
                    BattleTimeLimit = table.Column<int>(type: "int", nullable: false),
                    BattleFriendlyFire = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    NumRacers = table.Column<int>(type: "int", nullable: false),
                    MaxHumans = table.Column<int>(type: "int", nullable: false),
                    UniqueRacerCount = table.Column<int>(type: "int", nullable: false),
                    AssociatedItemIds = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsTeamPick = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LevelMode = table.Column<int>(type: "int", nullable: false),
                    ScoreboardMode = table.Column<int>(type: "int", nullable: false),
                    AssociatedUsernames = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AssociatedCoordinates = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Coolness = table.Column<float>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FirstPublished = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastPublished = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    TrackId = table.Column<int>(type: "int", nullable: false),
                    ModerationStatus = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCreations", x => x.PlayerCreationId);
                    table.ForeignKey(
                        name: "FK_PlayerCreations_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GamePlayers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    GameState = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamePlayers_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GamePlayerStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    IsComplete = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Stat1 = table.Column<int>(type: "int", nullable: false),
                    Stat2 = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<float>(type: "float", nullable: false),
                    IsWinner = table.Column<int>(type: "int", nullable: false),
                    FinishPlace = table.Column<int>(type: "int", nullable: false),
                    FinishTime = table.Column<float>(type: "float", nullable: false),
                    LapsCompleted = table.Column<int>(type: "int", nullable: false),
                    Points = table.Column<float>(type: "float", nullable: false),
                    Volatility = table.Column<float>(type: "float", nullable: false),
                    Deviation = table.Column<float>(type: "float", nullable: false),
                    PlaygroupSize = table.Column<int>(type: "int", nullable: false),
                    NumKills = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePlayerStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamePlayerStats_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayerCommentRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    RatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PlayerCommentId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCommentRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerCommentRatings_PlayerComments_PlayerCommentId",
                        column: x => x.PlayerCommentId,
                        principalTable: "PlayerComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerCommentRatings_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HeartedPlayerCreations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    HeartedPlayerCreationId = table.Column<int>(type: "int", nullable: false),
                    HeartedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeartedPlayerCreations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeartedPlayerCreations_PlayerCreations_HeartedPlayerCreation~",
                        column: x => x.HeartedPlayerCreationId,
                        principalTable: "PlayerCreations",
                        principalColumn: "PlayerCreationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeartedPlayerCreations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayerCreationBookmarks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BookmarkedPlayerCreationId = table.Column<int>(type: "int", nullable: false),
                    BookmarkedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCreationBookmarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerCreationBookmarks_PlayerCreations_BookmarkedPlayerCrea~",
                        column: x => x.BookmarkedPlayerCreationId,
                        principalTable: "PlayerCreations",
                        principalColumn: "PlayerCreationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerCreationBookmarks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayerCreationComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Body = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    PlayerCreationId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCreationComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerCreationComments_PlayerCreations_PlayerCreationId",
                        column: x => x.PlayerCreationId,
                        principalTable: "PlayerCreations",
                        principalColumn: "PlayerCreationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerCreationComments_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayerCreationDownloads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayerCreationId = table.Column<int>(type: "int", nullable: false),
                    DownloadedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCreationDownloads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerCreationDownloads_PlayerCreations_PlayerCreationId",
                        column: x => x.PlayerCreationId,
                        principalTable: "PlayerCreations",
                        principalColumn: "PlayerCreationId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayerCreationRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    RatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PlayerCreationId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCreationRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerCreationRatings_PlayerCreations_PlayerCreationId",
                        column: x => x.PlayerCreationId,
                        principalTable: "PlayerCreations",
                        principalColumn: "PlayerCreationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerCreationRatings_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayerCreationReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Content = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PlayerCreationId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    Tags = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCreationReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerCreationReviews_PlayerCreations_PlayerCreationId",
                        column: x => x.PlayerCreationId,
                        principalTable: "PlayerCreations",
                        principalColumn: "PlayerCreationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerCreationReviews_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayerCreationViews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayerCreationId = table.Column<int>(type: "int", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCreationViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerCreationViews_PlayerCreations_PlayerCreationId",
                        column: x => x.PlayerCreationId,
                        principalTable: "PlayerCreations",
                        principalColumn: "PlayerCreationId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Scores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    PlaygroupSize = table.Column<int>(type: "int", nullable: false),
                    SubGroupId = table.Column<int>(type: "int", nullable: false),
                    SubKeyId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Points = table.Column<float>(type: "float", nullable: false),
                    FinishTime = table.Column<float>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scores_PlayerCreations_SubKeyId",
                        column: x => x.SubKeyId,
                        principalTable: "PlayerCreations",
                        principalColumn: "PlayerCreationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Scores_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayerCreationCommentRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    RatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PlayerCreationCommentId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCreationCommentRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerCreationCommentRatings_PlayerCreationComments_PlayerCr~",
                        column: x => x.PlayerCreationCommentId,
                        principalTable: "PlayerCreationComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerCreationCommentRatings_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayerCreationReviewRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    RatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PlayerCreationReviewId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCreationReviewRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerCreationReviewRatings_PlayerCreationReviews_PlayerCrea~",
                        column: x => x.PlayerCreationReviewId,
                        principalTable: "PlayerCreationReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerCreationReviewRatings_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_GamePlayers_GameId",
                table: "GamePlayers",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GamePlayerStats_GameId",
                table: "GamePlayerStats",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_HostPlayerId",
                table: "Games",
                column: "HostPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_GriefReports_UserId",
                table: "GriefReports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HeartedPlayerCreations_HeartedPlayerCreationId",
                table: "HeartedPlayerCreations",
                column: "HeartedPlayerCreationId");

            migrationBuilder.CreateIndex(
                name: "IX_HeartedPlayerCreations_UserId",
                table: "HeartedPlayerCreations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HeartedProfiles_HeartedUserId",
                table: "HeartedProfiles",
                column: "HeartedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HeartedProfiles_UserId",
                table: "HeartedProfiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCommentRatings_PlayerCommentId",
                table: "PlayerCommentRatings",
                column: "PlayerCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCommentRatings_PlayerId",
                table: "PlayerCommentRatings",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerComments_AuthorId",
                table: "PlayerComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerComments_PlayerId",
                table: "PlayerComments",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreationBookmarks_BookmarkedPlayerCreationId",
                table: "PlayerCreationBookmarks",
                column: "BookmarkedPlayerCreationId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreationBookmarks_UserId",
                table: "PlayerCreationBookmarks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreationCommentRatings_PlayerCreationCommentId",
                table: "PlayerCreationCommentRatings",
                column: "PlayerCreationCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreationCommentRatings_PlayerId",
                table: "PlayerCreationCommentRatings",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreationComments_PlayerCreationId",
                table: "PlayerCreationComments",
                column: "PlayerCreationId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreationComments_PlayerId",
                table: "PlayerCreationComments",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreationDownloads_PlayerCreationId",
                table: "PlayerCreationDownloads",
                column: "PlayerCreationId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreationRatings_PlayerCreationId",
                table: "PlayerCreationRatings",
                column: "PlayerCreationId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreationRatings_PlayerId",
                table: "PlayerCreationRatings",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreationReviewRatings_PlayerCreationReviewId",
                table: "PlayerCreationReviewRatings",
                column: "PlayerCreationReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreationReviewRatings_PlayerId",
                table: "PlayerCreationReviewRatings",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreationReviews_PlayerCreationId",
                table: "PlayerCreationReviews",
                column: "PlayerCreationId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreationReviews_PlayerId",
                table: "PlayerCreationReviews",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreations_PlayerId",
                table: "PlayerCreations",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreationViews_PlayerCreationId",
                table: "PlayerCreationViews",
                column: "PlayerCreationId");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_PlayerId",
                table: "Scores",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_SubKeyId",
                table: "Scores",
                column: "SubKeyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GamePlayers");

            migrationBuilder.DropTable(
                name: "GamePlayerStats");

            migrationBuilder.DropTable(
                name: "GriefReports");

            migrationBuilder.DropTable(
                name: "HeartedPlayerCreations");

            migrationBuilder.DropTable(
                name: "HeartedProfiles");

            migrationBuilder.DropTable(
                name: "PlayerCommentRatings");

            migrationBuilder.DropTable(
                name: "PlayerCreationBookmarks");

            migrationBuilder.DropTable(
                name: "PlayerCreationCommentRatings");

            migrationBuilder.DropTable(
                name: "PlayerCreationDownloads");

            migrationBuilder.DropTable(
                name: "PlayerCreationRatings");

            migrationBuilder.DropTable(
                name: "PlayerCreationReviewRatings");

            migrationBuilder.DropTable(
                name: "PlayerCreationViews");

            migrationBuilder.DropTable(
                name: "Scores");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "PlayerComments");

            migrationBuilder.DropTable(
                name: "PlayerCreationComments");

            migrationBuilder.DropTable(
                name: "PlayerCreationReviews");

            migrationBuilder.DropTable(
                name: "PlayerCreations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameServer.Migrations
{
    /// <inheritdoc />
    public partial class MNRSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Points",
                table: "Users",
                newName: "OnlineQuits");

            migrationBuilder.RenameColumn(
                name: "Votes",
                table: "PlayerCreations",
                newName: "ParentPlayerId");

            migrationBuilder.AddColumn<int>(
                name: "CharacterIdx",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "KartIdx",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "LongestDrift",
                table: "Users",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "LongestHangTime",
                table: "Users",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<bool>(
                name: "PlayedMNR",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<float>(
                name: "BestLapTime",
                table: "Scores",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "CharacterIdx",
                table: "Scores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GhostCarDataMD5",
                table: "Scores",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsMNR",
                table: "Scores",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "KartIdx",
                table: "Scores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "Latitude",
                table: "Scores",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Longitude",
                table: "Scores",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AlterColumn<int>(
                name: "ModerationStatus",
                table: "PlayerCreations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<float>(
                name: "BestLapTime",
                table: "PlayerCreations",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<bool>(
                name: "IsMNR",
                table: "PlayerCreations",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OriginalPlayerId",
                table: "PlayerCreations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParentCreationId",
                table: "PlayerCreations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "PlayerCreationRatings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsMNR",
                table: "HeartedProfiles",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberLaps",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Privacy",
                table: "Games",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SpeedClass",
                table: "Games",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Track",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TrackGroup",
                table: "Games",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Bank",
                table: "GamePlayerStats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "BestLapTime",
                table: "GamePlayerStats",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "CharacterIdx",
                table: "GamePlayerStats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "KartIdx",
                table: "GamePlayerStats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "Latitude",
                table: "GamePlayerStats",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "LocationTag",
                table: "GamePlayerStats",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<float>(
                name: "LongestDrift",
                table: "GamePlayerStats",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "LongestHangTime",
                table: "GamePlayerStats",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Longitude",
                table: "GamePlayerStats",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "MusicIdx",
                table: "GamePlayerStats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TrackIdx",
                table: "GamePlayerStats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TrackPlatform",
                table: "GamePlayerStats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Announcements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LanguageCode = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Subject = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Text = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Platform = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MailMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SenderId = table.Column<int>(type: "int", nullable: false),
                    RecipientId = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Body = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RecipientList = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AttachmentReference = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    HasRead = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    HasReplied = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    HasForwarded = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailMessages_Users_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MailMessages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayerComplaints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerComplaints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerComplaints_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerComplaints_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayerCreationComplaints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    PlayerCreationId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCreationComplaints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerCreationComplaints_PlayerCreations_PlayerCreationId",
                        column: x => x.PlayerCreationId,
                        principalTable: "PlayerCreations",
                        principalColumn: "PlayerCreationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerCreationComplaints_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerCreationComplaints_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayerCreationPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayerCreationId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCreationPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerCreationPoints_PlayerCreations_PlayerCreationId",
                        column: x => x.PlayerCreationId,
                        principalTable: "PlayerCreations",
                        principalColumn: "PlayerCreationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerCreationPoints_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayerExperiencePoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerExperiencePoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerExperiencePoints_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayerPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerPoints_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayerRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    RatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerRatings_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerRatings_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MailMessages_RecipientId",
                table: "MailMessages",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_MailMessages_SenderId",
                table: "MailMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerComplaints_PlayerId",
                table: "PlayerComplaints",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerComplaints_UserId",
                table: "PlayerComplaints",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreationComplaints_PlayerCreationId",
                table: "PlayerCreationComplaints",
                column: "PlayerCreationId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreationComplaints_PlayerId",
                table: "PlayerCreationComplaints",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreationComplaints_UserId",
                table: "PlayerCreationComplaints",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreationPoints_PlayerCreationId",
                table: "PlayerCreationPoints",
                column: "PlayerCreationId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreationPoints_PlayerId",
                table: "PlayerCreationPoints",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerExperiencePoints_PlayerId",
                table: "PlayerExperiencePoints",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPoints_PlayerId",
                table: "PlayerPoints",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRatings_AuthorId",
                table: "PlayerRatings",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRatings_PlayerId",
                table: "PlayerRatings",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Announcements");

            migrationBuilder.DropTable(
                name: "MailMessages");

            migrationBuilder.DropTable(
                name: "PlayerComplaints");

            migrationBuilder.DropTable(
                name: "PlayerCreationComplaints");

            migrationBuilder.DropTable(
                name: "PlayerCreationPoints");

            migrationBuilder.DropTable(
                name: "PlayerExperiencePoints");

            migrationBuilder.DropTable(
                name: "PlayerPoints");

            migrationBuilder.DropTable(
                name: "PlayerRatings");

            migrationBuilder.DropColumn(
                name: "CharacterIdx",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "KartIdx",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LongestDrift",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LongestHangTime",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PlayedMNR",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BestLapTime",
                table: "Scores");

            migrationBuilder.DropColumn(
                name: "CharacterIdx",
                table: "Scores");

            migrationBuilder.DropColumn(
                name: "GhostCarDataMD5",
                table: "Scores");

            migrationBuilder.DropColumn(
                name: "IsMNR",
                table: "Scores");

            migrationBuilder.DropColumn(
                name: "KartIdx",
                table: "Scores");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Scores");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Scores");

            migrationBuilder.DropColumn(
                name: "BestLapTime",
                table: "PlayerCreations");

            migrationBuilder.DropColumn(
                name: "IsMNR",
                table: "PlayerCreations");

            migrationBuilder.DropColumn(
                name: "OriginalPlayerId",
                table: "PlayerCreations");

            migrationBuilder.DropColumn(
                name: "ParentCreationId",
                table: "PlayerCreations");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "PlayerCreationRatings");

            migrationBuilder.DropColumn(
                name: "IsMNR",
                table: "HeartedProfiles");

            migrationBuilder.DropColumn(
                name: "NumberLaps",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Privacy",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "SpeedClass",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Track",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "TrackGroup",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Bank",
                table: "GamePlayerStats");

            migrationBuilder.DropColumn(
                name: "BestLapTime",
                table: "GamePlayerStats");

            migrationBuilder.DropColumn(
                name: "CharacterIdx",
                table: "GamePlayerStats");

            migrationBuilder.DropColumn(
                name: "KartIdx",
                table: "GamePlayerStats");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "GamePlayerStats");

            migrationBuilder.DropColumn(
                name: "LocationTag",
                table: "GamePlayerStats");

            migrationBuilder.DropColumn(
                name: "LongestDrift",
                table: "GamePlayerStats");

            migrationBuilder.DropColumn(
                name: "LongestHangTime",
                table: "GamePlayerStats");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "GamePlayerStats");

            migrationBuilder.DropColumn(
                name: "MusicIdx",
                table: "GamePlayerStats");

            migrationBuilder.DropColumn(
                name: "TrackIdx",
                table: "GamePlayerStats");

            migrationBuilder.DropColumn(
                name: "TrackPlatform",
                table: "GamePlayerStats");

            migrationBuilder.RenameColumn(
                name: "OnlineQuits",
                table: "Users",
                newName: "Points");

            migrationBuilder.RenameColumn(
                name: "ParentPlayerId",
                table: "PlayerCreations",
                newName: "Votes");

            migrationBuilder.AlterColumn<string>(
                name: "ModerationStatus",
                table: "PlayerCreations",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}

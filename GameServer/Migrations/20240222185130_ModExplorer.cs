using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameServer.Migrations
{
    /// <inheritdoc />
    public partial class ModExplorer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasCheckedInBefore",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<float>(
                name: "LastLatitude",
                table: "Users",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "LastLongitude",
                table: "Users",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ModMiles",
                table: "Users",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "LocationTag",
                table: "Scores",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AwardUnlocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AwardUnlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AwardUnlocks_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "POIVisits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    PointOfInterestId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POIVisits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_POIVisits_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TravelPoints",
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
                    table.PrimaryKey("PK_TravelPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelPoints_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AwardUnlocks_PlayerId",
                table: "AwardUnlocks",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_POIVisits_PlayerId",
                table: "POIVisits",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelPoints_PlayerId",
                table: "TravelPoints",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AwardUnlocks");

            migrationBuilder.DropTable(
                name: "POIVisits");

            migrationBuilder.DropTable(
                name: "TravelPoints");

            migrationBuilder.DropColumn(
                name: "HasCheckedInBefore",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastLatitude",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastLongitude",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ModMiles",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LocationTag",
                table: "Scores");
        }
    }
}

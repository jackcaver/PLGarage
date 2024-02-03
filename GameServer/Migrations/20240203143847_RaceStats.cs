using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameServer.Migrations
{
    /// <inheritdoc />
    public partial class RaceStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnlineFinished",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OnlineFinishedLastWeek",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OnlineFinishedThisWeek",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OnlineQuits",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OnlineRaces",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OnlineRacesLastWeek",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OnlineRacesThisWeek",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OnlineWins",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OnlineWinsLastWeek",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OnlineWinsThisWeek",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "RacesFinished",
                table: "PlayerCreations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "OnlineRaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlineRaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnlineRaces_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OnlineRacesFinished",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsWinner = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlineRacesFinished", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnlineRacesFinished_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_OnlineRaces_PlayerId",
                table: "OnlineRaces",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_OnlineRacesFinished_PlayerId",
                table: "OnlineRacesFinished",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OnlineRaces");

            migrationBuilder.DropTable(
                name: "OnlineRacesFinished");

            migrationBuilder.DropColumn(
                name: "RacesFinished",
                table: "PlayerCreations");

            migrationBuilder.AddColumn<int>(
                name: "OnlineFinished",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OnlineFinishedLastWeek",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OnlineFinishedThisWeek",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OnlineQuits",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OnlineRaces",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OnlineRacesLastWeek",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OnlineRacesThisWeek",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OnlineWins",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OnlineWinsLastWeek",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OnlineWinsThisWeek",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

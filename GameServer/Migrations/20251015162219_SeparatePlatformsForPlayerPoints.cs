using GameServer.Models.PlayerData;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameServer.Migrations
{
    /// <inheritdoc />
    public partial class SeparatePlatformsForPlayerPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Platform",
                table: "PlayerPoints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Platform",
                table: "PlayerExperiencePoints",
                type: "int",
                nullable: false,
                defaultValue: Platform.PS3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Platform",
                table: "PlayerPoints");

            migrationBuilder.DropColumn(
                name: "Platform",
                table: "PlayerExperiencePoints");
        }
    }
}

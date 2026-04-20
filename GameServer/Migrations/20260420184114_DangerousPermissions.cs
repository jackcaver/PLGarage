using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameServer.Migrations
{
    /// <inheritdoc />
    public partial class DangerousPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ManageWhitelist",
                table: "Moderators",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RemovePlayerCreations",
                table: "Moderators",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RemoveUsers",
                table: "Moderators",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ResetCreationStats",
                table: "Moderators",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ResetUserStats",
                table: "Moderators",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManageWhitelist",
                table: "Moderators");

            migrationBuilder.DropColumn(
                name: "RemovePlayerCreations",
                table: "Moderators");

            migrationBuilder.DropColumn(
                name: "RemoveUsers",
                table: "Moderators");

            migrationBuilder.DropColumn(
                name: "ResetCreationStats",
                table: "Moderators");

            migrationBuilder.DropColumn(
                name: "ResetUserStats",
                table: "Moderators");
        }
    }
}

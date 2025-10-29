using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameServer.Migrations
{
    /// <inheritdoc />
    public partial class ModeraationChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BanUsers",
                table: "Moderators",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ChangeCreationStatus",
                table: "Moderators",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ChangeUserSettings",
                table: "Moderators",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ManageModerators",
                table: "Moderators",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ViewGriefReports",
                table: "Moderators",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ViewPlayerComplaints",
                table: "Moderators",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ViewPlayerCreationComplaints",
                table: "Moderators",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BanUsers",
                table: "Moderators");

            migrationBuilder.DropColumn(
                name: "ChangeCreationStatus",
                table: "Moderators");

            migrationBuilder.DropColumn(
                name: "ChangeUserSettings",
                table: "Moderators");

            migrationBuilder.DropColumn(
                name: "ManageModerators",
                table: "Moderators");

            migrationBuilder.DropColumn(
                name: "ViewGriefReports",
                table: "Moderators");

            migrationBuilder.DropColumn(
                name: "ViewPlayerComplaints",
                table: "Moderators");

            migrationBuilder.DropColumn(
                name: "ViewPlayerCreationComplaints",
                table: "Moderators");
        }
    }
}

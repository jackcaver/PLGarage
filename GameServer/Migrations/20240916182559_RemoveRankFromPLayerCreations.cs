using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameServer.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRankFromPLayerCreations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "PlayerCreations");

            migrationBuilder.RenameColumn(
                name: "HideCreationsWithoutPreviews",
                table: "Users",
                newName: "ShowCreationsWithoutPreviews");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShowCreationsWithoutPreviews",
                table: "Users",
                newName: "HideCreationsWithoutPreviews");

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "PlayerCreations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

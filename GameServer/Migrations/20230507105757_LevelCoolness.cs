using Microsoft.EntityFrameworkCore.Migrations;

namespace GameServer.Migrations
{
    public partial class LevelCoolness : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coolness",
                table: "PlayerCreations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Coolness",
                table: "PlayerCreations",
                type: "float",
                nullable: false,
                defaultValue: 0f);
        }
    }
}

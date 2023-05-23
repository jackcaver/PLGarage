using Microsoft.EntityFrameworkCore.Migrations;

namespace GameServer.Migrations
{
    public partial class NewSessionHandling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Presence",
                table: "Users");

            migrationBuilder.AddColumn<ulong>(
                name: "PSNID",
                table: "Users",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "RPCNID",
                table: "Users",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PSNID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RPCNID",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "Presence",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

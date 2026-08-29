using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameServer.Migrations
{
    /// <inheritdoc />
    public partial class UniqueDownloadsAndViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "Sessions",
                type: "varchar(45)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "PlayerId",
                table: "PlayerCreationViews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlayerId",
                table: "PlayerCreationDownloads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("DELETE FROM PlayerCreationDownloads", true);
            migrationBuilder.Sql("DELETE FROM PlayerCreationViews", true);
            
            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreationViews_PlayerId",
                table: "PlayerCreationViews",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreationUniqueRacers_PlayerId",
                table: "PlayerCreationUniqueRacers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCreationDownloads_PlayerId",
                table: "PlayerCreationDownloads",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerCreationDownloads_Users_PlayerId",
                table: "PlayerCreationDownloads",
                column: "PlayerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerCreationUniqueRacers_Users_PlayerId",
                table: "PlayerCreationUniqueRacers",
                column: "PlayerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerCreationViews_Users_PlayerId",
                table: "PlayerCreationViews",
                column: "PlayerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerCreationDownloads_Users_PlayerId",
                table: "PlayerCreationDownloads");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerCreationUniqueRacers_Users_PlayerId",
                table: "PlayerCreationUniqueRacers");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerCreationViews_Users_PlayerId",
                table: "PlayerCreationViews");

            migrationBuilder.DropIndex(
                name: "IX_PlayerCreationViews_PlayerId",
                table: "PlayerCreationViews");

            migrationBuilder.DropIndex(
                name: "IX_PlayerCreationUniqueRacers_PlayerId",
                table: "PlayerCreationUniqueRacers");

            migrationBuilder.DropIndex(
                name: "IX_PlayerCreationDownloads_PlayerId",
                table: "PlayerCreationDownloads");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "PlayerCreationViews");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "PlayerCreationDownloads");

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "Sessions",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}

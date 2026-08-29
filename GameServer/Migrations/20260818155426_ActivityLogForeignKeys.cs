using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameServer.Migrations
{
    /// <inheritdoc />
    public partial class ActivityLogForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PlayerId",
                table: "ActivityLog",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PlayerCreationId",
                table: "ActivityLog",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "ActivityLog",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.Sql("UPDATE ActivityLog SET PlayerId=NULL WHERE PlayerId=0;");
            migrationBuilder.Sql("UPDATE ActivityLog SET PlayerCreationId=NULL WHERE PlayerCreationId=0;");
            migrationBuilder.Sql("UPDATE ActivityLog SET AuthorId=NULL WHERE AuthorId=0;");
            
            migrationBuilder.CreateIndex(
                name: "IX_ActivityLog_AuthorId",
                table: "ActivityLog",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLog_PlayerCreationId",
                table: "ActivityLog",
                column: "PlayerCreationId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLog_PlayerId",
                table: "ActivityLog",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLog_PlayerCreations_PlayerCreationId",
                table: "ActivityLog",
                column: "PlayerCreationId",
                principalTable: "PlayerCreations",
                principalColumn: "PlayerCreationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLog_Users_AuthorId",
                table: "ActivityLog",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLog_Users_PlayerId",
                table: "ActivityLog",
                column: "PlayerId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLog_PlayerCreations_PlayerCreationId",
                table: "ActivityLog");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLog_Users_AuthorId",
                table: "ActivityLog");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLog_Users_PlayerId",
                table: "ActivityLog");

            migrationBuilder.DropIndex(
                name: "IX_ActivityLog_AuthorId",
                table: "ActivityLog");

            migrationBuilder.DropIndex(
                name: "IX_ActivityLog_PlayerCreationId",
                table: "ActivityLog");

            migrationBuilder.DropIndex(
                name: "IX_ActivityLog_PlayerId",
                table: "ActivityLog");

            migrationBuilder.Sql("UPDATE ActivityLog SET PlayerId=0 WHERE PlayerId IS NULL;");
            migrationBuilder.Sql("UPDATE ActivityLog SET PlayerCreationId=0 WHERE PlayerCreationId IS NULL;");
            migrationBuilder.Sql("UPDATE ActivityLog SET AuthorId=0 WHERE AuthorId IS NULL;");
            
            migrationBuilder.AlterColumn<int>(
                name: "PlayerId",
                table: "ActivityLog",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlayerCreationId",
                table: "ActivityLog",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "ActivityLog",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}

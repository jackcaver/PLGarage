using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameServer.Migrations
{
    /// <inheritdoc />
    public partial class ModeratorAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "PlayerCreationComplaints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "PlayerComplaints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "GriefReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ModeratorAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    GriefReportId = table.Column<int>(type: "int", nullable: true),
                    PlayerComplaintId = table.Column<int>(type: "int", nullable: true),
                    PlayerCreationComplaintId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModeratorAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModeratorAssignments_GriefReports_GriefReportId",
                        column: x => x.GriefReportId,
                        principalTable: "GriefReports",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ModeratorAssignments_Moderators_UserId",
                        column: x => x.UserId,
                        principalTable: "Moderators",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModeratorAssignments_PlayerComplaints_PlayerComplaintId",
                        column: x => x.PlayerComplaintId,
                        principalTable: "PlayerComplaints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ModeratorAssignments_PlayerCreationComplaints_PlayerCreation~",
                        column: x => x.PlayerCreationComplaintId,
                        principalTable: "PlayerCreationComplaints",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ModeratorAssignments_GriefReportId",
                table: "ModeratorAssignments",
                column: "GriefReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ModeratorAssignments_PlayerComplaintId",
                table: "ModeratorAssignments",
                column: "PlayerComplaintId");

            migrationBuilder.CreateIndex(
                name: "IX_ModeratorAssignments_PlayerCreationComplaintId",
                table: "ModeratorAssignments",
                column: "PlayerCreationComplaintId");

            migrationBuilder.CreateIndex(
                name: "IX_ModeratorAssignments_UserId",
                table: "ModeratorAssignments",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModeratorAssignments");

            migrationBuilder.DropColumn(
                name: "State",
                table: "PlayerCreationComplaints");

            migrationBuilder.DropColumn(
                name: "State",
                table: "PlayerComplaints");

            migrationBuilder.DropColumn(
                name: "State",
                table: "GriefReports");
        }
    }
}

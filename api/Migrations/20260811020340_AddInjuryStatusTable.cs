using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballGm.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddInjuryStatusTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InjuryStatus",
                columns: table => new
                {
                    Season = table.Column<short>(type: "INTEGER", nullable: false),
                    Week = table.Column<short>(type: "INTEGER", nullable: false),
                    PlayerId = table.Column<string>(type: "TEXT", nullable: false),
                    OfficialReportStatus = table.Column<string>(type: "TEXT", nullable: false),
                    PracticePrimaryStatus = table.Column<string>(type: "TEXT", nullable: false),
                    PracticeStatus = table.Column<string>(type: "TEXT", nullable: false),
                    LastUpdated = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InjuryStatus", x => new { x.Season, x.Week, x.PlayerId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InjuryStatus");
        }
    }
}

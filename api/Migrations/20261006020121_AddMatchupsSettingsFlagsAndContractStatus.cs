using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballGm.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchupsSettingsFlagsAndContractStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Settings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFixed",
                table: "Settings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Contracts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Matchups",
                columns: table => new
                {
                    MatchupId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LeagueId = table.Column<int>(type: "INTEGER", nullable: false),
                    Week = table.Column<int>(type: "INTEGER", nullable: false),
                    HomeTeamTeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    AwayTeamTeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    HomeScore = table.Column<float>(type: "REAL", nullable: false),
                    AwayScore = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matchups", x => x.MatchupId);
                    table.ForeignKey(
                        name: "FK_Matchups_Teams_AwayTeamTeamId",
                        column: x => x.AwayTeamTeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Matchups_Teams_HomeTeamTeamId",
                        column: x => x.HomeTeamTeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matchups_AwayTeamTeamId",
                table: "Matchups",
                column: "AwayTeamTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Matchups_HomeTeamTeamId",
                table: "Matchups",
                column: "HomeTeamTeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matchups");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "IsFixed",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Contracts");
        }
    }
}

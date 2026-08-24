using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballGm.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLeagueSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Budgets",
                columns: table => new
                {
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    Week = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentObligations = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budgets", x => new { x.TeamId, x.Week });
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    ContractId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartWeek = table.Column<int>(type: "INTEGER", nullable: false),
                    EndWeek = table.Column<int>(type: "INTEGER", nullable: false),
                    SigningBonus = table.Column<decimal>(type: "TEXT", nullable: false),
                    Salary = table.Column<decimal>(type: "TEXT", nullable: false),
                    GiftedCapSpace = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.ContractId);
                });

            migrationBuilder.CreateTable(
                name: "Leagues",
                columns: table => new
                {
                    LeagueId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AdminUserId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    AdminTeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leagues", x => x.LeagueId);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    SettingsId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LeagueId = table.Column<int>(type: "INTEGER", nullable: false),
                    WeeklyCapSpace = table.Column<decimal>(type: "TEXT", nullable: false),
                    EligiblePositions = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.SettingsId);
                    table.ForeignKey(
                        name: "FK_Settings_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "LeagueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LeagueId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Wins = table.Column<short>(type: "INTEGER", nullable: false),
                    Loses = table.Column<short>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.TeamId);
                    table.ForeignKey(
                        name: "FK_Teams_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "LeagueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rules",
                columns: table => new
                {
                    RuleId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SettingsId = table.Column<int>(type: "INTEGER", nullable: false),
                    RuleType = table.Column<short>(type: "INTEGER", nullable: false),
                    Stat = table.Column<short>(type: "INTEGER", nullable: true),
                    Threshold = table.Column<decimal>(type: "TEXT", nullable: true),
                    Points = table.Column<decimal>(type: "TEXT", nullable: true),
                    Weight = table.Column<decimal>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.RuleId);
                    table.ForeignKey(
                        name: "FK_Rules_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "SettingsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamPlayers",
                columns: table => new
                {
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerId = table.Column<string>(type: "TEXT", nullable: false),
                    ContractId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamPlayers", x => new { x.TeamId, x.PlayerId, x.ContractId });
                    table.ForeignKey(
                        name: "FK_TeamPlayers_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "ContractId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamPlayers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamPlayers_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rules_SettingsId",
                table: "Rules",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_LeagueId",
                table: "Settings",
                column: "LeagueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamPlayers_ContractId",
                table: "TeamPlayers",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamPlayers_PlayerId",
                table: "TeamPlayers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_LeagueId",
                table: "Teams",
                column: "LeagueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Budgets");

            migrationBuilder.DropTable(
                name: "Rules");

            migrationBuilder.DropTable(
                name: "TeamPlayers");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Leagues");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballGm.Api.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureCompositeKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Season = table.Column<short>(type: "INTEGER", nullable: false),
                    Week = table.Column<short>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    HomeTeamId = table.Column<short>(type: "INTEGER", nullable: false),
                    AwayTeamId = table.Column<short>(type: "INTEGER", nullable: false),
                    WinningTeamId = table.Column<short>(type: "INTEGER", nullable: false),
                    WinningScore = table.Column<short>(type: "INTEGER", nullable: false),
                    LosingScore = table.Column<short>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerGameStats",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    PassAttempts = table.Column<short>(type: "INTEGER", nullable: false),
                    PassCompletions = table.Column<short>(type: "INTEGER", nullable: false),
                    PassingYards = table.Column<short>(type: "INTEGER", nullable: false),
                    PassingTouchdowns = table.Column<short>(type: "INTEGER", nullable: false),
                    RushAttempts = table.Column<short>(type: "INTEGER", nullable: false),
                    RushingYards = table.Column<short>(type: "INTEGER", nullable: false),
                    RushingFirstDowns = table.Column<short>(type: "INTEGER", nullable: false),
                    RushingTouchdowns = table.Column<short>(type: "INTEGER", nullable: false),
                    Receptions = table.Column<short>(type: "INTEGER", nullable: false),
                    ReceivingYards = table.Column<short>(type: "INTEGER", nullable: false),
                    ReceivingTouchdowns = table.Column<short>(type: "INTEGER", nullable: false),
                    Interceptions = table.Column<short>(type: "INTEGER", nullable: false),
                    Fumbles = table.Column<short>(type: "INTEGER", nullable: false),
                    Sacks = table.Column<short>(type: "INTEGER", nullable: false),
                    FieldGoalsMade = table.Column<string>(type: "TEXT", nullable: false),
                    FieldGoalsAttempted = table.Column<string>(type: "TEXT", nullable: false),
                    ExtraPointsMade = table.Column<short>(type: "INTEGER", nullable: false),
                    ExtraPointsAttempted = table.Column<short>(type: "INTEGER", nullable: false),
                    ReturnedTouchdowns = table.Column<short>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerGameStats", x => new { x.PlayerId, x.GameId });
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TeamId = table.Column<short>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Position = table.Column<short>(type: "INTEGER", nullable: false),
                    JerseyNumber = table.Column<short>(type: "INTEGER", nullable: false),
                    Age = table.Column<short>(type: "INTEGER", nullable: false),
                    DraftYear = table.Column<short>(type: "INTEGER", nullable: false),
                    IsInjured = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerSeasonStats",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Season = table.Column<short>(type: "INTEGER", nullable: false),
                    PassAttempts = table.Column<short>(type: "INTEGER", nullable: false),
                    PassCompletions = table.Column<short>(type: "INTEGER", nullable: false),
                    PassingYards = table.Column<short>(type: "INTEGER", nullable: false),
                    PassingTouchdowns = table.Column<short>(type: "INTEGER", nullable: false),
                    RushAttempts = table.Column<short>(type: "INTEGER", nullable: false),
                    RushingYards = table.Column<short>(type: "INTEGER", nullable: false),
                    RushingFirstDowns = table.Column<short>(type: "INTEGER", nullable: false),
                    RushingTouchdowns = table.Column<short>(type: "INTEGER", nullable: false),
                    Receptions = table.Column<short>(type: "INTEGER", nullable: false),
                    ReceivingYards = table.Column<short>(type: "INTEGER", nullable: false),
                    ReceivingTouchdowns = table.Column<short>(type: "INTEGER", nullable: false),
                    Interceptions = table.Column<short>(type: "INTEGER", nullable: false),
                    Fumbles = table.Column<short>(type: "INTEGER", nullable: false),
                    Sacks = table.Column<short>(type: "INTEGER", nullable: false),
                    FieldGoalsMade = table.Column<short>(type: "INTEGER", nullable: false),
                    FieldGoalsAttempted = table.Column<short>(type: "INTEGER", nullable: false),
                    ExtraPointsMade = table.Column<short>(type: "INTEGER", nullable: false),
                    ExtraPointsAttempted = table.Column<short>(type: "INTEGER", nullable: false),
                    ReturnedTouchdowns = table.Column<short>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerSeasonStats", x => new { x.PlayerId, x.Season });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "PlayerGameStats");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "PlayerSeasonStats");
        }
    }
}

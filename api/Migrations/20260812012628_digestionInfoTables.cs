using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballGm.Api.Migrations
{
    /// <inheritdoc />
    public partial class digestionInfoTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameId = table.Column<string>(type: "TEXT", nullable: false),
                    Season = table.Column<short>(type: "INTEGER", nullable: false),
                    Week = table.Column<short>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    HomeTeam = table.Column<string>(type: "TEXT", nullable: false),
                    AwayTeam = table.Column<string>(type: "TEXT", nullable: false),
                    HomeScore = table.Column<string>(type: "TEXT", nullable: false),
                    AwayScore = table.Column<string>(type: "TEXT", nullable: false),
                    WindSpeed = table.Column<short>(type: "INTEGER", nullable: true),
                    Temperature = table.Column<short>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameId);
                });

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

            migrationBuilder.CreateTable(
                name: "PlayerGame",
                columns: table => new
                {
                    PlayerId = table.Column<string>(type: "TEXT", nullable: false),
                    GameId = table.Column<string>(type: "TEXT", nullable: false),
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
                    FieldGoalsMissed = table.Column<string>(type: "TEXT", nullable: false),
                    ExtraPointsMade = table.Column<short>(type: "INTEGER", nullable: false),
                    ExtraPointsAttempted = table.Column<short>(type: "INTEGER", nullable: false),
                    PassingTwoPointConversions = table.Column<short>(type: "INTEGER", nullable: false),
                    RushingTwoPointConversions = table.Column<short>(type: "INTEGER", nullable: false),
                    ReceivingTwoPointConversions = table.Column<short>(type: "INTEGER", nullable: false),
                    ReturnedTouchdowns = table.Column<short>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerGame", x => new { x.PlayerId, x.GameId });
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    PlayerId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PictureUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Team = table.Column<string>(type: "TEXT", nullable: false),
                    Position = table.Column<string>(type: "TEXT", nullable: false),
                    JerseyNumber = table.Column<short>(type: "INTEGER", nullable: false),
                    DraftYear = table.Column<short>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerSeason",
                columns: table => new
                {
                    PlayerId = table.Column<string>(type: "TEXT", nullable: false),
                    Season = table.Column<short>(type: "INTEGER", nullable: false),
                    PassAttempts = table.Column<short>(type: "INTEGER", nullable: false),
                    PassCompletions = table.Column<short>(type: "INTEGER", nullable: false),
                    PassingYards = table.Column<short>(type: "INTEGER", nullable: false),
                    PassingTouchdowns = table.Column<short>(type: "INTEGER", nullable: false),
                    RushAttempts = table.Column<short>(type: "INTEGER", nullable: false),
                    RushingYards = table.Column<short>(type: "INTEGER", nullable: false),
                    RushingBrokenTackles = table.Column<short>(type: "INTEGER", nullable: false),
                    RushingFirstDowns = table.Column<short>(type: "INTEGER", nullable: false),
                    RushingTouchdowns = table.Column<short>(type: "INTEGER", nullable: false),
                    Receptions = table.Column<short>(type: "INTEGER", nullable: false),
                    ReceivingYards = table.Column<short>(type: "INTEGER", nullable: false),
                    ReceivingTouchdowns = table.Column<short>(type: "INTEGER", nullable: false),
                    Interceptions = table.Column<short>(type: "INTEGER", nullable: false),
                    Fumbles = table.Column<short>(type: "INTEGER", nullable: false),
                    Sacks = table.Column<short>(type: "INTEGER", nullable: false),
                    FieldGoalsMade = table.Column<string>(type: "TEXT", nullable: false),
                    FieldGoalsMissed = table.Column<string>(type: "TEXT", nullable: false),
                    ExtraPointsMade = table.Column<short>(type: "INTEGER", nullable: false),
                    ExtraPointsAttempted = table.Column<short>(type: "INTEGER", nullable: false),
                    PassingTwoPointConversions = table.Column<short>(type: "INTEGER", nullable: false),
                    TwoPointConversionsMade = table.Column<short>(type: "INTEGER", nullable: false),
                    ReturnedTouchdowns = table.Column<short>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerSeason", x => new { x.PlayerId, x.Season });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "InjuryStatus");

            migrationBuilder.DropTable(
                name: "PlayerGame");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "PlayerSeason");
        }
    }
}

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
            // Local DBs may already have these tables from older migration IDs
            // that are no longer in the repo. CREATE IF NOT EXISTS lets Migrate()
            // catch up without wiping ingested data.
            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS "Games" (
                    "GameId" TEXT NOT NULL CONSTRAINT "PK_Games" PRIMARY KEY,
                    "Season" INTEGER NOT NULL,
                    "Week" INTEGER NOT NULL,
                    "Date" TEXT NOT NULL,
                    "HomeTeam" TEXT NOT NULL,
                    "AwayTeam" TEXT NOT NULL,
                    "HomeScore" TEXT NOT NULL,
                    "AwayScore" TEXT NOT NULL,
                    "WindSpeed" INTEGER NULL,
                    "Temperature" INTEGER NULL
                );
                """);

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS "InjuryStatus" (
                    "Season" INTEGER NOT NULL,
                    "Week" INTEGER NOT NULL,
                    "PlayerId" TEXT NOT NULL,
                    "OfficialReportStatus" TEXT NOT NULL,
                    "PracticePrimaryStatus" TEXT NOT NULL,
                    "PracticeStatus" TEXT NOT NULL,
                    "LastUpdated" TEXT NOT NULL,
                    CONSTRAINT "PK_InjuryStatus" PRIMARY KEY ("Season", "Week", "PlayerId")
                );
                """);

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS "PlayerGame" (
                    "PlayerId" TEXT NOT NULL,
                    "GameId" TEXT NOT NULL,
                    "PassAttempts" INTEGER NOT NULL,
                    "PassCompletions" INTEGER NOT NULL,
                    "PassingYards" INTEGER NOT NULL,
                    "PassingTouchdowns" INTEGER NOT NULL,
                    "RushAttempts" INTEGER NOT NULL,
                    "RushingYards" INTEGER NOT NULL,
                    "RushingFirstDowns" INTEGER NOT NULL,
                    "RushingTouchdowns" INTEGER NOT NULL,
                    "Receptions" INTEGER NOT NULL,
                    "ReceivingYards" INTEGER NOT NULL,
                    "ReceivingTouchdowns" INTEGER NOT NULL,
                    "Interceptions" INTEGER NOT NULL,
                    "Fumbles" INTEGER NOT NULL,
                    "Sacks" INTEGER NOT NULL,
                    "FieldGoalsMade" TEXT NOT NULL,
                    "FieldGoalsMissed" TEXT NOT NULL,
                    "ExtraPointsMade" INTEGER NOT NULL,
                    "ExtraPointsAttempted" INTEGER NOT NULL,
                    "PassingTwoPointConversions" INTEGER NOT NULL,
                    "RushingTwoPointConversions" INTEGER NOT NULL,
                    "ReceivingTwoPointConversions" INTEGER NOT NULL,
                    "ReturnedTouchdowns" INTEGER NOT NULL,
                    CONSTRAINT "PK_PlayerGame" PRIMARY KEY ("PlayerId", "GameId")
                );
                """);

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS "Players" (
                    "PlayerId" TEXT NOT NULL CONSTRAINT "PK_Players" PRIMARY KEY,
                    "Name" TEXT NOT NULL,
                    "PictureUrl" TEXT NOT NULL,
                    "Team" TEXT NOT NULL,
                    "Position" TEXT NOT NULL,
                    "JerseyNumber" INTEGER NOT NULL,
                    "DraftYear" INTEGER NOT NULL
                );
                """);

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS "PlayerSeason" (
                    "PlayerId" TEXT NOT NULL,
                    "Season" INTEGER NOT NULL,
                    "PassAttempts" INTEGER NOT NULL,
                    "PassCompletions" INTEGER NOT NULL,
                    "PassingYards" INTEGER NOT NULL,
                    "PassingTouchdowns" INTEGER NOT NULL,
                    "RushAttempts" INTEGER NOT NULL,
                    "RushingYards" INTEGER NOT NULL,
                    "RushingBrokenTackles" INTEGER NOT NULL,
                    "RushingFirstDowns" INTEGER NOT NULL,
                    "RushingTouchdowns" INTEGER NOT NULL,
                    "Receptions" INTEGER NOT NULL,
                    "ReceivingYards" INTEGER NOT NULL,
                    "ReceivingTouchdowns" INTEGER NOT NULL,
                    "Interceptions" INTEGER NOT NULL,
                    "Fumbles" INTEGER NOT NULL,
                    "Sacks" INTEGER NOT NULL,
                    "FieldGoalsMade" TEXT NOT NULL,
                    "FieldGoalsMissed" TEXT NOT NULL,
                    "ExtraPointsMade" INTEGER NOT NULL,
                    "ExtraPointsAttempted" INTEGER NOT NULL,
                    "PassingTwoPointConversions" INTEGER NOT NULL,
                    "TwoPointConversionsMade" INTEGER NOT NULL,
                    "ReturnedTouchdowns" INTEGER NOT NULL,
                    CONSTRAINT "PK_PlayerSeason" PRIMARY KEY ("PlayerId", "Season")
                );
                """);
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

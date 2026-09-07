using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballGm.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLeagueMembersAndBudgetSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Budgets",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "Loses",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Wins",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Teams");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Teams",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.RenameColumn(
                name: "TwoPointConversionsMade",
                table: "PlayerSeason",
                newName: "RushingTwoPointConversions");

            migrationBuilder.RenameColumn(
                name: "CurrentObligations",
                table: "Budgets",
                newName: "PaymentSchedule");

            migrationBuilder.RenameColumn(
                name: "Week",
                table: "Budgets",
                newName: "LeagueId");

            migrationBuilder.AddColumn<int>(
                name: "LeagueId",
                table: "TeamPlayers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<short>(
                name: "Stat",
                table: "Rules",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<short>(
                name: "ReceivingTwoPointConversions",
                table: "PlayerSeason",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "JoinCode",
                table: "Leagues",
                type: "TEXT",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "LeagueMembers",
                columns: table => new
                {
                    LeagueId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Role = table.Column<short>(type: "INTEGER", nullable: false),
                    JoinedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueMembers", x => new { x.LeagueId, x.UserId });
                    table.ForeignKey(
                        name: "FK_LeagueMembers_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "LeagueId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeagueMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(
                """
                INSERT OR IGNORE INTO "LeagueMembers" ("LeagueId", "UserId", "Role", "JoinedAtUtc")
                SELECT "LeagueId", "AdminUserId", 2, datetime('now')
                FROM "Leagues"
                WHERE "AdminUserId" IS NOT NULL AND "AdminUserId" != '';

                UPDATE "Teams"
                SET "UserId" = (
                    SELECT "AdminUserId" FROM "Leagues" WHERE "Leagues"."LeagueId" = "Teams"."LeagueId"
                )
                WHERE ("UserId" = '' OR "UserId" IS NULL)
                  AND EXISTS (SELECT 1 FROM "Leagues" WHERE "Leagues"."LeagueId" = "Teams"."LeagueId");

                UPDATE "Leagues"
                SET "JoinCode" = substr('L' || printf('%07d', "LeagueId"), 1, 8)
                WHERE "JoinCode" IS NULL OR "JoinCode" = '';

                UPDATE "TeamPlayers"
                SET "LeagueId" = (
                    SELECT "LeagueId" FROM "Teams" WHERE "Teams"."TeamId" = "TeamPlayers"."TeamId"
                )
                WHERE "LeagueId" = 0;

                UPDATE "Budgets"
                SET "LeagueId" = (
                    SELECT "LeagueId" FROM "Teams" WHERE "Teams"."TeamId" = "Budgets"."TeamId"
                )
                WHERE "LeagueId" = 0;
                """);

            migrationBuilder.DropColumn(
                name: "AdminTeamId",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "AdminUserId",
                table: "Leagues");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Budgets",
                table: "Budgets",
                columns: new[] { "LeagueId", "TeamId" });

            migrationBuilder.CreateIndex(
                name: "IX_Teams_UserId",
                table: "Teams",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamPlayers_LeagueId",
                table: "TeamPlayers",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_Leagues_JoinCode",
                table: "Leagues",
                column: "JoinCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeagueMembers_UserId",
                table: "LeagueMembers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamPlayers_Leagues_LeagueId",
                table: "TeamPlayers",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "LeagueId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Users_UserId",
                table: "Teams",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamPlayers_Leagues_LeagueId",
                table: "TeamPlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Users_UserId",
                table: "Teams");

            migrationBuilder.DropTable(
                name: "LeagueMembers");

            migrationBuilder.DropIndex(
                name: "IX_Teams_UserId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_TeamPlayers_LeagueId",
                table: "TeamPlayers");

            migrationBuilder.DropIndex(
                name: "IX_Leagues_JoinCode",
                table: "Leagues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Budgets",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "LeagueId",
                table: "TeamPlayers");

            migrationBuilder.DropColumn(
                name: "ReceivingTwoPointConversions",
                table: "PlayerSeason");

            migrationBuilder.DropColumn(
                name: "JoinCode",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Teams");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Teams",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.RenameColumn(
                name: "RushingTwoPointConversions",
                table: "PlayerSeason",
                newName: "TwoPointConversionsMade");

            migrationBuilder.RenameColumn(
                name: "PaymentSchedule",
                table: "Budgets",
                newName: "CurrentObligations");

            migrationBuilder.RenameColumn(
                name: "LeagueId",
                table: "Budgets",
                newName: "Week");

            migrationBuilder.AddColumn<short>(
                name: "Loses",
                table: "Teams",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "Wins",
                table: "Teams",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AlterColumn<short>(
                name: "Stat",
                table: "Rules",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "AdminTeamId",
                table: "Leagues",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AdminUserId",
                table: "Leagues",
                type: "TEXT",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Budgets",
                table: "Budgets",
                columns: new[] { "TeamId", "Week" });
        }
    }
}

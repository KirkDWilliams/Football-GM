using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballGm.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePlayerSeasonEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "IsInjured",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "LosingScore",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "WinningScore",
                table: "Games");

            migrationBuilder.RenameColumn(
                name: "FieldGoalsAttempted",
                table: "PlayerSeasonStats",
                newName: "TwoPointConversionsMade");

            migrationBuilder.RenameColumn(
                name: "FieldGoalsAttempted",
                table: "PlayerGameStats",
                newName: "FieldGoalsMissed");

            migrationBuilder.RenameColumn(
                name: "WinningTeam",
                table: "Games",
                newName: "HomeScore");

            migrationBuilder.AlterColumn<string>(
                name: "FieldGoalsMade",
                table: "PlayerSeasonStats",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "FieldGoalsMissed",
                table: "PlayerSeasonStats",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<short>(
                name: "PassingTwoPointConversions",
                table: "PlayerSeasonStats",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "RushingBrokenTackles",
                table: "PlayerSeasonStats",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "Players",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<short>(
                name: "PassingTwoPointConversions",
                table: "PlayerGameStats",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "ReceivingTwoPointConversions",
                table: "PlayerGameStats",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "RushingTwoPointConversions",
                table: "PlayerGameStats",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "AwayScore",
                table: "Games",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<short>(
                name: "Temperature",
                table: "Games",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "WindSpeed",
                table: "Games",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FieldGoalsMissed",
                table: "PlayerSeasonStats");

            migrationBuilder.DropColumn(
                name: "PassingTwoPointConversions",
                table: "PlayerSeasonStats");

            migrationBuilder.DropColumn(
                name: "RushingBrokenTackles",
                table: "PlayerSeasonStats");

            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PassingTwoPointConversions",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "ReceivingTwoPointConversions",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "RushingTwoPointConversions",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "AwayScore",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "WindSpeed",
                table: "Games");

            migrationBuilder.RenameColumn(
                name: "TwoPointConversionsMade",
                table: "PlayerSeasonStats",
                newName: "FieldGoalsAttempted");

            migrationBuilder.RenameColumn(
                name: "FieldGoalsMissed",
                table: "PlayerGameStats",
                newName: "FieldGoalsAttempted");

            migrationBuilder.RenameColumn(
                name: "HomeScore",
                table: "Games",
                newName: "WinningTeam");

            migrationBuilder.AlterColumn<short>(
                name: "FieldGoalsMade",
                table: "PlayerSeasonStats",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<short>(
                name: "Age",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<bool>(
                name: "IsInjured",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<short>(
                name: "LosingScore",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "WinningScore",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);
        }
    }
}

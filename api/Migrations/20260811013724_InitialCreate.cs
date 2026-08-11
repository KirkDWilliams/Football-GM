using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballGm.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerSeasonStats",
                table: "PlayerSeasonStats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerGameStats",
                table: "PlayerGameStats");

            migrationBuilder.RenameTable(
                name: "PlayerSeasonStats",
                newName: "PlayerSeason");

            migrationBuilder.RenameTable(
                name: "PlayerGameStats",
                newName: "PlayerGame");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerSeason",
                table: "PlayerSeason",
                columns: new[] { "PlayerId", "Season" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerGame",
                table: "PlayerGame",
                columns: new[] { "PlayerId", "GameId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerSeason",
                table: "PlayerSeason");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerGame",
                table: "PlayerGame");

            migrationBuilder.RenameTable(
                name: "PlayerSeason",
                newName: "PlayerSeasonStats");

            migrationBuilder.RenameTable(
                name: "PlayerGame",
                newName: "PlayerGameStats");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerSeasonStats",
                table: "PlayerSeasonStats",
                columns: new[] { "PlayerId", "Season" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerGameStats",
                table: "PlayerGameStats",
                columns: new[] { "PlayerId", "GameId" });
        }
    }
}

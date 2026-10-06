using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballGm.Api.Migrations
{
    /// <inheritdoc />
    public partial class OneInPlayDraftPerLeague : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Drafts_LeagueId",
                table: "Drafts");

            migrationBuilder.CreateIndex(
                name: "IX_Drafts_LeagueId_InPlay",
                table: "Drafts",
                column: "LeagueId",
                unique: true,
                filter: "\"Status\" != 3");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Drafts_LeagueId_InPlay",
                table: "Drafts");

            migrationBuilder.CreateIndex(
                name: "IX_Drafts_LeagueId",
                table: "Drafts",
                column: "LeagueId");
        }
    }
}

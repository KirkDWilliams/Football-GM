using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballGm.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDrafts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Drafts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LeagueId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<byte>(type: "INTEGER", nullable: false),
                    CurrentNominatorUserId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    NominationOrder = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drafts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Drafts_LeagueId",
                table: "Drafts",
                column: "LeagueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Drafts");
        }
    }
}

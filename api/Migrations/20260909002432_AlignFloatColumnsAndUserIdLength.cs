using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballGm.Api.Migrations
{
    /// <inheritdoc />
    public partial class AlignFloatColumnsAndUserIdLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "WeeklyCapSpace",
                table: "Settings",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<float>(
                name: "Weight",
                table: "Rules",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Threshold",
                table: "Rules",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Points",
                table: "Rules",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "SigningBonus",
                table: "Contracts",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<float>(
                name: "Salary",
                table: "Contracts",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<float>(
                name: "GiftedCapSpace",
                table: "Contracts",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "WeeklyCapSpace",
                table: "Settings",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.AlterColumn<float>(
                name: "Weight",
                table: "Rules",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "REAL",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Threshold",
                table: "Rules",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "REAL",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Points",
                table: "Rules",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "REAL",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "SigningBonus",
                table: "Contracts",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.AlterColumn<float>(
                name: "Salary",
                table: "Contracts",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.AlterColumn<float>(
                name: "GiftedCapSpace",
                table: "Contracts",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");
        }
    }
}

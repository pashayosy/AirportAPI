using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangeLegPar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Legs",
                keyColumn: "Id",
                keyValue: 6,
                column: "CrossingTime",
                value: 60);

            migrationBuilder.UpdateData(
                table: "Legs",
                keyColumn: "Id",
                keyValue: 7,
                column: "CrossingTime",
                value: 60);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Legs",
                keyColumn: "Id",
                keyValue: 6,
                column: "CrossingTime",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Legs",
                keyColumn: "Id",
                keyValue: 7,
                column: "CrossingTime",
                value: 0);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AirportApi.Migrations
{
    /// <inheritdoc />
    public partial class SomeSmallChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Legs",
                columns: new[] { "Id", "Capacity", "CrossingTime", "Name", "Type" },
                values: new object[,]
                {
                    { -1, 2147483647, 0, "Wait for Landing", 0 },
                    { 10, 2147483647, 0, "Departure to other Place", 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Legs",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.DeleteData(
                table: "Legs",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}

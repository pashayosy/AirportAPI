using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportApi.Migrations
{
    /// <inheritdoc />
    public partial class AddFlightLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessLeg_Legs_LegId",
                table: "ProcessLeg");

            migrationBuilder.DropForeignKey(
                name: "FK_ProcessLeg_Processes_ProcessId",
                table: "ProcessLeg");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProcessLeg",
                table: "ProcessLeg");

            migrationBuilder.RenameTable(
                name: "ProcessLeg",
                newName: "ProcessLegs");

            migrationBuilder.RenameIndex(
                name: "IX_ProcessLeg_LegId",
                table: "ProcessLegs",
                newName: "IX_ProcessLegs_LegId");

            migrationBuilder.AlterColumn<int>(
                name: "CrossingTime",
                table: "Legs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProcessLegs",
                table: "ProcessLegs",
                columns: new[] { "ProcessId", "LegId" });

            migrationBuilder.CreateTable(
                name: "FlightLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FlightId = table.Column<int>(type: "int", nullable: false),
                    LegId = table.Column<int>(type: "int", nullable: false),
                    In = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Out = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlightLogs_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlightLogs_Legs_LegId",
                        column: x => x.LegId,
                        principalTable: "Legs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_FlightLogs_FlightId",
                table: "FlightLogs",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightLogs_LegId",
                table: "FlightLogs",
                column: "LegId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessLegs_Legs_LegId",
                table: "ProcessLegs",
                column: "LegId",
                principalTable: "Legs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessLegs_Processes_ProcessId",
                table: "ProcessLegs",
                column: "ProcessId",
                principalTable: "Processes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessLegs_Legs_LegId",
                table: "ProcessLegs");

            migrationBuilder.DropForeignKey(
                name: "FK_ProcessLegs_Processes_ProcessId",
                table: "ProcessLegs");

            migrationBuilder.DropTable(
                name: "FlightLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProcessLegs",
                table: "ProcessLegs");

            migrationBuilder.RenameTable(
                name: "ProcessLegs",
                newName: "ProcessLeg");

            migrationBuilder.RenameIndex(
                name: "IX_ProcessLegs_LegId",
                table: "ProcessLeg",
                newName: "IX_ProcessLeg_LegId");

            migrationBuilder.AlterColumn<int>(
                name: "CrossingTime",
                table: "Legs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProcessLeg",
                table: "ProcessLeg",
                columns: new[] { "ProcessId", "LegId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessLeg_Legs_LegId",
                table: "ProcessLeg",
                column: "LegId",
                principalTable: "Legs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessLeg_Processes_ProcessId",
                table: "ProcessLeg",
                column: "ProcessId",
                principalTable: "Processes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

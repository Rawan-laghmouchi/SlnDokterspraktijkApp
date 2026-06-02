using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dokterspraktijk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Afspraken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DokterId = table.Column<int>(type: "int", nullable: false),
                    TijdslotId = table.Column<int>(type: "int", nullable: false),
                    Reden = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FotoBestandsnaam = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Afspraken", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dokters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Specialisatie = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dokters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Doktersattesten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AfspraakId = table.Column<int>(type: "int", nullable: false),
                    IsVrijgegeven = table.Column<bool>(type: "bit", nullable: false),
                    IsGedownload = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doktersattesten", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patienten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VoorkeursdokterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patienten", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tijdsloten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DokterId = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateOnly>(type: "date", nullable: false),
                    Tijd = table.Column<TimeOnly>(type: "time", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tijdsloten", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Dokters",
                columns: new[] { "Id", "Naam", "Specialisatie" },
                values: new object[,]
                {
                    { 1, "Timmermans", "Huisarts" },
                    { 2, "Brancaert", "Huisarts" }
                });

            migrationBuilder.InsertData(
                table: "Patienten",
                columns: new[] { "Id", "Naam", "VoorkeursdokterId" },
                values: new object[,]
                {
                    { 1, "Rawan", null },
                    { 2, "Hans", null }
                });

            migrationBuilder.InsertData(
                table: "Tijdsloten",
                columns: new[] { "Id", "Datum", "DokterId", "Status", "Tijd" },
                values: new object[,]
                {
                    { 1, new DateOnly(2026, 5, 15), 1, 1, new TimeOnly(10, 30, 0) },
                    { 2, new DateOnly(2026, 5, 15), 1, 2, new TimeOnly(9, 30, 0) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Afspraken");

            migrationBuilder.DropTable(
                name: "Dokters");

            migrationBuilder.DropTable(
                name: "Doktersattesten");

            migrationBuilder.DropTable(
                name: "Patienten");

            migrationBuilder.DropTable(
                name: "Tijdsloten");
        }
    }
}

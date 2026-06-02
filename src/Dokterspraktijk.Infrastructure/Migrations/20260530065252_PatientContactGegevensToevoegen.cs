using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dokterspraktijk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PatientContactGegevensToevoegen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Patienten",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Rijksregisternummer",
                table: "Patienten",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefoonnummer",
                table: "Patienten",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Patienten",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "Rijksregisternummer", "Telefoonnummer" },
                values: new object[] { "rawan.laghmouchi@gmail.be", "00.00.00-000.00", "0470123456" });

            migrationBuilder.UpdateData(
                table: "Patienten",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "Rijksregisternummer", "Telefoonnummer" },
                values: new object[] { "Hans.Vandenbogaerde@gmail.be", "11.11.11-111.11", "0470654321" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Patienten");

            migrationBuilder.DropColumn(
                name: "Rijksregisternummer",
                table: "Patienten");

            migrationBuilder.DropColumn(
                name: "Telefoonnummer",
                table: "Patienten");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dokterspraktijk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PatientSplitsen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Naam",
                table: "Patienten",
                newName: "Voornaam");

            migrationBuilder.AddColumn<string>(
                name: "Achternaam",
                table: "Patienten",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Patienten",
                keyColumn: "Id",
                keyValue: 1,
                column: "Achternaam",
                value: "Laghmouchi");

            migrationBuilder.UpdateData(
                table: "Patienten",
                keyColumn: "Id",
                keyValue: 2,
                column: "Achternaam",
                value: "Vandenbogaerde");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Achternaam",
                table: "Patienten");

            migrationBuilder.RenameColumn(
                name: "Voornaam",
                table: "Patienten",
                newName: "Naam");
        }
    }
}

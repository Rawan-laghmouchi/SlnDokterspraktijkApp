using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dokterspraktijk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AfspraakCategorieToevoegen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AfspraakCategorieen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AfspraakCategorieen", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AfspraakCategorieen",
                columns: new[] { "Id", "Naam" },
                values: new object[] { 1, "Consultatie" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AfspraakCategorieen");
        }
    }
}

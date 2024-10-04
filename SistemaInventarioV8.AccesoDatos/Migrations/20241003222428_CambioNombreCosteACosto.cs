using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaInventarioV8.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class CambioNombreCosteACosto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Coste",
                table: "Productos",
                newName: "Costo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Costo",
                table: "Productos",
                newName: "Coste");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiendaKeytlin.Server.Migrations
{
    /// <inheritdoc />
    public partial class productoscamposactualizados233 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Productos");

            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "Productos",
                newName: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_EstadoId",
                table: "Productos",
                column: "EstadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Estados_EstadoId",
                table: "Productos",
                column: "EstadoId",
                principalTable: "Estados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Estados_EstadoId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_EstadoId",
                table: "Productos");

            migrationBuilder.RenameColumn(
                name: "EstadoId",
                table: "Productos",
                newName: "Stock");

            migrationBuilder.AddColumn<bool>(
                name: "Estado",
                table: "Productos",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}

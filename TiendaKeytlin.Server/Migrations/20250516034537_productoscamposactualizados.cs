using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiendaKeytlin.Server.Migrations
{
    /// <inheritdoc />
    public partial class productoscamposactualizados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Marca",
                table: "Productos",
                newName: "MarcaProducto");

            migrationBuilder.RenameColumn(
                name: "Codigo",
                table: "Productos",
                newName: "CodigoProducto");

            migrationBuilder.AlterColumn<string>(
                name: "Imagen",
                table: "Productos",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Estado",
                table: "Productos",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Productos");

            migrationBuilder.RenameColumn(
                name: "MarcaProducto",
                table: "Productos",
                newName: "Marca");

            migrationBuilder.RenameColumn(
                name: "CodigoProducto",
                table: "Productos",
                newName: "Codigo");

            migrationBuilder.AlterColumn<string>(
                name: "Imagen",
                table: "Productos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}

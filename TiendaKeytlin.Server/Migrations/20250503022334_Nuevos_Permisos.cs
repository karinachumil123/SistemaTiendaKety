using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TiendaKeytlin.Server.Migrations
{
    /// <inheritdoc />
    public partial class Nuevos_Permisos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaNacimiento",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "edad",
                table: "Usuarios");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 5,
                column: "Nombre",
                value: "Ver Apertura Caja");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 6,
                column: "Nombre",
                value: "Crear Apertura Caja");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 7,
                column: "Nombre",
                value: "Editar Apertura Caja");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 8,
                column: "Nombre",
                value: "Eliminar Apertura Caja");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 9,
                column: "Nombre",
                value: "Ver Cierre Caja");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 10,
                column: "Nombre",
                value: "Crear Cierre Caja");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 11,
                column: "Nombre",
                value: "Editar Cierre Caja");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 12,
                column: "Nombre",
                value: "Eliminar Cierre Caja");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 13,
                column: "Nombre",
                value: "Ver Productos");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 14,
                column: "Nombre",
                value: "Crear Productos");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 15,
                column: "Nombre",
                value: "Editar Productos");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 16,
                column: "Nombre",
                value: "Eliminar Productos");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 17,
                column: "Nombre",
                value: "Ver Categorías");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 18,
                column: "Nombre",
                value: "Crear Categorías");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 19,
                column: "Nombre",
                value: "Editar Categorías");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 20,
                column: "Nombre",
                value: "Eliminar Categorías");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 21,
                column: "Nombre",
                value: "Ver Proveedores");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 22,
                column: "Nombre",
                value: "Crear Proveedores");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 23,
                column: "Nombre",
                value: "Editar Proveedores");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 24,
                column: "Nombre",
                value: "Eliminar Proveedores");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 25,
                column: "Nombre",
                value: "Ver Inventario");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 26,
                column: "Nombre",
                value: "Crear Inventario");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 27,
                column: "Nombre",
                value: "Editar Inventario");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 28,
                column: "Nombre",
                value: "Eliminar Inventario");

            migrationBuilder.InsertData(
                table: "Permisos",
                columns: new[] { "Id", "Nombre", "RolUsuarioId" },
                values: new object[,]
                {
                    { 29, "Ver Pedidos", null },
                    { 30, "Crear Pedidos", null },
                    { 31, "Editar Pedidos", null },
                    { 32, "Eliminar Pedidos", null },
                    { 33, "Ver Ventas", null },
                    { 34, "Crear Ventas", null },
                    { 35, "Editar Ventas", null },
                    { 36, "Eliminar Ventas", null },
                    { 37, "Ver Historial", null },
                    { 38, "Crear Historial", null },
                    { 39, "Editar Historial", null },
                    { 40, "Eliminar Historial", null },
                    { 41, "Ver Usuarios", null },
                    { 42, "Crear Usuarios", null },
                    { 43, "Editar Usuarios", null },
                    { 44, "Eliminar Usuarios", null },
                    { 45, "Ver Contacto", null },
                    { 46, "Crear Contacto", null },
                    { 47, "Editar Contacto", null },
                    { 48, "Eliminar Contacto", null },
                    { 49, "Ver Roles", null },
                    { 50, "Crear Roles", null },
                    { 51, "Editar Roles", null },
                    { 52, "Eliminar Roles", null },
                    { 53, "Ver Reportes de Usuarios", null },
                    { 54, "Crear Reportes de Usuarios", null },
                    { 55, "Editar Reportes de Usuarios", null },
                    { 56, "Eliminar Reportes de Usuarios", null },
                    { 57, "Ver Reportes de Ventas", null },
                    { 58, "Crear Reportes de Ventas", null },
                    { 59, "Editar Reportes de Ventas", null },
                    { 60, "Eliminar Reportes de Ventas", null },
                    { 61, "Ver Reportes de Pedidos", null },
                    { 62, "Crear Reportes de Pedidos", null },
                    { 63, "Editar Reportes de Pedidos", null },
                    { 64, "Eliminar Reportes de Pedidos", null },
                    { 65, "Ver Reportes de Inventario", null },
                    { 66, "Crear Reportes de Inventario", null },
                    { 67, "Editar Reportes de Inventario", null },
                    { 68, "Eliminar Reportes de Inventario", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNacimiento",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "edad",
                table: "Usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 5,
                column: "Nombre",
                value: "Ver Caja");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 6,
                column: "Nombre",
                value: "Crear Caja");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 7,
                column: "Nombre",
                value: "Editar Caja");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 8,
                column: "Nombre",
                value: "Eliminar Caja");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 9,
                column: "Nombre",
                value: "Ver Artículos");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 10,
                column: "Nombre",
                value: "Crear Artículos");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 11,
                column: "Nombre",
                value: "Editar Artículos");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 12,
                column: "Nombre",
                value: "Eliminar Artículos");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 13,
                column: "Nombre",
                value: "Ver Inventario");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 14,
                column: "Nombre",
                value: "Crear Inventario");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 15,
                column: "Nombre",
                value: "Editar Inventario");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 16,
                column: "Nombre",
                value: "Eliminar Inventario");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 17,
                column: "Nombre",
                value: "Ver Ventas");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 18,
                column: "Nombre",
                value: "Crear Ventas");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 19,
                column: "Nombre",
                value: "Editar Ventas");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 20,
                column: "Nombre",
                value: "Eliminar Ventas");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 21,
                column: "Nombre",
                value: "Ver Administración");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 22,
                column: "Nombre",
                value: "Crear Administración");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 23,
                column: "Nombre",
                value: "Editar Administración");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 24,
                column: "Nombre",
                value: "Eliminar Administración");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 25,
                column: "Nombre",
                value: "Ver Reportes");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 26,
                column: "Nombre",
                value: "Crear Reportes");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 27,
                column: "Nombre",
                value: "Editar Reportes");

            migrationBuilder.UpdateData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 28,
                column: "Nombre",
                value: "Eliminar Reportes");
        }
    }
}

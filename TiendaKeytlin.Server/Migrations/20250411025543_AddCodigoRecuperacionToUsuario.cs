using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiendaKeytlin.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddCodigoRecuperacionToUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoRecuperacion",
                table: "Usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaExpiracionCodigo",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoRecuperacion",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FechaExpiracionCodigo",
                table: "Usuarios");
        }
    }
}

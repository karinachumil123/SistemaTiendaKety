using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TiendaKeytlin.Server.Migrations
{
    /// <inheritdoc />
    public partial class Cierredecajas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ejecuta ALTER COLUMN con USING para convertir correctamente el tipo en PostgreSQL
            migrationBuilder.Sql(
                @"ALTER TABLE ""Aperturas"" ALTER COLUMN ""Fecha"" TYPE timestamp with time zone USING ""Fecha""::timestamp with time zone;"
            );

            // Comentamos o eliminamos el AlterColumn porque ya se hizo con SQL
            /*
            migrationBuilder.AlterColumn<DateTime>(
                name: "Fecha",
                table: "Aperturas",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
            */

            migrationBuilder.CreateTable(
                name: "SaldosCaja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SaldoAnterior = table.Column<decimal>(type: "numeric", nullable: false),
                    EntradasSalidas = table.Column<decimal>(type: "numeric", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaldosCaja", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cierres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreCajero = table.Column<string>(type: "text", nullable: false),
                    NumeroCaja = table.Column<string>(type: "text", nullable: false),
                    FechaApertura = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaCierre = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BaseCaja = table.Column<decimal>(type: "numeric", nullable: false),
                    SaldosId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cierres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cierres_SaldosCaja_SaldosId",
                        column: x => x.SaldosId,
                        principalTable: "SaldosCaja",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClasificacionCaja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Denominacion = table.Column<string>(type: "text", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    CierreCajaId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClasificacionCaja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClasificacionCaja_Cierres_CierreCajaId",
                        column: x => x.CierreCajaId,
                        principalTable: "Cierres",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cierres_SaldosId",
                table: "Cierres",
                column: "SaldosId");

            migrationBuilder.CreateIndex(
                name: "IX_ClasificacionCaja_CierreCajaId",
                table: "ClasificacionCaja",
                column: "CierreCajaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClasificacionCaja");

            migrationBuilder.DropTable(
                name: "Cierres");

            migrationBuilder.DropTable(
                name: "SaldosCaja");

            migrationBuilder.AlterColumn<string>(
                name: "Fecha",
                table: "Aperturas",
                type: "text",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }
    }
}

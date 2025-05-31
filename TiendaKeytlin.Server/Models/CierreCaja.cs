using System;
using System.Collections.Generic;

namespace TiendaKeytlin.Server.Models
{
    public class CierreCaja
    {
        public int Id { get; set; }
        public string NombreCajero { get; set; } = string.Empty;
        public string NumeroCaja { get; set; } = string.Empty;
        public DateTime? FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public decimal BaseCaja { get; set; }
        public List<ClasificacionCaja> Clasificaciones { get; set; } = new List<ClasificacionCaja>();
        public SaldosCaja Saldos { get; set; } = new SaldosCaja();
    }

    public class ClasificacionCaja
    {
        public int Id { get; set; }
        public string Denominacion { get; set; } = string.Empty;
        public decimal Valor { get; set; } // AGREGADO: para mantener consistencia
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }

        // Clave foránea para Entity Framework
        public int CierreCajaId { get; set; }
    }

    public class SaldosCaja
    {
        public int Id { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal EntradasSalidas { get; set; }
        public decimal Subtotal { get; set; } // AGREGADO: para el subtotal calculado
        public decimal Total { get; set; }

        // Clave foránea para Entity Framework
        public int CierreCajaId { get; set; }
    }
}
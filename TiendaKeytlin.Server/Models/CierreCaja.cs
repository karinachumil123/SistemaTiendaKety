using System;
using System.Collections.Generic;

namespace TiendaKeytlin.Server.Models
{
    public class CierreCaja
    {
        public int Id { get; set; }
        public string NombreCajero { get; set; }
        public string NumeroCaja { get; set; }
        public DateTime? FechaApertura { get; set; }  // Nullable DateTime
        public DateTime? FechaCierre { get; set; }    // Nullable DateTime
        public decimal BaseCaja { get; set; }

        public List<ClasificacionCaja> Clasificaciones { get; set; }
        public SaldosCaja Saldos { get; set; }
    }

    public class ClasificacionCaja
    {
        public int Id { get; set; }
        public string Denominacion { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class SaldosCaja
    {
        public int Id { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal EntradasSalidas { get; set; }
        public decimal Total { get; set; }
    }
}

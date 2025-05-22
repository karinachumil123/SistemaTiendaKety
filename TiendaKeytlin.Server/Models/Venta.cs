using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TiendaKeytlin.Server.Models
{
    public class Venta
    {
        public int Id { get; set; }

        [Required]
        public DateTime FechaVenta { get; set; }

        [Required]
        public decimal Subtotal { get; set; }

        [Required]
        public decimal Total { get; set; }

        [Required]
        public decimal MontoRecibido { get; set; }

        [Required]
        public decimal Cambio { get; set; }

        public string? Observaciones { get; set; }

        [Required]
        public int VendedorId { get; set; }

        [ForeignKey("VendedorId")]
        public virtual Usuario? Vendedor { get; set; }

        public virtual ICollection<DetalleVenta> DetallesVenta { get; set; } = new List<DetalleVenta>();
    }

}
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TiendaKeytlin.Server.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TiendaKeytlin.Server.Models
{
    public class Pedidos
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CodigoPedido { get; set; }
        public string NumeroPedido { get; set; } = string.Empty;


        [Required]
        public int CantidadPedido { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPedido { get; set; }

        [Required]
        public DateTime FechaPedido { get; set; }
        [Required(ErrorMessage = "El proveedor es obligatorio")]
        public int ProveedorId { get; set; }

        [Required]
        public int EstadoId { get; set; }
        [JsonIgnore]
        [ForeignKey("EstadoId")]
        public virtual EstadoUsuario Estado { get; set; }

        [JsonIgnore]
        [ForeignKey("ProveedorId")]
        public virtual Proveedor? Proveedor { get; set; }

        
    }
}


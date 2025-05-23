using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TiendaKeytlin.Server.Models
{
    public class DetallePedido
    {
        public int Id { get; set; }

        [Required]
        public int PedidoId { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        [Range(1, 100000)]
        public int Cantidad { get; set; }

        [Required]
        [Range(0, 999999)]
        public decimal PrecioUnitario { get; set; }

        [NotMapped]
        public decimal Subtotal => Cantidad * PrecioUnitario;

        [JsonIgnore]
        public virtual Pedido? Pedido { get; set; }

        [JsonIgnore]
        public virtual Productos? Producto { get; set; }
    }
}

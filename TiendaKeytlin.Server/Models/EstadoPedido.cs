using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TiendaKeytlin.Server.Models
{
    public class EstadoPedido
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del estado es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre del estado no debe exceder los 50 caracteres")]
        public string Nombre { get; set; }

        [JsonIgnore]
        public virtual ICollection<Pedido> Pedidos { get; set; } = new HashSet<Pedido>();
    }
}

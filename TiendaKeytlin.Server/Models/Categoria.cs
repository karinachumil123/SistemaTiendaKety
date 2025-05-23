using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TiendaKeytlin.Server.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TiendaKeytlin.Server.Models
{
    public class Categoria
    {
        public Categoria()
        {
            Productos = new HashSet<Productos>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string CategoriaNombre { get; set; }

        public string Descripcion { get; set; }

        [Required]
        public int EstadoUsuarioId { get; set; }

        [JsonIgnore]
        [ValidateNever] // <- clave para evitar validación automática del objeto anidado
        [ForeignKey("EstadoUsuarioId")]
        public virtual EstadoUsuario EstadoUsuario { get; set; }

        [JsonIgnore]
        public virtual ICollection<Productos> Productos { get; set; }
    }
}

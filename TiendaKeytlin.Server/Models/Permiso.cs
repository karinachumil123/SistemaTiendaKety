using System.ComponentModel.DataAnnotations;

namespace TiendaKeytlin.Server.Models
{
    public class Permiso
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }  
    }
}

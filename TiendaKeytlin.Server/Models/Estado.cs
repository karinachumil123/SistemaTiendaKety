using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TiendaKeytlin.Server.Models
{
    public class EstadoUsuario
    {
        public EstadoUsuario()
        {
            Usuarios = new HashSet<Usuario>();
        }

        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }

        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
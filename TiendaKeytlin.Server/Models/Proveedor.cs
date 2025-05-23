// Models/Proveedor.cs
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using TiendaKeytlin.Server.Migrations;

namespace TiendaKeytlin.Server.Models
{
    public class Proveedor
    {
        public Proveedor()
        {
            Productos = new HashSet<Productos>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; } // Cambiado de NombreEmpresa a Nombre
        public string NombreContacto { get; set; }
        public string Telefono { get; set; }
        public string TelefonoContacto { get; set; }
        public string Correo { get; set; } // Agregado para coincidir con frontend
        public string Estado { get; set; }
        public string? Direccion { get; set; } // Opcional
        public string? Descripcion { get; set; } // Opcional

        public virtual ICollection<Productos> Productos { get; set; }


    }
}

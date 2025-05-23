using Microsoft.EntityFrameworkCore;

namespace TiendaKeytlin.Server.DTOs
{
    public class VerificacionCodigoDto
    {
        public string Correo { get; set; }
        public string Codigo { get; set; }
    }
}

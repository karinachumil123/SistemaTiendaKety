namespace TiendaKeytlin.Server.Models
{
    public class AperturaCaja
    {
        public int Id { get; set; }
        public DateTime? Fecha { get; set; } = DateTime.MinValue;
        public decimal Monto { get; set; }
    }
}



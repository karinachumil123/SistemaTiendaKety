namespace TiendaKeytlin.Server.DTOs
{
    public class CrearPedidoDTO
    {
        public DateTime FechaPedido { get; set; } = DateTime.UtcNow;
        public int ProveedorId { get; set; }
        public int EstadoPedidoId { get; set; }
        public List<DetallePedidoDTO> Detalles { get; set; } = new();
    }

    public class DetallePedidoDTO
    {
        public int ProductoId { get; set; }


        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }


    }

}

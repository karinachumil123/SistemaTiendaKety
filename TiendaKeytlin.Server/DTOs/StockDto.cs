namespace TiendaKeytlin.Server.DTOs
{
    public class StockDto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CodigoProducto { get; set; } = string.Empty;

        public decimal PrecioVenta { get; set; }
        public decimal PrecioCompra { get; set; }

        public int StockDisponible { get; set; }

        public int ProveedorId { get; set; }
        public string NombreProveedor { get; set; } = string.Empty;

        public int CategoriaId { get; set; }
        public string NombreCategoria { get; set; } = string.Empty;

        public int EstadoProductoId { get; set; }
        public string NombreEstado { get; set; } = string.Empty;

        public string ImagenUrl { get; set; } = string.Empty;
    }
}

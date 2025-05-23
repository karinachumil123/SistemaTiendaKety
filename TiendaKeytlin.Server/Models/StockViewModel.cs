namespace TiendaKeytlin.Server.Models
{
    public class StockViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int StockDisponible { get; set; }
        public string Proveedor { get; set; }
        public string Categoria { get; set; }
        public string Estado { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal PrecioAdquisicion { get; set; }
        public string Imagen { get; set; }
    }
}

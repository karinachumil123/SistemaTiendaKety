using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaKeytlin.Server.Data;
using TiendaKeytlin.Server.DTOs;
using TiendaKeytlin.Server.Models;

[Route("api/[controller]")]
[ApiController]
public class StockController : ControllerBase
{
    private readonly AppDbContext _context;

    public StockController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet("stock")]
    public async Task<ActionResult<List<StockDto>>> ObtenerStockProductos()
    {
        var stock = await _context.Productos
            .Include(p => p.Proveedor)
            .Include(p => p.Categoria)
            .Include(p => p.Estado)
            .Select(p => new StockDto
            {
                ProductoId = p.Id,
                Nombre = p.Nombre,
                CodigoProducto = p.CodigoProducto,

                PrecioVenta = p.PrecioVenta,
                PrecioCompra = p.PrecioAdquisicion,

                StockDisponible = (
                    _context.DetallePedidos
                        .Where(dp => dp.ProductoId == p.Id && dp.Pedido.EstadoPedidoId == 3)
                        .Sum(dp => (int?)dp.Cantidad) ?? 0
                )
                - (
                    _context.DetalleVenta
                        .Where(dv => dv.ProductoId == p.Id)
                        .Sum(dv => (int?)dv.Cantidad) ?? 0
                ),

                ProveedorId = p.ProveedorId,
                NombreProveedor = p.Proveedor.Nombre,

                CategoriaId = p.CategoriaId,
                NombreCategoria = p.Categoria.CategoriaNombre,

                EstadoProductoId = p.EstadoId,
                NombreEstado = p.Estado.Nombre,

                ImagenUrl = p.Imagen
            })
            .ToListAsync();

        return Ok(stock);
    }

}


using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaKeytlin.Server.Data;

namespace TiendaKeytlin.Server.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("resumen")]
        public async Task<IActionResult> GetResumenDashboard()
        {
            var hoy = DateTime.UtcNow.Date;
            var mañana = hoy.AddDays(1);
            var primerDiaMes = new DateTime(hoy.Year, hoy.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var ventasHoy = await _context.Ventas
                .Where(v => v.FechaVenta >= hoy && v.FechaVenta < mañana)
                .SumAsync(v => (decimal?)v.Total) ?? 0;

            var ventasMes = await _context.Ventas
                .Where(v => v.FechaVenta >= primerDiaMes && v.FechaVenta < mañana)
                .SumAsync(v => (decimal?)v.Total) ?? 0;

            var productoMasVendido = await _context.DetalleVenta
                .GroupBy(d => d.ProductoId)
                .Select(g => new
                {
                    ProductoId = g.Key,
                    Cantidad = g.Sum(d => d.Cantidad)
                })
                .OrderByDescending(g => g.Cantidad)
                .Join(_context.Productos, g => g.ProductoId, p => p.Id, (g, p) => new
                {
                    p.Id,
                    p.Nombre,
                    g.Cantidad
                })
                .FirstOrDefaultAsync();

            var productos = await _context.Productos
                .Select(p => new
                {
                    p.Id,
                    p.Nombre,
                    Entradas = _context.DetallePedidos
                        .Where(pd => pd.ProductoId == p.Id)
                        .Sum(pd => (int?)pd.Cantidad) ?? 0,
                    Salidas = _context.DetalleVenta
                        .Where(dv => dv.ProductoId == p.Id)
                        .Sum(dv => (int?)dv.Cantidad) ?? 0
                })
                .ToListAsync();

            var productosBajoStock = productos
                .Select(p => new
                {
                    p.Id,
                    p.Nombre,
                    Stock = p.Entradas - p.Salidas
                })
                .Where(p => p.Stock <= 5)
                .ToList();

            // Últimas 5 ventas con productos vendidos
            var ultimasVentas = await _context.Ventas
                .OrderByDescending(v => v.FechaVenta)
                .Take(5)
                .Select(v => new
                {
                    v.Id,
                    v.FechaVenta,
                    v.Total,
                    Productos = _context.DetalleVenta
                        .Where(dv => dv.VentaId == v.Id)
                        .Join(_context.Productos,
                            dv => dv.ProductoId,
                            p => p.Id,
                            (dv, p) => new
                            {
                                p.Nombre,
                                dv.Cantidad
                            })
                        .ToList()
                })
                .ToListAsync();

            return Ok(new
            {
                ventasHoy,
                ventasMes,
                productoMasVendido,
                productosBajoStock,
                ultimasVentas
            });
        }

    }
    }


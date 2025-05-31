using Microsoft.AspNetCore.Mvc;
using TiendaKeytlin.Server.Data;
using TiendaKeytlin.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace TiendaKeytlin.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AperturasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AperturasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AgregarApertura([FromBody] AperturaCaja apertura)
        {
            try
            {
                _context.Aperturas.Add(apertura);
                await _context.SaveChangesAsync();
                return Ok(apertura);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("por-fecha/{fecha}")]
        public async Task<IActionResult> ObtenerPorFecha(string fecha)
        {
            try
            {
                if (DateTime.TryParse(fecha, out var fechaBusqueda))
                {
                    // Forzar a Kind.Unspecified para que sea compatible con PostgreSQL
                    fechaBusqueda = DateTime.SpecifyKind(fechaBusqueda.Date, DateTimeKind.Unspecified);
                    var siguienteDia = DateTime.SpecifyKind(fechaBusqueda.AddDays(1), DateTimeKind.Unspecified);

                    var apertura = await _context.Aperturas
                        .Where(a => a.Fecha.HasValue &&
                                    a.Fecha.Value >= fechaBusqueda &&
                                    a.Fecha.Value < siguienteDia)
                        .OrderByDescending(a => a.Id)
                        .FirstOrDefaultAsync();

                    if (apertura == null)
                        return NotFound(new { message = "No se encontró apertura para esta fecha" });

                    return Ok(apertura);
                }

                return BadRequest(new { message = "Formato de fecha inválido" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        // NUEVO: Endpoint para obtener todas las aperturas (útil para debugging)
        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            try
            {
                var aperturas = await _context.Aperturas
                    .OrderByDescending(a => a.Fecha)
                    .ToListAsync();
                return Ok(aperturas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // NUEVO: Endpoint para obtener la última apertura
        [HttpGet("ultima")]
        public async Task<IActionResult> ObtenerUltima()
        {
            try
            {
                var ultima = await _context.Aperturas
                    .OrderByDescending(a => a.Id)
                    .FirstOrDefaultAsync();

                if (ultima == null)
                    return NotFound(new { message = "No hay aperturas registradas" });

                return Ok(ultima);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
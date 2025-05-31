using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaKeytlin.Server.Data;
using TiendaKeytlin.Server.Models;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace TiendaKeytlin.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CierreCajaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CierreCajaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CrearCierre([FromBody] CierreCaja cierre)
        {
            try
            {
                // Buscar apertura si se proporcionó fecha de apertura
                if (cierre.FechaApertura.HasValue)
                {
                    var apertura = await _context.Aperturas
                        .Where(a => a.Fecha.HasValue &&
                               a.Fecha.Value.Date == cierre.FechaApertura.Value.Date)
                        .OrderByDescending(a => a.Id)
                        .FirstOrDefaultAsync();

                    if (apertura != null)
                    {
                        cierre.BaseCaja = apertura.Monto;
                    }
                }

                // Configurar las relaciones antes de guardar
                foreach (var clasificacion in cierre.Clasificaciones)
                {
                    clasificacion.CierreCajaId = 0; // Se asignará automáticamente
                }

                if (cierre.Saldos != null)
                {
                    cierre.Saldos.CierreCajaId = 0; // Se asignará automáticamente
                }

                // Agregar y guardar
                _context.Cierres.Add(cierre);
                await _context.SaveChangesAsync();

                // Retornar el cierre con sus relaciones cargadas
                var cierreGuardado = await _context.Cierres
                    .Include(c => c.Clasificaciones)
                    .Include(c => c.Saldos)
                    .FirstOrDefaultAsync(c => c.Id == cierre.Id);

                return Ok(cierreGuardado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message,
                    innerException = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                var cierres = await _context.Cierres
                    .Include(c => c.Clasificaciones)
                    .Include(c => c.Saldos)
                    .OrderByDescending(c => c.FechaCierre)
                    .ToListAsync();

                return Ok(cierres);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var cierre = await _context.Cierres
                    .Include(c => c.Clasificaciones)
                    .Include(c => c.Saldos)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (cierre == null)
                    return NotFound(new { message = "Cierre no encontrado" });

                return Ok(cierre);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarCierre(int id, [FromBody] CierreCaja cierreActualizado)
        {
            try
            {
                var cierreExistente = await _context.Cierres
                    .Include(c => c.Clasificaciones)
                    .Include(c => c.Saldos)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (cierreExistente == null)
                    return NotFound(new { message = "Cierre no encontrado" });

                // Actualizar propiedades principales
                cierreExistente.NombreCajero = cierreActualizado.NombreCajero;
                cierreExistente.NumeroCaja = cierreActualizado.NumeroCaja;
                cierreExistente.FechaApertura = cierreActualizado.FechaApertura;
                cierreExistente.FechaCierre = cierreActualizado.FechaCierre;
                cierreExistente.BaseCaja = cierreActualizado.BaseCaja;

                // Actualizar clasificaciones
                _context.Clasificaciones.RemoveRange(cierreExistente.Clasificaciones);
                foreach (var clasificacion in cierreActualizado.Clasificaciones)
                {
                    clasificacion.Id = 0; // Para que EF lo trate como nuevo
                    clasificacion.CierreCajaId = id;
                    cierreExistente.Clasificaciones.Add(clasificacion);
                }

                // Actualizar saldos
                if (cierreExistente.Saldos != null && cierreActualizado.Saldos != null)
                {
                    cierreExistente.Saldos.SaldoAnterior = cierreActualizado.Saldos.SaldoAnterior;
                    cierreExistente.Saldos.EntradasSalidas = cierreActualizado.Saldos.EntradasSalidas;
                    cierreExistente.Saldos.Subtotal = cierreActualizado.Saldos.Subtotal;
                    cierreExistente.Saldos.Total = cierreActualizado.Saldos.Total;
                }

                await _context.SaveChangesAsync();

                return Ok(cierreExistente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var cierre = await _context.Cierres
                    .Include(c => c.Clasificaciones)
                    .Include(c => c.Saldos)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (cierre == null)
                    return NotFound(new { message = "Cierre no encontrado" });

                _context.Cierres.Remove(cierre); // Las relaciones se eliminarán en cascada
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Endpoint adicional para obtener cierres por fecha
        [HttpGet("por-fecha/{fecha}")]
        public async Task<IActionResult> ObtenerPorFecha(string fecha)
        {
            try
            {
                if (DateTime.TryParse(fecha, out var fechaBusqueda))
                {
                    var cierres = await _context.Cierres
                        .Include(c => c.Clasificaciones)
                        .Include(c => c.Saldos)
                        .Where(c => c.FechaCierre.HasValue &&
                               c.FechaCierre.Value.Date == fechaBusqueda.Date)
                        .OrderByDescending(c => c.FechaCierre)
                        .ToListAsync();

                    return Ok(cierres);
                }
                return BadRequest(new { message = "Formato de fecha inválido" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
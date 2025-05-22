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
                if (cierre.FechaApertura.HasValue)
                {
                    var apertura = await _context.Aperturas
                        .Where(a => a.Fecha.Value.Date == cierre.FechaApertura.Value.Date)
                        .OrderByDescending(a => a.Id)
                        .FirstOrDefaultAsync();

                    if (apertura != null)
                    {
                        cierre.BaseCaja = apertura.Monto;
                    }
                }

                _context.Cierres.Add(cierre);
                await _context.SaveChangesAsync();
                return Ok(cierre);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var cierres = await _context.Cierres.ToListAsync();
            return Ok(cierres);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var cierre = await _context.Cierres.FindAsync(id);
            if (cierre == null) return NotFound();
            return Ok(cierre);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var cierre = await _context.Cierres.FindAsync(id);
            if (cierre == null) return NotFound();
            _context.Cierres.Remove(cierre);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

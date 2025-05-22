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
            _context.Aperturas.Add(apertura);
            await _context.SaveChangesAsync();
            return Ok(apertura);
        }

        [HttpGet("por-fecha/{fecha}")]
        public async Task<IActionResult> ObtenerPorFecha(string fecha)
        {
            if (DateTime.TryParse(fecha, out var fechaBusqueda))
            {
                var apertura = await _context.Aperturas
                    .Where(a => a.Fecha.HasValue &&
                           a.Fecha.Value.Date == fechaBusqueda.Date)
                    .OrderByDescending(a => a.Id)
                    .FirstOrDefaultAsync();

                if (apertura == null) return NotFound();

                return Ok(apertura);
            }
            return BadRequest("Formato de fecha inválido");
        }
    }
}


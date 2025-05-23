// Controllers/EmpresaController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaKeytlin.Server.Data;
using TiendaKeytlin.Server.Models;

namespace TiendaKeytlin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmpresaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Empresa
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var empresa = await _context.Empresa.FirstOrDefaultAsync();
            if (empresa == null)
            {
                return NotFound();
            }
            return Ok(empresa);
        }

        // PUT: api/Empresa
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Empresa empresa)
        {
            if (empresa == null)
            {
                return BadRequest();
            }

            var empresaExistente = await _context.Empresa.FirstOrDefaultAsync(e => e.Id == empresa.Id);
            if (empresaExistente == null)
            {
                return NotFound();
            }

            empresaExistente.Nombre = empresa.Nombre;
            empresaExistente.Telefono = empresa.Telefono;
            empresaExistente.Correo = empresa.Correo;
            empresaExistente.Direccion = empresa.Direccion;

            _context.Empresa.Update(empresaExistente);
            await _context.SaveChangesAsync();

            return Ok(empresaExistente);
        }
    }
}

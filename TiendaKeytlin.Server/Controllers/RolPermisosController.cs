using Microsoft.AspNetCore.Mvc;
using TiendaKeytlin.Server.Models;
using TiendaKeytlin.Server.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TiendaKeytlin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolPermisosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolPermisosController(AppDbContext context)
        {
            _context = context;
        }

        // Asignar permisos a un rol
        [HttpPost("{rolId}/asignar-permisos")]
        public async Task<IActionResult> AsignarPermisosARol(int rolId, [FromBody] List<int> permisoIds)
        {
            // Verificar si el rol existe
            var rol = await _context.Roles.FindAsync(rolId);
            if (rol == null)
            {
                return NotFound("El rol no existe.");
            }

            // Buscar los permisos en la base de datos
            var permisos = await _context.Permisos
                .Where(p => permisoIds.Contains(p.Id))
                .ToListAsync();

            if (!permisos.Any())
            {
                return NotFound("No se encontraron permisos.");
            }

            // Asignar los permisos al rol (solo si no están ya asignados)
            foreach (var permiso in permisos)
            {
                var rolPermisoExistente = await _context.RolPermisos
                    .FirstOrDefaultAsync(rp => rp.RolId == rolId && rp.PermisoId == permiso.Id);

                if (rolPermisoExistente == null)
                {
                    _context.RolPermisos.Add(new RolPermiso { RolId = rolId, PermisoId = permiso.Id });
                }
            }

            await _context.SaveChangesAsync();

            return Ok("Permisos asignados correctamente.");
        }


        // Obtener los permisos asignados a un rol
        [HttpGet("{rolId}/permisos")]
        public async Task<IActionResult> ObtenerPermisosDeRol(int rolId)
        {
            var permisos = await _context.RolPermisos
                .Where(rp => rp.RolId == rolId)
                .Select(rp => new { rp.Permiso.Id, rp.Permiso.Nombre }) // Solo id y nombre del permiso
                .ToListAsync();

            if (!permisos.Any())
            {
                return NotFound("Este rol no tiene permisos asignados.");
            }

            return Ok(permisos);
        }


        [HttpDelete("{rolId}/eliminar-permisos")]
        public async Task<IActionResult> EliminarPermisosDeRol(int rolId, [FromBody] List<int> permisoIds)
        {
            // Verificar si el rol existe
            var rol = await _context.Roles.FindAsync(rolId);
            if (rol == null)
            {
                return NotFound("El rol no existe.");
            }

            // Buscar los permisos en la base de datos
            var permisos = await _context.RolPermisos
                .Where(rp => rp.RolId == rolId && permisoIds.Contains(rp.PermisoId))
                .ToListAsync();

            if (!permisos.Any())
            {
                return NotFound("No se encontraron permisos asignados a este rol.");
            }

            // Eliminar los permisos asignados
            _context.RolPermisos.RemoveRange(permisos);
            await _context.SaveChangesAsync();

            return Ok("Permisos eliminados correctamente.");
        }

    }

    }

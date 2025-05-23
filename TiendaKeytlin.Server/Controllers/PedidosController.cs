using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaKeytlin.Server.Data;
using TiendaKeytlin.Server.DTOs;
using TiendaKeytlin.Server.Models;

namespace TiendaKeytlin.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PedidosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Pedidos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        {
            return await _context.Pedidos
                .Include(p => p.Proveedor)
                .Include(p => p.EstadoPedido)
                .Include(p => p.Detalles)
                  .ThenInclude(d => d.Producto)
                .ToListAsync();
        }

        // GET: api/Pedidos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedido(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Proveedor)
                .Include(p => p.EstadoPedido)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
                return NotFound();

            return pedido;
        }

        // POST: api/Pedidos
        [HttpPost]
        public async Task<ActionResult<Pedido>> CrearPedido(CrearPedidoDTO pedidoDTO)
        {
            // Validación (opcional pero recomendable)
            if (pedidoDTO.Detalles == null || !pedidoDTO.Detalles.Any())
                return BadRequest("Debe incluir al menos un detalle.");

            // Calcular total
            decimal total = pedidoDTO.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);

            // Crear el objeto Pedido
            var pedido = new Pedido
            {
                FechaPedido = pedidoDTO.FechaPedido,
                ProveedorId = pedidoDTO.ProveedorId,
                EstadoPedidoId = pedidoDTO.EstadoPedidoId,
                Total = total,
                Detalles = pedidoDTO.Detalles.Select(d => new DetallePedido
                {
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario
                }).ToList()
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPedido", new { id = pedido.Id }, pedido);
        }

        // PUT: api/Pedidos/5 - ACTUALIZADO para usar el mismo DTO que crear
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedido(int id, CrearPedidoDTO pedidoDTO)
        {
            var pedidoExistente = await _context.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedidoExistente == null)
                return NotFound();

            // Validación
            if (pedidoDTO.Detalles == null || !pedidoDTO.Detalles.Any())
                return BadRequest("Debe incluir al menos un detalle.");

            // Actualizar campos principales
            pedidoExistente.FechaPedido = pedidoDTO.FechaPedido;
            pedidoExistente.ProveedorId = pedidoDTO.ProveedorId;
            pedidoExistente.EstadoPedidoId = pedidoDTO.EstadoPedidoId;

            // Eliminar detalles existentes
            _context.DetallePedidos.RemoveRange(pedidoExistente.Detalles);

            // Agregar nuevos detalles
            pedidoExistente.Detalles = pedidoDTO.Detalles.Select(d => new DetallePedido
            {
                ProductoId = d.ProductoId,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario
            }).ToList();

            // Recalcular total
            pedidoExistente.Total = pedidoExistente.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Pedidos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedido(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
                return NotFound();

            _context.DetallePedidos.RemoveRange(pedido.Detalles);
            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/Pedidos/eliminar-logico/5 - NUEVO MÉTODO
        [HttpPut("eliminar-logico/{id}")]
        public async Task<IActionResult> EliminarPedidoLogico(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);

            if (pedido == null)
                return NotFound();

            // Asumiendo que tienes una propiedad 'Activo' o similar en tu modelo
            // Si no la tienes, necesitarás agregarla al modelo Pedido
            // pedido.Activo = false;

            // Por ahora, como alternativa, podemos cambiar el estado a "Cancelado" o similar
            // Buscar un estado que represente "eliminado" o "inactivo"
            var estadoInactivo = await _context.EstadoPedido
                .FirstOrDefaultAsync(e => e.Nombre.ToLower().Contains("cancelado") ||
                                         e.Nombre.ToLower().Contains("inactivo") ||
                                         e.Nombre.ToLower().Contains("eliminado"));

            if (estadoInactivo != null)
            {
                pedido.EstadoPedidoId = estadoInactivo.Id;
            }
            else
            {
                // Si no hay un estado específico, crear uno o usar un valor específico
                // Por ejemplo, usar el último estado disponible o crear lógica específica
                return BadRequest("No se encontró un estado apropiado para la eliminación lógica");
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/Pedidos/estados
        [HttpGet("estados")]
        public async Task<ActionResult<IEnumerable<EstadoPedido>>> GetEstadosPedido()
        {
            var estados = await _context.EstadoPedido.ToListAsync();
            return Ok(estados);
        }
    }
}
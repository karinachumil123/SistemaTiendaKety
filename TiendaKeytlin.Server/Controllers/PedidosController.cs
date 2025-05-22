using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaKeytlin.Server.Data;
using TiendaKeytlin.Server.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace TiendaKeytlin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PedidosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Pedidos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedidos>>> GetPedidos()
        {
            return await _context.Pedidos
                .Include(p => p.Proveedor)
                .Include(p => p.Estado)
                .ToListAsync();
        }

        // GET: api/Pedidos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pedidos>> GetPedido(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Proveedor)
                .Include(p => p.Estado)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
                return NotFound();

            return pedido;
        }

        // POST: api/Pedidos
        [HttpPost]
        public async Task<ActionResult<Pedidos>> PostPedido(Pedidos pedido)
        {
            // Obtener el último número correlativo del código de pedido
            int ultimoCodigo = await _context.Pedidos
                .OrderByDescending(p => p.CodigoPedido)
                .Select(p => p.CodigoPedido)
                .FirstOrDefaultAsync();

            pedido.CodigoPedido = ultimoCodigo + 1;

            // Formato del número de pedido: "PED-0001", "PED-0002", etc.
            pedido.NumeroPedido = $"PED-{pedido.CodigoPedido.ToString("D4")}";

            // Establecer la fecha actual automáticamente
            pedido.FechaPedido = DateTime.Now;

            // Aquí se puede validar que el estado inicial sea activo (ejemplo: 1 = Activo)
            if (pedido.EstadoId == 0)
                pedido.EstadoId = 1;

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPedido), new { id = pedido.Id }, pedido);
        }

        // PUT: api/Pedidos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedido(int id, Pedidos pedido)
        {
            if (id != pedido.Id)
                return BadRequest("El ID del pedido no coincide.");

            var pedidoExistente = await _context.Pedidos.FindAsync(id);
            if (pedidoExistente == null)
                return NotFound("Pedido no encontrado.");

            // Actualizar solo los campos editables
            pedidoExistente.ProveedorId = pedido.ProveedorId;
            pedidoExistente.EstadoId = pedido.EstadoId;

            _context.Entry(pedidoExistente).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE lógico: api/Pedidos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
                return NotFound("Pedido no encontrado.");

            // Cambio lógico del estado del pedido a inactivo (2 = Inactivo)
            pedido.EstadoId = 2;

            _context.Entry(pedido).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}


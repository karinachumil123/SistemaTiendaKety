using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaKeytlin.Server.Data;
using TiendaKeytlin.Server.Models;
using TiendaKeytlin.Server.DTOs;

namespace TiendaKeytlin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<VentaController> _logger;

        public VentaController(AppDbContext context, ILogger<VentaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Venta
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VentaResponseDto>>> GetVentas()
        {
            try
            {
                _logger.LogInformation("Obteniendo lista de ventas");
                var ventas = await _context.Ventas
                    .Include(v => v.Vendedor)
                    .Include(v => v.DetallesVenta)
                        .ThenInclude(d => d.Producto)
                    .OrderByDescending(v => v.FechaVenta)
                    .ToListAsync();

                var ventasResponse = ventas.Select(v => MapVentaToResponseDto(v)).ToList();
                return Ok(ventasResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener ventas: {ex.Message}");
                return StatusCode(500, $"Error interno al obtener ventas: {ex.Message}");
            }
        }

        // GET: api/Venta/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VentaResponseDto>> GetVenta(int id)
        {
            try
            {
                _logger.LogInformation($"Buscando venta con ID: {id}");
                var venta = await _context.Ventas
                    .Include(v => v.Vendedor)
                    .Include(v => v.DetallesVenta)
                        .ThenInclude(d => d.Producto)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (venta == null)
                {
                    _logger.LogWarning($"Venta con ID {id} no encontrada");
                    return NotFound($"No se encontró la venta con ID {id}");
                }

                var ventaResponse = MapVentaToResponseDto(venta);
                return Ok(ventaResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener venta {id}: {ex.Message}");
                return StatusCode(500, $"Error interno al obtener venta: {ex.Message}");
            }
        }

        // GET: api/Venta/Recibo/5
        [HttpGet("Recibo/{id}")]
        public async Task<ActionResult<ReciboVentaDto>> GetReciboVenta(int id)
        {
            try
            {
                _logger.LogInformation($"Generando recibo para venta con ID: {id}");
                var venta = await _context.Ventas
                    .Include(v => v.Vendedor)
                    .Include(v => v.DetallesVenta)
                        .ThenInclude(d => d.Producto)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (venta == null)
                {
                    _logger.LogWarning($"Venta con ID {id} no encontrada para generar recibo");
                    return NotFound($"No se encontró la venta con ID {id}");
                }

                var recibo = new ReciboVentaDto
                {
                    VentaId = venta.Id,
                    FechaVenta = venta.FechaVenta,
                    NombreVendedor = venta.Vendedor?.Nombre ?? "No disponible",
                    EmailVendedor = venta.Vendedor?.Correo ?? "No disponible",
                    TelefonoVendedor = venta.Vendedor?.Telefono ?? "No disponible",
                    DireccionTienda = "Tienda Keytlin - Dirección Principal",
                    Subtotal = venta.Subtotal,
                    Total = venta.Total,
                    MontoRecibido = venta.MontoRecibido,
                    Cambio = venta.Cambio,
                    Productos = venta.DetallesVenta?.Select(d => new DetalleVentaResponseDto
                    {
                        Id = d.Id,
                        ProductoId = d.ProductoId,
                        NombreProducto = d.Producto?.Nombre ?? "Producto no disponible",
                        CodigoProducto = d.Producto?.CodigoProducto ?? "Sin código",
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Subtotal
                    }).ToList() ?? new List<DetalleVentaResponseDto>()
                };

                return Ok(recibo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al generar recibo para venta {id}: {ex.Message}");
                return StatusCode(500, $"Error interno al generar recibo: {ex.Message}");
            }
        }

        // POST: api/Venta
        [HttpPost]
        public async Task<ActionResult<VentaResponseDto>> PostVenta(VentaCreateDto ventaDto)
        {
            try
            {
                _logger.LogInformation("Iniciando creación de nueva venta con datos: {@VentaDto}", ventaDto);

                // Validar vendedor
                var vendedor = await _context.Usuarios.FindAsync(ventaDto.VendedorId);
                if (vendedor == null)
                {
                    _logger.LogWarning("Vendedor no encontrado: {VendedorId}", ventaDto.VendedorId);
                    return BadRequest($"El vendedor con ID {ventaDto.VendedorId} no existe");
                }

                // Validar productos antes de iniciar la transacción
                foreach (var detalleDto in ventaDto.DetallesVenta)
                {
                    var producto = await _context.Productos.FindAsync(detalleDto.ProductoId);
                    if (producto == null)
                    {
                        _logger.LogWarning("Producto no encontrado: {ProductoId}", detalleDto.ProductoId);
                        return BadRequest($"El producto con ID {detalleDto.ProductoId} no existe");
                    }
                }

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Crear la venta sin detalles primero
                    var venta = new Venta
                    {
                        FechaVenta = DateTime.UtcNow,
                        Subtotal = ventaDto.Subtotal,
                        Total = ventaDto.Total,
                        MontoRecibido = ventaDto.MontoRecibido,
                        Cambio = ventaDto.Cambio,
                        Observaciones = ventaDto.Observaciones,
                        VendedorId = ventaDto.VendedorId
                        // No inicializamos DetallesVenta aquí
                    };

                    _logger.LogInformation("Guardando venta base");
                    _context.Ventas.Add(venta);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Venta base guardada con ID: {VentaId}", venta.Id);

                    // Ahora agregamos los detalles uno por uno
                    foreach (var detalleDto in ventaDto.DetallesVenta)
                    {
                        var detalle = new DetalleVenta
                        {
                            VentaId = venta.Id,
                            ProductoId = detalleDto.ProductoId,
                            Cantidad = detalleDto.Cantidad,
                            PrecioUnitario = detalleDto.PrecioUnitario,
                            Subtotal = detalleDto.Subtotal
                        };

                        _logger.LogInformation("Agregando detalle: {@Detalle}", new
                        {
                            VentaId = detalle.VentaId,
                            ProductoId = detalle.ProductoId,
                            Cantidad = detalle.Cantidad
                        });

                        _context.Add(detalle);
                        // Guardamos cada detalle individualmente para detectar errores específicos
                        try
                        {
                            await _context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error al guardar detalle: {ProductoId}, {Mensaje}",
                                detalleDto.ProductoId, ex.InnerException?.Message ?? ex.Message);
                            throw; // Re-lanzamos para que se maneje en el catch externo
                        }
                    }

                    await transaction.CommitAsync();
                    _logger.LogInformation("Transacción completada exitosamente");

                    // Cargamos la venta completa con sus relaciones
                    var ventaCompleta = await _context.Ventas
                        .Include(v => v.Vendedor)
                        .Include(v => v.DetallesVenta)
                            .ThenInclude(d => d.Producto)
                        .FirstOrDefaultAsync(v => v.Id == venta.Id);

                    if (ventaCompleta == null)
                    {
                        return StatusCode(500, "Error interno: No se pudo recuperar la venta creada");
                    }

                    var ventaResponse = MapVentaToResponseDto(ventaCompleta);
                    return CreatedAtAction(nameof(GetVenta), new { id = venta.Id }, ventaResponse);
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();

                    // Capturar el mensaje de error interno
                    string innerMessage = dbEx.InnerException?.Message ?? "No hay detalles adicionales";
                    _logger.LogError(dbEx, "Error de base de datos al guardar la venta: {Message}. Inner: {InnerMessage}",
                        dbEx.Message, innerMessage);

                    return StatusCode(500, $"Error al guardar en base de datos: {dbEx.Message}. Detalle: {innerMessage}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    // Capturar el mensaje de error interno
                    string innerMessage = ex.InnerException?.Message ?? "No hay detalles adicionales";
                    _logger.LogError(ex, "Error al procesar la venta: {Message}. Inner: {InnerMessage}",
                        ex.Message, innerMessage);

                    return StatusCode(500, $"Error al crear la venta: {ex.Message}. Detalle: {innerMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al procesar la solicitud: {Message}", ex.Message);
                return StatusCode(500, $"Error en la solicitud: {ex.Message}");
            }
        }

        private VentaResponseDto MapVentaToResponseDto(Venta venta)
        {
            if (venta == null)
            {
                throw new ArgumentNullException(nameof(venta));
            }

            return new VentaResponseDto
            {
                Id = venta.Id,
                FechaVenta = venta.FechaVenta,
                Subtotal = venta.Subtotal,
                Total = venta.Total,
                MontoRecibido = venta.MontoRecibido,
                Cambio = venta.Cambio,
                Observaciones = venta.Observaciones,
                Vendedor = venta.Vendedor != null ? new VendedorDto
                {
                    Id = venta.Vendedor.Id,
                    Nombre = venta.Vendedor.Nombre ?? "Sin nombre",
                    Email = venta.Vendedor.Correo ?? "Sin correo",
                    Telefono = venta.Vendedor.Telefono ?? "Sin teléfono"
                } : new VendedorDto
                {
                    Id = 0,
                    Nombre = "Vendedor no disponible",
                    Email = "No disponible",
                    Telefono = "No disponible"
                },
                DetallesVenta = venta.DetallesVenta?.Select(d => new DetalleVentaResponseDto
                {
                    Id = d.Id,
                    ProductoId = d.ProductoId,
                    NombreProducto = d.Producto?.Nombre ?? "Producto no disponible",
                    CodigoProducto = d.Producto?.CodigoProducto ?? "Sin código",
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList() ?? new List<DetalleVentaResponseDto>()
            };
        }
    }
}
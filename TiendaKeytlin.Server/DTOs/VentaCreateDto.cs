using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TiendaKeytlin.Server.DTOs
{
    // DTOs para crear una nueva venta
    public class VentaCreateDto
    {
        [Required]
        public decimal Subtotal { get; set; }

        [Required]
        public decimal Total { get; set; }

        [Required]
        public decimal MontoRecibido { get; set; }

        [Required]
        public decimal Cambio { get; set; }

        public string? Observaciones { get; set; }

        [Required]
        public int VendedorId { get; set; }

        [Required]
        public List<DetalleVentaCreateDto> DetallesVenta { get; set; } = new List<DetalleVentaCreateDto>();
    }

    public class DetalleVentaCreateDto
    {
        [Required]
        public int ProductoId { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public decimal PrecioUnitario { get; set; }

        [Required]
        public decimal Subtotal { get; set; }
    }

    // DTOs para respuesta
    public class VentaResponseDto
    {
        public int Id { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public decimal MontoRecibido { get; set; }
        public decimal Cambio { get; set; }
        public string? Observaciones { get; set; }
        public VendedorDto Vendedor { get; set; }
        public List<DetalleVentaResponseDto> DetallesVenta { get; set; } = new List<DetalleVentaResponseDto>();
    }

    public class DetalleVentaResponseDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; }
        public string CodigoProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class VendedorDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
    }

    // DTO para el recibo
    public class ReciboVentaDto
    {
        public int VentaId { get; set; }
        public DateTime FechaVenta { get; set; }
        public string NombreVendedor { get; set; }
        public string EmailVendedor { get; set; }
        public string TelefonoVendedor { get; set; }
        public string DireccionTienda { get; set; }
        public List<DetalleVentaResponseDto> Productos { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public decimal MontoRecibido { get; set; }
        public decimal Cambio { get; set; }
    }
}
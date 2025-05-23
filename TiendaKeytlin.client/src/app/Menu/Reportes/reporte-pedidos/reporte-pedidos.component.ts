import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common'; 
import Swal from 'sweetalert2';
import { AuthService } from '../../../services/auth.service';
import { FormsModule } from '@angular/forms';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { EmpresaService } from '../../../services/empresa.service';
import { PedidoService } from '../../../services/pedido.service';

// Interfaz para el pedido con datos completos para el reporte
export interface PedidoReporte {
  id: number;
  fechaPedido: string;
  proveedorId: number;
  nombreProveedor: string;
  estadoPedidoId: number;
  nombreEstado: string;
  totalProductos: number;
  valorTotal: number;
  detalles: DetallePedido[];
}

export interface DetallePedido {
  productoId: number;
  nombreProducto: string;
  cantidad: number;
  precioUnitario: number;
  subtotal: number;
}

@Component({
  selector: 'app-reporte-pedidos',
  standalone: true,
  templateUrl: './reporte-pedidos.component.html',
  styleUrl: './reporte-pedidos.component.css',
  imports: [FormsModule, CommonModule]
})

export class ReportePedidosComponent implements OnInit {
  // Datos principales
  pedidos: any[] = [];
  originalPedidos: any[] = [];
  proveedores: any[] = [];
  estados: any[] = [];

  empresa: any = {
    nombre: '',
    telefono: '',
    correo: '',
    direccion: ''
  };

  usuarioActual: any = null;

  // Datos para mostrar en el reporte
  reportMonth: string = '';
  reportDate: string = '';
  reportUser: string = '';

  // Filtros
  filtroTexto: string = '';
  filtroProveedor: string = '';
  filtroEstado: string = '';
  fechaInicio: string = '';
  fechaFin: string = '';

  // Paginación
  paginaActual: number = 1;
  elementosPorPagina: number = 10;

  constructor(
    private authService: AuthService,
    private empresaService: EmpresaService,
    private pedidoService: PedidoService
  ) {}

  ngOnInit(): void {
    this.cargarPedidos();
    this.cargarProveedores();
    this.cargarEstados();
    this.cargarEmpresa();
    this.obtenerUsuarioActual();
  }

  inicializarDatosReporte(): void {
    console.log('Usuario actual:', this.usuarioActual);

    const now = new Date();
    const year = now.getFullYear();

    const diasSemana = ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'];
    const diaNombre = diasSemana[now.getDay()];
    const diaNumero = now.getDate();

    const horas = now.getHours().toString().padStart(2, '0');
    const minutos = now.getMinutes().toString().padStart(2, '0');

    this.reportDate = `${diaNombre} ${diaNumero} - Hora ${horas}:${minutos}`;

    const meses = [
      'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
      'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
    ];
    this.reportMonth = `${meses[now.getMonth()]} ${year}`;

    this.reportUser = this.usuarioActual 
      ? `${this.usuarioActual.name} ${this.usuarioActual.apellido}` 
      : 'Administrador';
  }

  limpiarFiltros(): void {
    this.filtroTexto = '';
    this.filtroProveedor = '';
    this.filtroEstado = '';
    this.fechaInicio = '';
    this.fechaFin = '';
    this.pedidos = [...this.originalPedidos];
    this.paginaActual = 1;
  }

  cargarPedidos(): void {
    this.pedidoService.obtenerPedidos().subscribe({
      next: (data) => {
        this.originalPedidos = data;
        this.pedidos = [...data];
        this.verificarPropiedadesPedidos();
      },
      error: (error) => {
        Swal.fire({
          icon: 'error',
          title: 'Error al cargar pedidos',
          text: error.message || 'Hubo un problema al cargar los pedidos.'
        });
      }
    });
  }

  cargarProveedores(): void {
    this.pedidoService.obtenerProveedores().subscribe({
      next: (data) => {
        this.proveedores = data;
      },
      error: (error) => {
        console.error('Error al cargar proveedores:', error);
      }
    });
  }

  cargarEstados(): void {
    this.pedidoService.obtenerEstados().subscribe({
      next: (data) => {
        this.estados = data;
      },
      error: (error) => {
        console.error('Error al cargar estados:', error);
      }
    });
  }

  cargarEmpresa(): void {
    this.empresaService.obtenerEmpresa().subscribe({
      next: (data) => {
        this.empresa = data;
      },
      error: (error) => {
        console.error('Error al cargar empresa:', error);
      }
    });
  }

  obtenerUsuarioActual(): void {
    this.authService.getUsuarioActual().subscribe((usuario: any) => {
      this.usuarioActual = usuario;
      this.inicializarDatosReporte();
    });
  }

  verificarPropiedadesPedidos(): void {
    if (this.pedidos.length > 0) {
      console.log('Estructura de pedidos:', Object.keys(this.pedidos[0]));
    }
  }

  buscarPedidos(): void {
    let pedidosFiltrados = [...this.originalPedidos];

    // Filtro por texto general
    if (this.filtroTexto.trim()) {
      const filtro = this.filtroTexto.trim().toLowerCase();
      pedidosFiltrados = pedidosFiltrados.filter(pedido =>
        pedido.id?.toString().includes(filtro) ||
        pedido.proveedorNombre?.toLowerCase().includes(filtro) ||
        pedido.estadoNombre?.toLowerCase().includes(filtro)
      );
    }

    // Filtro por proveedor
    if (this.filtroProveedor) {
      pedidosFiltrados = pedidosFiltrados.filter(pedido =>
        pedido.proveedorId?.toString() === this.filtroProveedor
      );
    }

    // Filtro por estado
    if (this.filtroEstado) {
      pedidosFiltrados = pedidosFiltrados.filter(pedido =>
        pedido.estadoPedidoId?.toString() === this.filtroEstado
      );
    }

    // Filtro por fecha
    if (this.fechaInicio && this.fechaFin) {
      const fechaIni = new Date(this.fechaInicio);
      const fechaFinal = new Date(this.fechaFin);
      
      pedidosFiltrados = pedidosFiltrados.filter(pedido => {
        const fechaPedido = new Date(pedido.fechaPedido);
        return fechaPedido >= fechaIni && fechaPedido <= fechaFinal;
      });
    }

    this.pedidos = pedidosFiltrados;
    this.paginaActual = 1;
  }

  pedidosPaginados(): any[] {
    const inicio = (this.paginaActual - 1) * this.elementosPorPagina;
    return this.pedidos.slice(inicio, inicio + this.elementosPorPagina);
  }

  exportarPDFFrontend(): void {
    if (!this.usuarioActual) {
      this.authService.getUsuarioActual().subscribe((usuario: any) => {
        this.usuarioActual = usuario;
        this.inicializarDatosReporte();
        this.generarPDF();
      });
    } else {
      this.generarPDF();
    }
  }

  generarPDF(): void {
    this.inicializarDatosReporte();

    const element = document.getElementById('reportePedidos');
    if (!element) {
      console.error('Elemento del reporte no encontrado');
      return;
    }

    setTimeout(() => {
      const clone = element.cloneNode(true) as HTMLElement;

      clone.style.width = '794px';
      clone.style.maxWidth = '100%';
      clone.style.overflow = 'hidden';

      document.body.appendChild(clone);

      const tabla = clone.querySelector('.tabla-pedidos') as HTMLTableElement;
      if (tabla) {
        tabla.style.borderCollapse = 'collapse';
        tabla.style.tableLayout = 'auto';
        tabla.style.width = '100%';

        const celdas = tabla.querySelectorAll('th, td');
        celdas.forEach((celda) => {
          const el = celda as HTMLElement;
          el.style.padding = '4px 6px';
          el.style.whiteSpace = 'nowrap';
          el.style.overflow = 'visible';
          el.style.textOverflow = 'clip';
          el.style.fontSize = '12px';
          el.style.fontFamily = 'Arial, sans-serif';
        });
      }

      html2canvas(clone, { scale: 2 }).then((canvas) => {
        const imgData = canvas.toDataURL('image/png');
        const pdf = new jsPDF('portrait', 'mm', 'a4');
        const pdfWidth = 210;
        const pdfHeight = 297;

        const imgProps = pdf.getImageProperties(imgData);
        const scale = Math.min(pdfWidth / imgProps.width, pdfHeight / imgProps.height);
        const imgWidth = imgProps.width * scale;
        const imgHeight = imgProps.height * scale;

        pdf.addImage(imgData, 'PNG', 0, 0, imgWidth, imgHeight);
        const fileName = `reporte_pedidos_${new Date().toISOString().replace(/[:.]/g, '-')}.pdf`;
        pdf.save(fileName);

        document.body.removeChild(clone);
      });
    }, 250);
  }

  // Métodos de paginación
  get totalPaginas(): number {
    return Math.ceil(this.pedidos.length / this.elementosPorPagina);
  }

  paginaAnterior(): void {
    if (this.paginaActual > 1) {
      this.paginaActual--;
    }
  }

  paginaSiguiente(): void {
    if (this.paginaActual < this.totalPaginas) {
      this.paginaActual++;
    }
  }

  // Métodos de utilidad para los pedidos
  get totalPedidos(): number {
    return this.pedidos.length;
  }

  get valorTotalPedidos(): number {
    return this.pedidos.reduce((total, pedido) => {
      const valorPedido = this.calcularValorPedido(pedido);
      return total + valorPedido;
    }, 0);
  }

  get totalProductosPedidos(): number {
    return this.pedidos.reduce((total, pedido) => {
      const cantidadProductos = pedido.detalles?.reduce((sum: number, detalle: any) => 
        sum + (detalle.cantidad || 0), 0) || 0;
      return total + cantidadProductos;
    }, 0);
  }

  calcularValorPedido(pedido: any): number {
    if (!pedido.detalles || !Array.isArray(pedido.detalles)) {
      return 0;
    }
    return pedido.detalles.reduce((total: number, detalle: any) => 
      total + ((detalle.cantidad || 0) * (detalle.precioUnitario || 0)), 0
    );
  }

  calcularCantidadProductos(pedido: any): number {
    if (!pedido.detalles || !Array.isArray(pedido.detalles)) {
      return 0;
    }
    return pedido.detalles.reduce((total: number, detalle: any) => 
      total + (detalle.cantidad || 0), 0
    );
  }

  // Método para obtener color del estado
  getColorEstado(estado: string): string {
    switch (estado?.toLowerCase()) {
      case 'pendiente': return '#fef9c3'; // Amarillo
      case 'en proceso': return '#dbeafe'; // Azul claro
      case 'completado': return '#d1fae5'; // Verde
      case 'cancelado': return '#fee2e2'; // Rojo
      default: return '#f3f4f6'; // Gris
    }
  }

  getColorTextoEstado(estado: string): string {
    switch (estado?.toLowerCase()) {
      case 'pendiente': return '#78350f'; // Marrón
      case 'en proceso': return '#1e40af'; // Azul oscuro
      case 'completado': return '#065f46'; // Verde oscuro
      case 'cancelado': return '#991b1b'; // Rojo oscuro
      default: return '#374151'; // Gris oscuro
    }
  }

  formatearFecha(fecha: string): string {
    if (!fecha) return 'N/A';
    const date = new Date(fecha);
    return date.toLocaleDateString('es-ES', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  obtenerNombreProveedor(proveedorId: number): string {
    const proveedor = this.proveedores.find(p => p.id === proveedorId);
    return proveedor?.nombre || 'N/A';
  }

  obtenerNombreEstado(estadoId: number): string {
    const estado = this.estados.find(e => e.id === estadoId);
    return estado?.nombre || 'N/A';
  }
}
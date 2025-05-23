import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common'; 
import Swal from 'sweetalert2';
import { AuthService } from '../../../services/auth.service';
import { FormsModule } from '@angular/forms';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { EmpresaService } from '../../../services/empresa.service';
import { ProductosService, ProductoConStock } from '../../../services/stock.service';

@Component({
  selector: 'app-reporte-inventario',
  standalone: true,
  templateUrl: './reporte-inventario.component.html',
  styleUrl: './reporte-inventario.component.css',
  imports: [FormsModule, CommonModule]
})

export class ReporteInventarioComponent implements OnInit {
  // Datos principales
  productos: ProductoConStock[] = [];
  originalProductos: ProductoConStock[] = [];

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
  filtroProducto: string = '';
  fechaInicio: string = '';
  fechaFin: string = '';

  // Paginación
  paginaActual: number = 1;
  elementosPorPagina: number = 10;

  constructor(
    private authService: AuthService,
    private empresaService: EmpresaService,
    private productosService: ProductosService
  ) {}

  ngOnInit(): void {
    this.cargarInventario();
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
    this.filtroProducto = '';
    this.fechaInicio = '';
    this.fechaFin = '';
    this.productos = [...this.originalProductos];
  }

  cargarInventario(): void {
    this.productosService.obtenerStockProductos().subscribe({
      next: (data) => {
        this.originalProductos = data;
        this.productos = [...data];
        this.verificarPropiedadesInventario();
      },
      error: (error) => {
        Swal.fire({
          icon: 'error',
          title: 'Error al cargar inventario',
          text: error.message || 'Hubo un problema al cargar el inventario.'
        });
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

  verificarPropiedadesInventario(): void {
    if (this.productos.length > 0) {
      console.log('Estructura de inventario:', Object.keys(this.productos[0]));
    }
  }

  buscarProducto(): void {
    const filtro = this.filtroProducto.trim().toLowerCase();

    if (!filtro) {
      this.productos = [...this.originalProductos];
      return;
    }

    this.productos = this.originalProductos.filter(producto =>
      producto.nombre.toLowerCase().includes(filtro) ||
      producto.codigoProducto.toLowerCase().includes(filtro) ||
      producto.nombreCategoria.toLowerCase().includes(filtro) ||
      producto.nombreProveedor.toLowerCase().includes(filtro)
    );
  }

  filtrarPorFecha(): void {
    // Nota: Como la interfaz ProductoConStock no tiene fecha de creación,
    // este filtro no aplicará a menos que agregues ese campo a tu interfaz
    Swal.fire({
      icon: 'info',
      title: 'Filtro por fecha no disponible',
      text: 'El inventario no cuenta con fechas de creación para filtrar.'
    });
  }

  inventarioPaginado(): ProductoConStock[] {
    const inicio = (this.paginaActual - 1) * this.elementosPorPagina;
    return this.productos.slice(inicio, inicio + this.elementosPorPagina);
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

    const element = document.getElementById('reporteInventario');
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

      const tabla = clone.querySelector('.tabla-inventario') as HTMLTableElement;
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
        const fileName = `reporte_inventario_${new Date().toISOString().replace(/[:.]/g, '-')}.pdf`;
        pdf.save(fileName);

        document.body.removeChild(clone);
      });
    }, 250);
  }

  // Métodos de paginación
  get totalPaginas(): number {
    return Math.ceil(this.productos.length / this.elementosPorPagina);
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

  // Métodos de utilidad para el inventario
  get totalProductos(): number {
    return this.productos.length;
  }

  get totalStock(): number {
    return this.productos.reduce((total, producto) => total + producto.stockDisponible, 0);
  }

  get valorTotalInventario(): number {
    return this.productos.reduce((total, producto) => 
      total + (producto.precioCompra * producto.stockDisponible), 0
    );
  }

  // Método para obtener color del estado de stock
  getColorStock(stock: number): string {
    if (stock === 0) return '#fee2e2'; // Rojo claro para sin stock
    if (stock <= 5) return '#fef9c3'; // Amarillo para stock bajo
    return '#d1fae5'; // Verde para stock normal
  }

  getColorTextoStock(stock: number): string {
    if (stock === 0) return '#991b1b'; // Rojo oscuro
    if (stock <= 5) return '#78350f'; // Marrón
    return '#065f46'; // Verde oscuro
  }
}
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common'; 
import Swal from 'sweetalert2';
import { AuthService } from '../../../services/auth.service';
import { FormsModule } from '@angular/forms';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { EmpresaService } from '../../../services/empresa.service';
import { VentaService } from '../../../services/ventas.service';

@Component({
  selector: 'app-reporte-venta',
  templateUrl: './reporte-venta.component.html',
  styleUrl: './reporte-venta.component.css',
  imports: [FormsModule, CommonModule]
})

export class ReporteVentaComponent implements OnInit {
  ventas: any[] = [];
  ventasOriginales: any[] = [];

  empresa: any = {
    nombre: '',
    telefono: '',
    correo: '',
    direccion: ''
  };

  usuarioActual: any = null;

  // Datos del reporte
  reportMonth: string = '';
  reportDate: string = '';
  reportUser: string = '';

  // Filtros
  filtroVenta: string = '';
  fechaInicio: string = '';
  fechaFin: string = '';

  // Paginación
  paginaActual: number = 1;
  elementosPorPagina: number = 10;

  constructor(
    private ventaService: VentaService,
    private authService: AuthService,
    private empresaService: EmpresaService
  ) {}

  ngOnInit(): void {
    this.cargarVentas();
    this.cargarEmpresa();
    this.obtenerUsuarioActual();
  }

  inicializarDatosReporte(): void {
    const now = new Date();
    const year = now.getFullYear();
    const diaNombre = ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'][now.getDay()];
    const diaNumero = now.getDate();
    const horas = now.getHours().toString().padStart(2, '0');
    const minutos = now.getMinutes().toString().padStart(2, '0');
    const meses = ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
                   'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'];

    this.reportDate = `${diaNombre} ${diaNumero} - Hora ${horas}:${minutos}`;
    this.reportMonth = `${meses[now.getMonth()]} ${year}`;
    this.reportUser = this.usuarioActual 
      ? `${this.usuarioActual.name} ${this.usuarioActual.apellido}` 
      : 'Administrador';
  }

  cargarVentas(): void {
    this.ventaService.obtenerVentas().subscribe({
      next: (data) => {
        this.ventasOriginales = data;
        this.ventas = [...data];
        this.verificarEstructuraVentas();
      },
      error: (error) => {
        Swal.fire({
          icon: 'error',
          title: 'Error al cargar ventas',
          text: error.message || 'Hubo un problema al cargar las ventas.'
        });
      }
    });
  }

  cargarEmpresa(): void {
    this.empresaService.obtenerEmpresa().subscribe({
      next: (data) => this.empresa = data,
      error: (error) => console.error('Error al cargar empresa:', error)
    });
  }

  obtenerUsuarioActual(): void {
    this.authService.getUsuarioActual().subscribe((usuario: any) => {
      this.usuarioActual = usuario;
      this.inicializarDatosReporte();
    });
  }

  limpiarFiltros(): void {
    this.filtroVenta = '';
    this.fechaInicio = '';
    this.fechaFin = '';
    this.ventas = [...this.ventasOriginales];
  }

  buscarVenta(): void {
    const filtro = this.filtroVenta.trim().toLowerCase();
    if (!filtro) {
      this.ventas = [...this.ventasOriginales];
      return;
    }

    this.ventas = this.ventasOriginales.filter(venta =>
      venta.nombre?.toLowerCase().includes(filtro) ||
      venta.apellido?.toLowerCase().includes(filtro) ||
      venta.correo?.toLowerCase().includes(filtro)
    );
  }

  filtrarPorFecha(): void {
    if (!this.fechaInicio || !this.fechaFin) {
      Swal.fire({ icon: 'warning', title: 'Fechas incompletas', text: 'Seleccione ambas fechas.' });
      return;
    }

    const inicio = new Date(this.fechaInicio);
    const fin = new Date(this.fechaFin);
    if (fin < inicio) {
      Swal.fire({ icon: 'warning', title: 'Rango inválido', text: 'La fecha final no puede ser anterior.' });
      return;
    }

    const inicioNormalizado = new Date(inicio.getFullYear(), inicio.getMonth(), inicio.getDate());
    const finNormalizado = new Date(fin.getFullYear(), fin.getMonth(), fin.getDate());

    this.ventas = this.ventasOriginales.filter(venta => {
      const fecha = new Date(venta.fechaVenta);
      const fechaNormalizada = new Date(fecha.getFullYear(), fecha.getMonth(), fecha.getDate());
      return fechaNormalizada >= inicioNormalizado && fechaNormalizada <= finNormalizado;
    });
  }

  ventasPaginadas(): any[] {
    const inicio = (this.paginaActual - 1) * this.elementosPorPagina;
    return this.ventas.slice(inicio, inicio + this.elementosPorPagina);
  }

  verificarEstructuraVentas(): void {
    if (this.ventas.length > 0) {
      console.log('Estructura de Venta:', Object.keys(this.ventas[0]));
      console.log('Vendedor:', this.ventas[0].vendedor);
    }
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
    const element = document.getElementById('reporteVentas');
    if (!element) return;

    setTimeout(() => {
      const clone = element.cloneNode(true) as HTMLElement;
      clone.style.width = '794px';
      clone.style.maxWidth = '100%';
      clone.style.overflow = 'hidden';

      document.body.appendChild(clone);

      const tabla = clone.querySelector('.tabla-usuarios') as HTMLTableElement;
      if (tabla) {
        tabla.style.borderCollapse = 'collapse';
        tabla.style.tableLayout = 'auto';
        tabla.style.width = '100%';

        const celdas = tabla.querySelectorAll('th, td');
        celdas.forEach(celda => {
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
        const fileName = `reporte_ventas_${new Date().toISOString().replace(/[:.]/g, '-')}.pdf`;
        pdf.save(fileName);

        document.body.removeChild(clone);
      });
    }, 250);
  }
}

import { Component, OnInit } from '@angular/core';
import { VentaService } from '../../../services/ventas.service';

@Component({
  selector: 'app-historial',
  standalone: false,
  templateUrl: './historial.component.html',
  styleUrl: './historial.component.css'
})
export class HistorialComponent implements OnInit {
    ventas: any[] = [];
    cargando = true;
    error: string = '';
    paginaActual: number = 1;
    elementosPorPagina: number = 10;
    originalVentas: any[] = [];    // Aquí el listado original sin filtrar

    constructor(private ventaService: VentaService) {}

  ngOnInit(): void {
    this.cargarVentas();
}


ventasPaginadas(): any[] {
  return this.ventas.slice(
    (this.paginaActual - 1) * this.elementosPorPagina,
    this.paginaActual * this.elementosPorPagina
  );
}

totalPaginas(): number {
  return Math.ceil(this.ventas.length / this.elementosPorPagina);
}

paginaAnterior() {
  if (this.paginaActual > 1) {
    this.paginaActual--;
  }
}

paginaSiguiente() {
  if (this.paginaActual < this.totalPaginas()) {
    this.paginaActual++;
  }
}


cargarVentas(): void {
    this.cargando = true;
    this.ventaService.obtenerVentas().subscribe({
      next: (data) => {
        this.ventas = data;
        this.cargando = false;
      },
      error: (err) => {
        this.error = err.message;
        this.cargando = false;
      }
    });
  }

  verDetalle(id: number): void {
    this.ventaService.obtenerVentaPorId(id).subscribe({
      next: (venta) => {
        console.log('Detalle de venta:', venta);
        // Aquí puedes abrir un modal o una vista detallada
      },
      error: (err) => {
        console.error('Error al obtener detalle:', err.message);
      }
    });
  }

  verRecibo(id: number): void {
    this.ventaService.obtenerReciboVenta(id).subscribe({
      next: (recibo) => {
        console.log('Recibo de venta:', recibo);
        // Puedes mostrar en modal o generar PDF
      },
      error: (err) => {
        console.error('Error al obtener recibo:', err.message);
      }
    });
  }
}
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import Swal from 'sweetalert2';
import { AperturaCajaService } from '../../../services/AperturaCaja.service';
import { CierreCajaService, CierreCaja, ClasificacionCaja } from '../../../services/CierreCaja.service';

@Component({
  selector: 'app-cierre-caja',
  templateUrl: './cierre-caja.component.html',
  styleUrls: ['./cierre-caja.component.css'],
  standalone: true,
  imports: [FormsModule, CommonModule]
})
export class CierreCajaComponent implements OnInit {

  entradasSalidas: number = 0;
  total: number = 0;
  fechaApertura?: string | null;

  cierre: CierreCaja = {
    nombreCajero: '',
    numeroCaja: '',
    fechaApertura: '',
    fechaCierre: '',
    baseCaja: 0,
    clasificaciones: [
      { denominacion: 'Monedas de Q. 0.25', valor: 0.25, cantidad: 0, subtotal: 0 },
      { denominacion: 'Monedas de Q. 0.50', valor: 0.50, cantidad: 0, subtotal: 0 },
      { denominacion: 'Monedas de Q. 1.00', valor: 1.00, cantidad: 0, subtotal: 0 },
      { denominacion: 'Billetes de Q. 5.00', valor: 5.00, cantidad: 0, subtotal: 0 },
      { denominacion: 'Billetes de Q. 10.00', valor: 10.00, cantidad: 0, subtotal: 0 },
      { denominacion: 'Billetes de Q. 20.00', valor: 20.00, cantidad: 0, subtotal: 0 },
      { denominacion: 'Billetes de Q. 50.00', valor: 50.00, cantidad: 0, subtotal: 0 },
      { denominacion: 'Billetes de Q. 100.00', valor: 100.00, cantidad: 0, subtotal: 0 },
      { denominacion: 'Billetes de Q. 200.00', valor: 200.00, cantidad: 0, subtotal: 0 }
    ],
    saldos: {
      saldoAnterior: 0,
      entradasSalidas: 0,
      subtotal: 0,
      total: 0
    }
  };

  constructor(
    private aperturaService: AperturaCajaService,
    private cierreCajaService: CierreCajaService
  ) { }

  ngOnInit(): void {
    this.calcularTotales();
  }

  actualizarSubtotal(clasificacion: ClasificacionCaja): void {
    clasificacion.subtotal = clasificacion.cantidad * clasificacion.valor;
    this.calcularTotales();
  }

  calcularTotales(): void {
    const subtotal = this.cierre.clasificaciones.reduce((acc, item) => acc + item.subtotal, 0);
    this.entradasSalidas = subtotal;
    this.cierre.saldos.entradasSalidas = subtotal;
    this.cierre.saldos.saldoAnterior = this.cierre.baseCaja;
    this.total = this.entradasSalidas - this.cierre.saldos.saldoAnterior;
    this.cierre.saldos.total = this.total;
  }

  cerrarCaja(): void {
  const fechaCierreUTC = new Date().toISOString();

  const fechaAperturaUTC = this.cierre.fechaApertura
    ? new Date(this.cierre.fechaApertura).toISOString()
    : null;

  this.cierre.saldos.subtotal = this.cierre.saldos.entradasSalidas + this.cierre.saldos.saldoAnterior;

  const cierreParaGuardar: CierreCaja = {
    ...this.cierre,
    fechaApertura: fechaAperturaUTC,
    fechaCierre: fechaCierreUTC
  };

  console.log('Datos a enviar:', cierreParaGuardar);

  this.cierreCajaService.crearCierre(cierreParaGuardar).subscribe({
    next: (res: CierreCaja) => {
      Swal.fire({
        icon: 'success',
        title: 'Cierre guardado',
        text: 'El cierre de caja se ha guardado correctamente.',
        confirmButtonColor: '#3085d6'
      });
    },
    error: (err) => {
      const mensaje = err.error?.message || err.message || 'Error desconocido';
      Swal.fire({
        icon: 'error',
        title: 'Error al guardar',
        text: mensaje,
        confirmButtonColor: '#d33'
      });
    }
  });
}

  exportarPDF(): void {
  const original = document.getElementById('contenido-a-imprimir');
  if (!original) {
    console.error('No se encontró el contenido para imprimir');
    return;
  }

  // Clonamos el contenido para no modificar el DOM real
  const clone = original.cloneNode(true) as HTMLElement;

  // Reemplazamos todos los <input> por <span> con su valor
  const inputs = clone.querySelectorAll('input');
  inputs.forEach(input => {
    const span = document.createElement('span');
    span.textContent = input.value || ''; // toma el valor del input
    input.parentNode?.replaceChild(span, input);
  });

  // Abrimos ventana para imprimir
  const ventana = window.open('', '', 'width=800,height=600');
  if (ventana) {
    ventana.document.write(`
      <html>
        <head>
          <title>Recibo de Cierre de Caja</title>
          <style>
            body {
              font-family: Arial, sans-serif;
              padding: 20px;
              color: #000;
            }
            h1, h2 {
              text-align: center;
              color: #700000;
            }
            table {
              width: 100%;
              border-collapse: collapse;
              margin-top: 10px;
            }
            th, td {
              border: 1px solid #333;
              padding: 8px;
              text-align: left;
            }
            th {
              background-color: #8B0000;
              color: white;
            }
            .section-title {
              background-color: #64839F;
              color: white;
              padding: 4px;
              margin-top: 20px;
              font-weight: bold;
            }
            .text-right {
              text-align: right;
            }
            .font-bold {
              font-weight: bold;
            }
          </style>
        </head>
        <body>
          ${clone.innerHTML}
        </body>
      </html>
    `);
    ventana.document.close();
    ventana.focus();
    ventana.print();
    ventana.close();
  }
}

}


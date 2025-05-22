import { Component, OnInit } from '@angular/core';
import { AperturaCajaService, AperturaCaja } from '../../../services/AperturaCaja.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import Swal from 'sweetalert2';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-apertura-caja',
  templateUrl: './apertura-caja.component.html',
  styleUrls: ['./apertura-caja.component.css'],
  imports: [FormsModule, CommonModule]
})
export class AperturaCajaComponent implements OnInit {
  fechaActual: string = '';    // ISO para backend
  fechaMostrar: string = '';   // dd/mm/yyyy para mostrar
  monto: number = 0;

  ultimaApertura: AperturaCaja | null = null;

  constructor(private aperturaCajaService: AperturaCajaService) {}

  ngOnInit(): void {
    const guardada = localStorage.getItem('ultimaApertura');
    if (guardada) {
      this.ultimaApertura = JSON.parse(guardada);
    }

    const hoy = new Date();
    this.fechaActual = hoy.toISOString();

    const dia = String(hoy.getDate()).padStart(2, '0');
    const mes = String(hoy.getMonth() + 1).padStart(2, '0');
    const anio = hoy.getFullYear();
    this.fechaMostrar = `${dia}/${mes}/${anio}`;
  }

  mensaje: string | null = null;
tipoMensaje: 'exito' | 'error' | null = null;

guardarApertura(): void {
  if (this.monto <= 0) {
    Swal.fire({
      icon: 'warning',
      title: 'Monto inválido',
      text: 'Ingrese un monto válido.'
    });
    return;
  }

  const apertura: AperturaCaja = {
    fecha: this.fechaActual,
    monto: this.monto
  };

  this.aperturaCajaService.agregarApertura(apertura).subscribe({
    next: () => {
      this.ultimaApertura = apertura;
      localStorage.setItem('ultimaApertura', JSON.stringify(apertura));
      this.monto = 0;

      Swal.fire({
        icon: 'success',
        title: 'Éxito',
        text: 'Apertura guardada con éxito.'
      });
    },
    error: (error) => {
      console.error('Error al guardar apertura:', error);
      Swal.fire({
        icon: 'error',
        title: 'Error',
        text: 'Error al guardar apertura. Intente nuevamente.'
      });
    }
  });
}


}

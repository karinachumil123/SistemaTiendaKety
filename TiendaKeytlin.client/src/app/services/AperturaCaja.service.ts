import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface AperturaCaja {
  id?: number;
  fecha: string; // por ejemplo: '2025-05-21T00:00:00Z' (ISO 8601)
  monto: number;
}

@Injectable({
  providedIn: 'root'
})
export class AperturaCajaService {
  private readonly apiUrl = 'http://localhost:5010/api/aperturas'; // CORRECTO

  constructor(private http: HttpClient) {}

  agregarApertura(apertura: AperturaCaja): Observable<any> {
    // Aquí puedes hacer formateo de la fecha si es necesario, o enviarla tal cual
    return this.http.post(this.apiUrl, apertura);
  }
  obtenerPorFecha(fecha: string): Observable<AperturaCaja | null> {
  return this.http.get<AperturaCaja>(`${this.apiUrl}/aperturas/fecha/${fecha}`);
}

}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface AperturaCaja {
  id?: number;
  fecha: string; // ISO 8601 format
  monto: number;
}

@Injectable({
  providedIn: 'root'
})
export class AperturaCajaService {
  private readonly apiUrl = 'http://localhost:5010/api/aperturas';

  constructor(private http: HttpClient) {}

  agregarApertura(apertura: AperturaCaja): Observable<any> {
    return this.http.post(this.apiUrl, apertura);
  }

  // CORREGIDO: URL correcta para obtener por fecha
  obtenerPorFecha(fecha: string): Observable<AperturaCaja> {
    return this.http.get<AperturaCaja>(`${this.apiUrl}/por-fecha/${fecha}`);
  }

  // Método adicional para obtener la última apertura
  obtenerUltimaApertura(): Observable<AperturaCaja[]> {
    return this.http.get<AperturaCaja[]>(this.apiUrl);
  }
}
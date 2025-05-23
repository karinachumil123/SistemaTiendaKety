import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ClasificacionCaja {
  denominacion: string;
  cantidad: number;
  subtotal: number;
  valor: number;  
}

export interface SaldosCaja {
  saldoAnterior: number;
  entradasSalidas: number;
  subtotal: number;
  total: number;
}

export interface CierreCaja {
  id?: number;
  nombreCajero: string;
  numeroCaja: string;
  fechaApertura: string | null;
  fechaCierre: string | null;
  baseCaja: number;
  clasificaciones: ClasificacionCaja[];
  saldos: SaldosCaja;
}

@Injectable({
  providedIn: 'root'
})
export class CierreCajaService {
  // URL base correcta según el controlador CierreCajaController
  private readonly apiUrl = 'http://localhost:5010/api/cierrecaja';

  constructor(private http: HttpClient) {}

  // POST correcto sin ruta adicional
  crearCierre(cierre: CierreCaja): Observable<CierreCaja> {
    return this.http.post<CierreCaja>(this.apiUrl, cierre);
  }

  obtenerCierres(): Observable<CierreCaja[]> {
    return this.http.get<CierreCaja[]>(this.apiUrl);
  }

  obtenerCierrePorId(id: number): Observable<CierreCaja> {
    return this.http.get<CierreCaja>(`${this.apiUrl}/${id}`);
  }

  eliminarCierre(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}

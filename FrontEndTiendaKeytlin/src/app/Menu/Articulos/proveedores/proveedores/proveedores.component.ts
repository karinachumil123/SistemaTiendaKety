import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProveedoresModalComponent } from '../proveedores-modal/proveedores-modal.component';
import { ProveedorService, Proveedor } from '../../../../services/proveedor.service';
import { AuthService } from '../../../../services/auth.service';

@Component({
  selector: 'app-proveedores',
  standalone: true,
  templateUrl: './proveedores.component.html',
  styleUrls: ['./proveedores.component.css'],
  imports: [CommonModule, FormsModule, ProveedoresModalComponent]
})
export class ProveedoresComponent implements OnInit {
  proveedores: Proveedor[] = [];
  proveedoresFiltrados: Proveedor[] = [];
  filtroProveedor: string = '';
  filtroEstado: string = 'todos'; // <-- Nuevo filtro por estado
  mostrarModal: boolean = false;
  proveedorSeleccionado: Proveedor | null = null;
  modoVer: boolean = false;

  // Paginación
  paginaActual: number = 1;
  registrosPorPagina: number = 10;

  // Variables de Permisos
  puedeCrear: boolean = false;
  puedeEditar: boolean = false;
  puedeEliminar: boolean = false;
  puedeVer: boolean = false;

  constructor(private proveedorService: ProveedorService,
              private authService: AuthService) { }

  ngOnInit() {
    this.obtenerProveedores();
    this.cargarPermisos();
  }

  obtenerProveedores() {
    this.proveedorService.getProveedores().subscribe({
      next: (data) => {
        this.proveedores = data;
        this.proveedoresFiltrados = [...data];
      },
      error: (error) => console.error('Error al obtener proveedores:', error)
    });
  }

  proveedorEstado(estado: string | boolean | undefined): string {
  if (estado === true) return 'Activo';
  if (estado === false) return 'Inactivo';
  if (typeof estado === 'string') return estado;
  return 'Desconocido'; // o el texto que prefieras para estados nulos o indefinidos
}

  cargarPermisos() {
    const permisos = this.authService.obtenerPermisos();
  
    this.puedeCrear = permisos.includes('Crear Proveedores');
    this.puedeEditar = permisos.includes('Editar Proveedores');
    this.puedeEliminar = permisos.includes('Eliminar Proveedores');
    this.puedeVer = permisos.includes('Ver Proveedores');
  }

 
  filtrarProveedores() {
  let filtrados = [...this.proveedores];

  // Filtro por texto (nombre, contacto, teléfono, id)
  if (this.filtroProveedor.trim() !== '') {
    const filtro = this.filtroProveedor.toLowerCase();
    filtrados = filtrados.filter(p =>
      (p.nombre?.toLowerCase().includes(filtro)) ||
      (p.nombreContacto?.toLowerCase().includes(filtro)) ||
      (p.telefono?.includes(filtro)) ||
      (p.id !== undefined && p.id !== null && p.id.toString().includes(filtro))
    );
  }

  // Filtro por estado
  if (this.filtroEstado === 'activos') {
    filtrados = filtrados.filter(p => p.estado === 'Activo');
  } else if (this.filtroEstado === 'inactivos') {
    filtrados = filtrados.filter(p => p.estado === 'Inactivo');
  }

  this.proveedoresFiltrados = filtrados;
  this.paginaActual = 1;
}


  abrirModal(proveedor?: Proveedor, modoVer: boolean = false) {
    this.proveedorSeleccionado = proveedor ? { ...proveedor } : null;
    this.modoVer = modoVer;
    this.mostrarModal = true;
  }

  cerrarModal() {
    this.mostrarModal = false;
    this.proveedorSeleccionado = null;
    this.modoVer = false;
  }

  manejarProveedorGuardado(proveedor: Proveedor) {
    if (proveedor.id) {
      this.proveedorService.updateProveedor(proveedor.id, proveedor).subscribe({
        next: (updatedProveedor) => {
          const index = this.proveedores.findIndex(p => p.id === updatedProveedor.id);
          if (index !== -1) {
            this.proveedores[index] = updatedProveedor;
          }
          this.filtrarProveedores();  // Aplicar filtro actualizado
          this.cerrarModal();
        },
        error: (error) => console.error('Error al actualizar proveedor:', error)
      });
    } else {
      this.proveedorService.addProveedor(proveedor).subscribe({
        next: (newProveedor) => {
          this.proveedores.push(newProveedor);
          this.filtrarProveedores(); // Aplicar filtro actualizado
          this.cerrarModal();
        },
        error: (error) => console.error('Error al agregar proveedor:', error)
      });
    }
  }

  eliminarProveedor(id: number | undefined) {
    if (!id) return;

    if (confirm('¿Estás seguro de eliminar este proveedor?')) {
      this.proveedorService.deleteProveedor(id).subscribe({
        next: () => {
          this.proveedores = this.proveedores.filter(p => p.id !== id);
          this.filtrarProveedores();
        },
        error: (error) => console.error('Error al eliminar proveedor:', error)
      });
    }
  }

  proveedoresPaginados() {
    const inicio = (this.paginaActual - 1) * this.registrosPorPagina;
    return this.proveedoresFiltrados.slice(inicio, inicio + this.registrosPorPagina);
  }

  totalPaginas() {
    return Math.ceil(this.proveedoresFiltrados.length / this.registrosPorPagina);
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

  getPaginas(): number[] {
    const total = this.totalPaginas();
    const paginas = [];

    const inicio = Math.max(1, this.paginaActual - 1);
    const fin = Math.min(total, this.paginaActual + 1);

    for (let i = inicio; i <= fin; i++) {
      paginas.push(i);
    }

    return paginas;
  }

  irAPagina(pagina: number) {
    this.paginaActual = pagina;
  }
}


import { Component, Input, Output, EventEmitter, ViewChild, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { Proveedor } from '../../../../services/proveedor.service';

@Component({
  selector: 'app-proveedores-modal',
  standalone: true,
  templateUrl: './proveedores-modal.component.html',
  styleUrls: ['./proveedores-modal.component.css'],
  imports: [CommonModule, FormsModule]
})
export class ProveedoresModalComponent implements OnInit {
  @Input() proveedor: Proveedor | null = null;
  @Input() modoVer: boolean = false;

  @Output() cerrarModal = new EventEmitter<void>();
  @Output() guardarProveedor = new EventEmitter<Proveedor>();
  @Output() actualizarProveedor = new EventEmitter<{ id: number; proveedor: Proveedor }>();
  @Output() eliminarProveedor = new EventEmitter<number>();

  @ViewChild('proveedor') proveedorForm1!: NgForm;

  // Inicializamos con valores vacíos o por defecto
  proveedorForm: Proveedor = {
    nombre: '',
    nombreContacto: '',
    telefono: '',
    telefonoContacto: '',
    correo: '',
    direccion: '',
    estado: 'Activo',
    descripcion: ''
  };

  cargando = false;

  ngOnInit() {
    // Si recibimos un proveedor por input, clonamos sus valores para editar/ver
    if (this.proveedor) {
      this.proveedorForm = { ...this.proveedor };
    }
  }

  guardar() {
    if (this.modoVer) {
      return; // No hacer nada si solo estamos viendo
    }

    // Validación básica para campos obligatorios
    if (!this.proveedorForm.nombre || !this.proveedorForm.nombreContacto || !this.proveedorForm.telefono) {
      alert('Los campos marcados con * son obligatorios');
      return;
    }

    // Si proveedor con id, emitimos actualización, si no, emitimos creación
    if (this.proveedor && this.proveedor.id !== undefined) {
      this.actualizarProveedor.emit({ id: this.proveedor.id, proveedor: this.proveedorForm });
    } else {
      this.guardarProveedor.emit(this.proveedorForm);
    }
  }

  eliminar() {
    // Solo eliminar si proveedor existe y tiene id definido
    if (this.proveedor && this.proveedor.id !== undefined) {
      const confirmar = confirm('¿Estás seguro de eliminar este proveedor?');
      if (confirmar) {
        this.eliminarProveedor.emit(this.proveedor.id);
      }
    }
  }
}

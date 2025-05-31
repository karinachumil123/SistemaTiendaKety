

import { Component, EventEmitter, Inject, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule, DOCUMENT } from '@angular/common';
import { ProductoService } from '../../../../services/productos.service';
import { Categoria, CategoriaService } from '../../../../services/categoria.service';
import { Proveedor, ProveedorService } from '../../../../services/proveedor.service';
import Swal from 'sweetalert2';

// Interfaces para tipado fuerte
interface Producto {
  id?: number;
  nombre?: string;
  descripcion?: string;
  codigoProducto?: string; // ← cambiar de CodigoProducto
  MarcaProducto?: string;  // ← cambiar de MarcaProducto
  precioAdquisicion: number | null;
  precioVenta: number | null;
  proveedorId?: number;
  categoriaId?: number;
  estadoId?: number;
  imagen?: string;
  categoria?: any;
  proveedor?: any;
}


@Component({
  selector: 'app-productos-modal',
  standalone: true,
  templateUrl: './productos-modal.component.html',
  styleUrl: './productos-modal.component.css',
  imports: [FormsModule, CommonModule]
})

export class ProductosModalComponent implements OnInit {
  @Input() producto: Producto = {
    precioAdquisicion: null,
    precioVenta: null
  };
  @Input() modoVer: boolean = false;
  @Output() cerrarModal = new EventEmitter<void>();
  @Output() guardar = new EventEmitter<Producto>();
  @ViewChild('productoForm') productoForm!: NgForm;


  imagenPrevia: string | ArrayBuffer | null = null;
  archivoImagen: File | null = null;
  cargando = false;
  categorias: Categoria[] = [];
  proveedores: Proveedor[] = [];
  estados: any[] = [];

  constructor(
    private productoService: ProductoService,
    private categoriaService: CategoriaService,
    private proveedorService: ProveedorService,
    @Inject(DOCUMENT) private document: Document
  ) {}
  

  ngOnInit(): void {
  
    this.cargarDatos(); 
  
    if (this.producto.imagen) {
      this.imagenPrevia = this.getImagenUrl(this.producto.imagen);
    }
    if (this.modoVer) {
    setTimeout(() => {
      const form = this.productoForm;
      if (form) {
        Object.keys(form.controls).forEach(key => {
          form.controls[key].disable();
        });
      }
    });
  }
  }

  getTitulo(): string {
  if (this.modoVer) return 'Ver Producto';
  return (this.producto.id && this.producto.id > 0) ? 'Editar Producto' : 'Agregar Producto';
}
 
  // En el modal, modifica la carga de datos:
cargarDatos(): void {
  this.productoService.obtenerCategorias().subscribe({
    next: (categorias) => {
      this.categorias = categorias;
      // Si no hay categoría seleccionada, asignar una por defecto
      if (!this.producto.categoriaId && categorias.length > 0) {
        this.producto.categoriaId = categorias.find(c => c.categoriaNombre?.toLowerCase() === 'general')?.id || categorias[0].id;
      }
    }
  });

  this.productoService.obtenerProveedores().subscribe({
    next: (proveedores) => {
      this.proveedores = proveedores;
      if (!this.producto.proveedorId && proveedores.length > 0) {
        this.producto.proveedorId = proveedores[0].id;
      }
    }
  });

  this.productoService.obtenerEstados().subscribe({
    next: (estados) => {
      this.estados = estados;
      // Si no hay estado seleccionado, asignar 'Activo' por defecto
      if (!this.producto.estadoId && estados.length > 0) {
        const estadoActivo = estados.find(e => e.nombre?.toLowerCase() === 'activo');
        this.producto.estadoId = estadoActivo ? estadoActivo.id : estados[0].id;
      }
    }
  });
}
  

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    const file = input.files[0];

    if (!file.type.match('image/jpeg') && !file.type.match('image/png')) {
      alert('Solo se permiten archivos JPG o PNG');
      return;
    }

    if (file.size > 2 * 1024 * 1024) {
      alert('La imagen no debe superar los 2MB');
      return;
    }

    this.archivoImagen = file;
    this.producto.imagen = file.name;

    const reader = new FileReader();
    reader.onload = () => {
      this.imagenPrevia = reader.result;
    };
    reader.readAsDataURL(file);
  }

  onFileInputClick(): void {
    this.document.getElementById('fileInput')?.click();
  }

  getImagenUrl(nombreImagen: string): string {
    return this.productoService.getImagenUrl(nombreImagen);
  }

    convertirArchivoABase64(file: File): Promise<string | undefined> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
  
      reader.onload = () => {
        const resultado = reader.result;
        if (typeof resultado === 'string') {
          resolve(resultado);
        } else {
          resolve(undefined); // Si no es string, devolvemos undefined
        }
      };
  
      reader.onerror = error => {
        reject(error);
      };
  
      reader.readAsDataURL(file);
    });
  }

async onSubmit(): Promise<void> {

  if (this.modoVer) {
      return; // No hacer nada si solo estamos viendo
    }
  if (this.cargando) return;

  if (this.productoForm.invalid || !this.validarFormulario()) {
    this.marcarCamposComoTocados();
    return;
  }

  this.cargando = true;

  try {
    let base64: string | undefined = undefined;
  
      if (this.archivoImagen) {
        base64 = await this.convertirArchivoABase64(this.archivoImagen);
      }

    const productoCompleto: Producto = {
      ...this.producto,
      categoria: this.categorias.find(c => c.id === this.producto.categoriaId),
      proveedor: this.proveedores.find(p => p.id === this.producto.proveedorId),
      imagen: base64 ?? this.producto.imagen
    };

    this.guardar.emit(productoCompleto);

  } catch (error: any) {
    console.error('Error al guardar producto:', error);

    if (error.status === 400 && error.error?.errors) {
      const errores = Object.values(error.error.errors).flat().join('\n');
      Swal.fire({
        icon: 'error',
        title: 'Errores de validación',
        text: errores
      });
    } else {
      Swal.fire({
        icon: 'error',
        title: 'Error',
        text: 'Ocurrió un error al guardar el producto. Intente nuevamente.'
      });
    }

  } finally {
    this.cargando = false;
  }
}


private validarFormulario(): boolean {
  const camposRequeridos = [
    { campo: this.producto.nombre?.trim(), mensaje: 'El nombre del producto es requerido' },
    { campo: this.producto.codigoProducto, mensaje: 'El código del producto es requerido' },
    { campo: this.producto.precioAdquisicion, mensaje: 'El precio de adquisición es requerido' },
    { campo: this.producto.precioVenta, mensaje: 'El precio de venta es requerido' },
    { campo: this.producto.categoriaId, mensaje: 'La categoría es requerida' },
    { campo: this.producto.proveedorId, mensaje: 'El proveedor es requerido' }
  ];

  for (const { campo, mensaje } of camposRequeridos) {
    if (campo === null || campo === undefined || campo === '' || (typeof campo === 'number' && isNaN(campo))) {
      alert(mensaje);
      return false; // ← Retorna `false` si algún campo es inválido
    }
  }

  return true; // ← Retorna `true` si todos los campos son válidos
}


  private marcarCamposComoTocados(): void {
    if (!this.productoForm?.controls) return;
    Object.values(this.productoForm.controls).forEach(control => {
      control.markAsTouched();
    });
  }

}


import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { StatusRegistro, Usuario } from '../../core/models/api.models';
import { mensagemErro } from '../../shared/http-error';

@Component({
  selector: 'app-usuarios',
  imports: [ReactiveFormsModule, FormsModule],
  templateUrl: './usuarios.component.html'
})
export class UsuariosComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly fb = inject(FormBuilder);

  usuarios: Usuario[] = [];
  loading = false;
  error = '';
  filtro: StatusRegistro | '' = '';
  modalAberto = false;
  modo: 'criar' | 'editar' = 'criar';
  selecionado?: Usuario;

  form = this.fb.nonNullable.group({
    codigo: ['', Validators.required],
    login: ['', [Validators.required, Validators.minLength(3)]],
    senha: [''],
    status: this.fb.nonNullable.control<StatusRegistro>('Ativo', Validators.required)
  });

  ngOnInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.loading = true;
    this.error = '';
    this.api.listarUsuarios(this.filtro).subscribe({
      next: (data) => {
        this.usuarios = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = mensagemErro(err);
        this.loading = false;
      }
    });
  }

  abrirCriacao(): void {
    this.modo = 'criar';
    this.selecionado = undefined;
    this.form.reset({ codigo: '', login: '', senha: '', status: 'Ativo' });
    this.form.controls.codigo.enable();
    this.form.controls.login.enable();
    this.form.controls.senha.setValidators([Validators.required, Validators.minLength(6)]);
    this.form.controls.senha.updateValueAndValidity();
    this.modalAberto = true;
  }

  abrirEdicao(usuario: Usuario): void {
    this.modo = 'editar';
    this.selecionado = usuario;
    this.form.reset({ codigo: usuario.codigo, login: usuario.login, senha: '', status: usuario.status });
    this.form.controls.codigo.disable();
    this.form.controls.login.disable();
    this.form.controls.senha.clearValidators();
    this.form.controls.senha.setValidators([Validators.minLength(6)]);
    this.form.controls.senha.updateValueAndValidity();
    this.modalAberto = true;
  }

  salvar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    this.loading = true;
    this.error = '';

    const request$ = this.modo === 'criar'
      ? this.api.criarUsuario({ codigo: raw.codigo, login: raw.login, senha: raw.senha, status: raw.status })
      : this.api.atualizarUsuario(this.selecionado!.codigo, {
          status: raw.status,
          senha: raw.senha || undefined
        });

    request$.subscribe({
      next: () => {
        this.modalAberto = false;
        this.carregar();
      },
      error: (err) => {
        this.error = mensagemErro(err);
        this.loading = false;
      }
    });
  }
}

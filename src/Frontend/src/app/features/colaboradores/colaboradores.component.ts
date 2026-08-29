import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { Colaborador, Unidade, Usuario } from '../../core/models/api.models';
import { mensagemErro } from '../../shared/http-error';

@Component({
  selector: 'app-colaboradores',
  imports: [ReactiveFormsModule],
  templateUrl: './colaboradores.component.html'
})
export class ColaboradoresComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly fb = inject(FormBuilder);

  colaboradores: Colaborador[] = [];
  unidades: Unidade[] = [];
  usuarios: Usuario[] = [];
  loading = false;
  error = '';
  modalAberto = false;
  confirmarExclusao?: Colaborador;
  modo: 'criar' | 'editar' = 'criar';
  selecionado?: Colaborador;

  form = this.fb.nonNullable.group({
    codigo: ['', Validators.required],
    nome: ['', Validators.required],
    unidadeId: ['', Validators.required],
    usuarioId: ['', Validators.required]
  });

  ngOnInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.loading = true;
    this.error = '';
    this.api.listarColaboradores().subscribe({
      next: (data) => {
        this.colaboradores = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = mensagemErro(err);
        this.loading = false;
      }
    });
    this.api.listarUnidades().subscribe({ next: (data) => this.unidades = data });
    this.api.listarUsuarios('Ativo').subscribe({ next: (data) => this.usuarios = data });
  }

  get unidadesParaSelect(): Unidade[] {
    const ativas = this.unidades.filter(u => u.status === 'Ativo');
    if (this.selecionado) {
      const atual = this.unidades.find(u => u.id === this.selecionado!.unidadeId);
      if (atual && !ativas.some(a => a.id === atual.id)) {
        return [atual, ...ativas];
      }
    }
    return ativas;
  }

  usuariosDisponiveis(): Usuario[] {
    const usados = new Set(this.colaboradores.map(c => c.usuarioId));
    if (this.modo === 'editar' && this.selecionado) {
      usados.delete(this.selecionado.usuarioId);
    }
    return this.usuarios.filter(u => !usados.has(u.id));
  }

  abrirCriacao(): void {
    this.modo = 'criar';
    this.selecionado = undefined;
    this.form.reset({ codigo: '', nome: '', unidadeId: '', usuarioId: '' });
    this.form.controls.codigo.enable();
    this.form.controls.usuarioId.enable();
    this.modalAberto = true;
  }

  abrirEdicao(colaborador: Colaborador): void {
    this.modo = 'editar';
    this.selecionado = colaborador;
    this.form.reset({
      codigo: colaborador.codigo,
      nome: colaborador.nome,
      unidadeId: colaborador.unidadeId,
      usuarioId: colaborador.usuarioId
    });
    this.form.controls.codigo.disable();
    this.form.controls.usuarioId.disable();
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
      ? this.api.criarColaborador(raw)
      : this.api.atualizarColaborador(this.selecionado!.codigo, { nome: raw.nome, unidadeId: raw.unidadeId });

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

  excluir(): void {
    if (!this.confirmarExclusao) {
      return;
    }

    this.api.removerColaborador(this.confirmarExclusao.codigo).subscribe({
      next: () => {
        this.confirmarExclusao = undefined;
        this.carregar();
      },
      error: (err) => {
        this.error = mensagemErro(err);
        this.confirmarExclusao = undefined;
      }
    });
  }
}

import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { StatusRegistro, Unidade, UnidadeDetalhe } from '../../core/models/api.models';
import { mensagemErro } from '../../shared/http-error';

@Component({
  selector: 'app-unidades',
  imports: [ReactiveFormsModule],
  templateUrl: './unidades.component.html'
})
export class UnidadesComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly fb = inject(FormBuilder);

  unidades: Unidade[] = [];
  detalhe?: UnidadeDetalhe;
  loading = false;
  error = '';
  modalAberto = false;
  modo: 'criar' | 'editar' = 'criar';
  selecionada?: Unidade;

  form = this.fb.nonNullable.group({
    codigo: ['', Validators.required],
    nome: ['', Validators.required],
    status: this.fb.nonNullable.control<StatusRegistro>('Ativo', Validators.required)
  });

  ngOnInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.loading = true;
    this.error = '';
    this.api.listarUnidades().subscribe({
      next: (data) => {
        this.unidades = data;
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
    this.selecionada = undefined;
    this.form.reset({ codigo: '', nome: '', status: 'Ativo' });
    this.form.controls.codigo.enable();
    this.modalAberto = true;
  }

  abrirEdicao(unidade: Unidade): void {
    this.modo = 'editar';
    this.selecionada = unidade;
    this.form.reset({ codigo: unidade.codigo, nome: unidade.nome, status: unidade.status });
    this.form.controls.codigo.disable();
    this.modalAberto = true;
  }

  verColaboradores(unidade: Unidade): void {
    this.api.obterUnidade(unidade.id).subscribe({
      next: (data) => this.detalhe = data,
      error: (err) => this.error = mensagemErro(err)
    });
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
      ? this.api.criarUnidade(raw)
      : this.api.atualizarUnidade(this.selecionada!.id, { nome: raw.nome, status: raw.status });

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

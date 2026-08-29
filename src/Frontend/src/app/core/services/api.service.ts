import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Colaborador, StatusRegistro, Unidade, UnidadeDetalhe, Usuario } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  constructor(private http: HttpClient) {}

  listarUsuarios(status?: StatusRegistro | '') {
    let params = new HttpParams();
    if (status) {
      params = params.set('status', status);
    }
    return this.http.get<Usuario[]>('/api/usuarios', { params });
  }

  obterUsuario(codigo: string) {
    return this.http.get<Usuario>(`/api/usuarios/${codigo}`);
  }

  criarUsuario(payload: { codigo: string; login: string; senha: string; status: StatusRegistro }) {
    return this.http.post<Usuario>('/api/usuarios', payload);
  }

  atualizarUsuario(codigo: string, payload: { senha?: string; status: StatusRegistro }) {
    return this.http.put<Usuario>(`/api/usuarios/${codigo}`, payload);
  }

  listarColaboradores() {
    return this.http.get<Colaborador[]>('/api/colaboradores');
  }

  obterColaborador(codigo: string) {
    return this.http.get<Colaborador>(`/api/colaboradores/${codigo}`);
  }

  criarColaborador(payload: { codigo: string; nome: string; unidadeId: string; usuarioId: string }) {
    return this.http.post<Colaborador>('/api/colaboradores', payload);
  }

  atualizarColaborador(codigo: string, payload: { nome: string; unidadeId: string }) {
    return this.http.put<Colaborador>(`/api/colaboradores/${codigo}`, payload);
  }

  removerColaborador(codigo: string) {
    return this.http.delete(`/api/colaboradores/${codigo}`);
  }

  listarUnidades() {
    return this.http.get<Unidade[]>('/api/unidades');
  }

  obterUnidade(id: string) {
    return this.http.get<UnidadeDetalhe>(`/api/unidades/${id}`);
  }

  criarUnidade(payload: { codigo: string; nome: string; status: StatusRegistro }) {
    return this.http.post<Unidade>('/api/unidades', payload);
  }

  atualizarUnidade(id: string, payload: { nome: string; status: StatusRegistro }) {
    return this.http.put<Unidade>(`/api/unidades/${id}`, payload);
  }
}

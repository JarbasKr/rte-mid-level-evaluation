export type StatusRegistro = 'Ativo' | 'Inativo';

export interface LoginRequest {
  login: string;
  senha: string;
}

export interface AuthUser {
  id: string;
  name: string;
  email: string;
}

export interface LoginResponse {
  token: string;
  tipo: string;
  expiration: string;
  expiraEm: string;
  login: string;
  user: AuthUser;
}

export interface Usuario {
  id: string;
  codigo: string;
  login: string;
  status: StatusRegistro;
}

export interface Unidade {
  id: string;
  codigo: string;
  nome: string;
  status: StatusRegistro;
  quantidadeColaboradores: number;
}

export interface Colaborador {
  id: string;
  codigo: string;
  nome: string;
  unidadeId: string;
  unidadeCodigo: string;
  unidadeNome: string;
  usuarioId: string;
  usuarioLogin: string;
}

export interface UnidadeDetalhe extends Unidade {
  colaboradores: Colaborador[];
}

export interface ApiError {
  message: string;
  statusCode: number;
  errors?: Record<string, string[]>;
}

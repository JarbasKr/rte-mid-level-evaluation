import { HttpErrorResponse } from '@angular/common/http';
import { ApiError } from '../core/models/api.models';

export function mensagemErro(error: unknown): string {
  if (error instanceof HttpErrorResponse) {
    const body = error.error as ApiError | string | null;
    if (body && typeof body === 'object' && body.message) {
      return body.message;
    }
    if (error.status === 0) {
      return 'Não foi possível conectar à API. Verifique se o backend está em execução.';
    }
  }
  return 'Não foi possível concluir a operação.';
}

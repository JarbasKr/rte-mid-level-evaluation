# RTE — Gestão de Colaboradores

Portal corporativo para cadastro de **usuários**, **colaboradores** e **unidades**, com autenticação JWT, API REST em ASP.NET Core e front-end Angular.

```
Angular  →  HTTP + JWT  →  .NET Web API  →  EF Core  →  PostgreSQL (Docker)
```

## Requisitos

- Docker e Docker Compose
- .NET SDK 8 (apenas se for executar a API fora do container)
- Node.js 22+ e Angular CLI (apenas se for executar o front-end com `ng serve`)
- Postman (opcional)

## Credenciais iniciais

| Login | Senha | Observação |
| --- | --- | --- |
| `admin` | `Admin@123` | Usuário seed para o portal |
| `msilva` | `Colab@123` | Usuário associado à colaboradora Maria Silva |

A senha é persistida apenas como hash (BCrypt). Não há senha em texto puro no banco.

## Inicialização com Docker (recomendado)

Na raiz do repositório:

```bash
docker compose up -d --build
```

Serviços:

| Serviço | URL |
| --- | --- |
| Portal Angular | http://localhost:4200 |
| API | http://localhost:5000 |
| Swagger | http://localhost:5000/swagger |
| Health | http://localhost:5000/health |
| PostgreSQL | localhost:5432 |

O backend aplica as migrations e o seed automaticamente na subida.

Para parar:

```bash
docker compose down
```

O volume `rte_pgdata` preserva os dados. Para zerar o banco:

```bash
docker compose down -v
```

## Inicialização local (API + Angular, banco no Docker)

1. Suba somente o PostgreSQL:

```bash
docker compose up -d postgres
```

2. API:

```bash
cd src/Backend
dotnet restore
dotnet ef database update
dotnet run
```

A API escuta em `http://localhost:5000`. Em Development, `Jwt:Secret` vem de `appsettings.Development.json`. Em outros ambientes, defina `Jwt__Secret` (mínimo 32 caracteres).

3. Angular:

```bash
cd src/Frontend
npm install
npm start
```

O `ng serve` usa `proxy.conf.json` para encaminhar `/api` para `http://localhost:5000`.

## Autenticação

```http
POST /api/auth/login
Content-Type: application/json

{
  "login": "admin",
  "senha": "Admin@123"
}
```

Resposta:

```json
{
  "token": "...",
  "tipo": "Bearer",
  "expiration": "2026-08-29T20:00:00Z",
  "expiraEm": "2026-08-29T20:00:00Z",
  "login": "admin",
  "user": {
    "id": "...",
    "name": "admin",
    "email": "admin"
  }
}
```

O domínio atual identifica o usuário pelo **login** (não há e-mail cadastrado). O campo `email` do payload de login replica o login para manter o contrato esperado pelos clientes.

Endpoints protegidos exigem:

```http
Authorization: Bearer {token}
```

O Angular inclui o token automaticamente via interceptor. O Swagger aceita o mesmo esquema em **Authorize**.

## Endpoints

URL base: `http://localhost:5000`

### Auth

| Método | Rota | Auth |
| --- | --- | --- |
| POST | `/api/auth/login` | Não |

### Usuários

| Método | Rota | Auth |
| --- | --- | --- |
| GET | `/api/usuarios?status=Ativo\|Inativo` | Sim |
| GET | `/api/usuarios/{codigo}` | Sim |
| POST | `/api/usuarios` | Sim |
| PUT | `/api/usuarios/{codigo}` | Sim |

Body de criação:

```json
{
  "codigo": "USR002",
  "login": "jferreira",
  "senha": "Senha@123",
  "status": "Ativo"
}
```

### Unidades

| Método | Rota | Auth |
| --- | --- | --- |
| GET | `/api/unidades` | Sim |
| GET | `/api/unidades/{id}` | Sim |
| POST | `/api/unidades` | Sim |
| PUT | `/api/unidades/{id}` | Sim |

### Colaboradores

| Método | Rota | Auth |
| --- | --- | --- |
| GET | `/api/colaboradores` | Sim |
| GET | `/api/colaboradores/{codigo}` | Sim |
| POST | `/api/colaboradores` | Sim |
| PUT | `/api/colaboradores/{codigo}` | Sim |
| DELETE | `/api/colaboradores/{codigo}` | Sim |

Regras principais:

- Código de usuário, unidade e colaborador é único.
- Colaborador exige unidade **ativa** e usuário **ativo**.
- Um usuário só pode estar associado a um colaborador.
- Unidade inativa não recebe novos colaboradores.

Erros de negócio retornam JSON no formato:

```json
{
  "message": "Não foi possível realizar a operação.",
  "statusCode": 422,
  "errors": {}
}
```

## Postman

Importe `postman/RTE-Gestao.postman_collection.json`.

1. Execute **Authentication → Login** (grava `token` e `userId`).
2. Execute **Unidades → Listar** (grava `unidadeId`).
3. Use os demais requests. Endpoints protegidos enviam `Authorization: Bearer {{token}}`.

## Testes automatizados

```bash
dotnet test tests/Rte.Api.Tests/Rte.Api.Tests.csproj
```

## Arquitetura

- **Front-end:** Angular 19 (rotas lazy, guards, interceptor JWT, Reactive Forms).
- **Back-end:** ASP.NET Core Web API, controllers → services → repositório genérico / EF Core.
- **Herança:** `EntityBase` (identidade e auditoria) e `StatusEntity` (ciclo ativo/inativo de usuário e unidade). Colaborador não herda status porque o domínio não prevê inativação desse agregado.
- **Persistência:** PostgreSQL 16 via Docker, migrations EF Core.

## Variáveis de ambiente da API

| Variável | Descrição |
| --- | --- |
| `ConnectionStrings__DefaultConnection` | Connection string do PostgreSQL |
| `Jwt__Secret` | Chave de assinatura do token (não commitar valor de produção) |
| `Jwt__Issuer` | Emissor JWT |
| `Jwt__Audience` | Audiência JWT |
| `Jwt__ExpirationMinutes` | Validade do token |

O secret de desenvolvimento fica em `src/Backend/appsettings.Development.json` e não deve ser reutilizado em produção.

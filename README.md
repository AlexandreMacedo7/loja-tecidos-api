# Loja Tecidos API

Backend da loja de tecidos (Empório dos Tecidos) em .NET 10 com Clean Architecture.

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

## Configuração local

### 1. Variáveis de ambiente

Copie o template e ajuste se necessário:

```powershell
copy .env.example .env
```

| Arquivo | Vai para o Git? |
|---------|-----------------|
| `.env.example` | Sim (template, sem segredos reais) |
| `.env` | **Não** (está no `.gitignore`) |

A senha padrão de desenvolvimento está documentada em `.env.example` e espelhada em `appsettings.Development.json`.

### 2. SQL Server (Docker)

```powershell
docker compose up -d
docker compose ps
```

Aguarde o container `lojatecidos-sql` ficar **healthy**.

### 3. Executar a API

```powershell
dotnet run --project LojaTecidos.Api
```

Em Development, as migrations são aplicadas automaticamente na subida.

| Recurso | URL |
|---------|-----|
| Scalar (documentação interativa) | http://localhost:5051/scalar/v1 |
| OpenAPI JSON | http://localhost:5051/openapi/v1.json |
| Health check | http://localhost:5051/health |

## Autenticação

A API usa **JWT**. Endpoints de negócio exigem token, exceto login e health.

### Usuários seed (Development / Testing)

| E-mail | Senha | Papel |
|--------|-------|-------|
| `admin@emporiotecidos.com.br` | `Admin@123` | Admin |
| `gerente@emporiotecidos.com.br` | `Gerente@123` | Gerente |

### Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "gerente@emporiotecidos.com.br",
  "senha": "Gerente@123"
}
```

Use o token retornado no header: `Authorization: Bearer {token}`

### Permissões

| Operação | Gerente | Admin |
|----------|---------|-------|
| Clientes, produtos, vendas | Sim | Sim |
| Alterar perfil de crédito | Não | Sim |
| Bloquear/desbloquear cliente | Não | Sim |

## CORS

Em Development, origens configuradas em `appsettings.Development.json` (`Cors:Origens`), incluindo `http://localhost:5173` para frontends locais.

## CI (GitHub Actions)

O workflow `.github/workflows/ci.yml` executa `dotnet build` e `dotnet test` em cada push/PR na `main`. Não há deploy — tudo roda localmente ou no runner do GitHub.

## Testes

```powershell
dotnet test
```

## Estrutura

```
LojaTecidos.Domain          → regras de negócio
LojaTecidos.Application     → casos de uso
LojaTecidos.Infrastructure  → EF Core + SQL Server
LojaTecidos.Api             → endpoints REST
```

## Migrations manuais

Se precisar aplicar migrations fora da API:

```powershell
dotnet ef database update --project LojaTecidos.Infrastructure --startup-project LojaTecidos.Api
```

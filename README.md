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

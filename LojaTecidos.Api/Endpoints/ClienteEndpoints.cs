using LojaTecidos.Application.Abstractions;
using LojaTecidos.Application.Clientes;
using LojaTecidos.Application.Common.Dtos;
using LojaTecidos.Application.Common.Paginacao;
using LojaTecidos.Api.Authorization;
using LojaTecidos.Api.Extensions;
using LojaTecidos.Domain.Entities.Enum;

namespace LojaTecidos.Api.Endpoints;

public static class ClienteEndpoints
{
    public static IEndpointRouteBuilder MapClienteEndpoints(this IEndpointRouteBuilder app)
    {
        var authorization = PoliticasAutorizacao.GerenteOuAdmin;

        app.MapPost("/api/clientes", async (
            CadastrarClienteRequest request,
            IUseCase<CadastrarClienteRequest, ClienteDto> useCase,
            CancellationToken cancellationToken) =>
            await UseCaseEndpointExtensions.ExecuteAsync(
                useCase,
                request,
                dto => Results.Created($"/api/clientes/{dto.Id}", dto),
                cancellationToken))
            .WithTags("Clientes")
            .RequireAuthorization(authorization);

        app.MapGet("/api/clientes", async (
            int? pagina,
            int? tamanhoPagina,
            IUseCase<ListarClientesRequest, ResultadoPaginadoDto<ClienteDto>> useCase,
            CancellationToken cancellationToken) =>
            await UseCaseEndpointExtensions.ExecuteAsync(
                useCase,
                new ListarClientesRequest(
                    pagina ?? 1,
                    tamanhoPagina ?? PaginacaoParametros.TamanhoPaginaPadrao),
                Results.Ok,
                cancellationToken))
            .WithTags("Clientes")
            .RequireAuthorization(authorization);

        var group = app.MapGroup("/api/clientes")
            .WithTags("Clientes")
            .RequireAuthorization(authorization);

        group.MapGet("/{clienteId:guid}", async (
            Guid clienteId,
            IUseCase<ObterClienteRequest, ClienteDto?> useCase,
            CancellationToken cancellationToken) =>
            await UseCaseEndpointExtensions.ExecuteAsync(
                useCase,
                new ObterClienteRequest(clienteId),
                dto => dto is null ? Results.NotFound() : Results.Ok(dto),
                cancellationToken));

        group.MapPut("/{clienteId:guid}/perfil", async (
            Guid clienteId,
            AlterarPerfilBody body,
            IUseCase<AlterarPerfilClienteRequest, ClienteDto> useCase,
            CancellationToken cancellationToken) =>
            await UseCaseEndpointExtensions.ExecuteAsync(
                useCase,
                new AlterarPerfilClienteRequest(clienteId, body.Categoria),
                Results.Ok,
                cancellationToken))
            .RequireAuthorization(PoliticasAutorizacao.Admin);

        group.MapPut("/{clienteId:guid}/bloqueio", async (
            Guid clienteId,
            AlterarBloqueioBody body,
            IUseCase<AlterarBloqueioClienteRequest, ClienteDto> useCase,
            CancellationToken cancellationToken) =>
            await UseCaseEndpointExtensions.ExecuteAsync(
                useCase,
                new AlterarBloqueioClienteRequest(clienteId, body.Bloqueado),
                Results.Ok,
                cancellationToken))
            .RequireAuthorization(PoliticasAutorizacao.Admin);

        group.MapPost("/{clienteId:guid}/pagamentos-fiado", async (
            Guid clienteId,
            RegistrarPagamentoFiadoBody body,
            IUseCase<RegistrarPagamentoFiadoRequest, ClienteDto> useCase,
            CancellationToken cancellationToken) =>
            await UseCaseEndpointExtensions.ExecuteAsync(
                useCase,
                new RegistrarPagamentoFiadoRequest(
                    clienteId,
                    body.Valor,
                    body.DataPagamento,
                    body.NovaDataVencimento),
                Results.Ok,
                cancellationToken));

        return app;
    }

    private sealed record AlterarPerfilBody(CategoriaPerfil Categoria);

    private sealed record AlterarBloqueioBody(bool Bloqueado);

    private sealed record RegistrarPagamentoFiadoBody(
        decimal Valor,
        DateTime DataPagamento,
        DateTime? NovaDataVencimento = null);
}

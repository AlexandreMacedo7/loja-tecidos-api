using LojaTecidos.Application.Abstractions;
using LojaTecidos.Application.Common.Dtos;
using LojaTecidos.Application.Vendas;
using LojaTecidos.Api.Authorization;
using LojaTecidos.Api.Extensions;

namespace LojaTecidos.Api.Endpoints;

public static class VendaEndpoints
{
    public static IEndpointRouteBuilder MapVendaEndpoints(this IEndpointRouteBuilder app)
    {
        var authorization = PoliticasAutorizacao.GerenteOuAdmin;

        app.MapPost("/api/vendas/avista", async (
            RegistrarVendaAvistaBody body,
            HttpContext httpContext,
            IUseCase<RegistrarVendaAvistaRequest, VendaDto> useCase,
            CancellationToken cancellationToken) =>
        {
            var usuarioId = httpContext.User.ObterUsuarioId();
            if (string.IsNullOrWhiteSpace(usuarioId))
                return Results.Unauthorized();

            return await UseCaseEndpointExtensions.ExecuteAsync(
                useCase,
                new RegistrarVendaAvistaRequest(
                    body.DataVenda ?? DataHoraLoja.Agora,
                    body.Itens,
                    body.DescontoPercentual,
                    usuarioId),
                dto => Results.Created($"/api/vendas/{dto.CodigoVenda}", dto),
                cancellationToken);
        })
        .WithTags("Vendas")
        .RequireAuthorization(authorization);

        app.MapPost("/api/vendas/fiado", async (
            RegistrarVendaFiadoBody body,
            HttpContext httpContext,
            IUseCase<RegistrarVendaFiadoRequest, VendaDto> useCase,
            CancellationToken cancellationToken) =>
        {
            var usuarioId = httpContext.User.ObterUsuarioId();
            if (string.IsNullOrWhiteSpace(usuarioId))
                return Results.Unauthorized();

            return await UseCaseEndpointExtensions.ExecuteAsync(
                useCase,
                new RegistrarVendaFiadoRequest(
                    body.ClienteId,
                    body.DataVenda ?? DataHoraLoja.Agora,
                    body.DataVencimento,
                    body.Itens,
                    body.DescontoPercentual,
                    usuarioId),
                dto => Results.Created($"/api/vendas/{dto.CodigoVenda}", dto),
                cancellationToken);
        })
        .WithTags("Vendas")
        .RequireAuthorization(authorization);

        var group = app.MapGroup("/api/vendas")
            .WithTags("Vendas")
            .RequireAuthorization(authorization);

        group.MapPost("/{codigoVenda}/devolucao", async (
            string codigoVenda,
            RegistrarDevolucaoBody body,
            IUseCase<RegistrarDevolucaoRequest, VendaDto> useCase,
            CancellationToken cancellationToken) =>
            await UseCaseEndpointExtensions.ExecuteAsync(
                useCase,
                new RegistrarDevolucaoRequest(
                    codigoVenda,
                    body.DataDevolucao ?? DataHoraLoja.Agora),
                Results.Ok,
                cancellationToken));

        group.MapGet("/{codigoVenda}", async (
            string codigoVenda,
            IUseCase<ObterVendaRequest, VendaDto?> useCase,
            CancellationToken cancellationToken) =>
            await UseCaseEndpointExtensions.ExecuteAsync(
                useCase,
                new ObterVendaRequest(codigoVenda),
                dto => dto is null ? Results.NotFound() : Results.Ok(dto),
                cancellationToken));

        return app;
    }

    private sealed record RegistrarVendaAvistaBody(
        IReadOnlyList<ItemVendaRequest> Itens,
        DateTime? DataVenda = null,
        decimal DescontoPercentual = 0);

    private sealed record RegistrarVendaFiadoBody(
        Guid ClienteId,
        DateTime DataVencimento,
        IReadOnlyList<ItemVendaRequest> Itens,
        DateTime? DataVenda = null,
        decimal DescontoPercentual = 0);

    private sealed record RegistrarDevolucaoBody(DateTime? DataDevolucao = null);
}

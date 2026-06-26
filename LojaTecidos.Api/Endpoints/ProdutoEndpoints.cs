using LojaTecidos.Application.Abstractions;
using LojaTecidos.Application.Common.Dtos;
using LojaTecidos.Application.Produtos;
using LojaTecidos.Api.Authorization;
using LojaTecidos.Api.Extensions;
using LojaTecidos.Application.Common.Paginacao;
using LojaTecidos.Domain.Entities.Enum;

namespace LojaTecidos.Api.Endpoints;

public static class ProdutoEndpoints
{
    public static IEndpointRouteBuilder MapProdutoEndpoints(this IEndpointRouteBuilder app)
    {
        var authorization = PoliticasAutorizacao.GerenteOuAdmin;

        app.MapPost("/api/produtos", async (
            CadastrarProdutoBody body,
            IUseCase<CadastrarProdutoRequest, ProdutoDto> useCase,
            CancellationToken cancellationToken) =>
        {
            var dataCadastro = body.DataCadastro ?? DataHoraLoja.Agora;
            return await UseCaseEndpointExtensions.ExecuteAsync(
                useCase,
                new CadastrarProdutoRequest(
                    body.CodigoInterno,
                    body.Nome,
                    body.NomeFornecedor,
                    body.Categoria,
                    body.UnidadeMedida,
                    body.PrecoUnitario,
                    body.EstoqueInicial,
                    dataCadastro,
                    body.CodigoFornecedor),
                dto => Results.Created($"/api/produtos/{dto.CodigoInterno}", dto),
                cancellationToken);
        })
        .WithTags("Produtos")
        .RequireAuthorization(authorization);

        app.MapGet("/api/produtos", async (
            int? pagina,
            int? tamanhoPagina,
            IUseCase<ListarProdutosRequest, ResultadoPaginadoDto<ProdutoDto>> useCase,
            CancellationToken cancellationToken) =>
            await UseCaseEndpointExtensions.ExecuteAsync(
                useCase,
                new ListarProdutosRequest(
                    DataHoraLoja.Agora,
                    pagina ?? 1,
                    tamanhoPagina ?? PaginacaoParametros.TamanhoPaginaPadrao),
                Results.Ok,
                cancellationToken))
        .WithTags("Produtos")
        .RequireAuthorization(authorization);

        app.MapGet("/api/produtos/alertas", async (
            IUseCase<ListarAlertasEstoqueRequest, IReadOnlyList<AlertaEstoqueDto>> useCase,
            CancellationToken cancellationToken) =>
            await UseCaseEndpointExtensions.ExecuteAsync(
                useCase,
                new ListarAlertasEstoqueRequest(DataHoraLoja.Agora),
                Results.Ok,
                cancellationToken))
        .WithTags("Produtos")
        .RequireAuthorization(authorization);

        var group = app.MapGroup("/api/produtos")
            .WithTags("Produtos")
            .RequireAuthorization(authorization);

        group.MapGet("/{codigoInterno}", async (
            string codigoInterno,
            IUseCase<ObterProdutoRequest, ProdutoDto?> useCase,
            CancellationToken cancellationToken) =>
            await UseCaseEndpointExtensions.ExecuteAsync(
                useCase,
                new ObterProdutoRequest(codigoInterno, DataHoraLoja.Agora),
                dto => dto is null ? Results.NotFound() : Results.Ok(dto),
                cancellationToken));

        group.MapPost("/{codigoInterno}/entrada-estoque", async (
            string codigoInterno,
            MovimentacaoEstoqueBody body,
            IUseCase<RegistrarEntradaEstoqueRequest, ProdutoDto> useCase,
            CancellationToken cancellationToken) =>
            await UseCaseEndpointExtensions.ExecuteAsync(
                useCase,
                new RegistrarEntradaEstoqueRequest(
                    codigoInterno,
                    body.Quantidade,
                    body.Data ?? DataHoraLoja.Agora,
                    DataHoraLoja.Agora),
                Results.Ok,
                cancellationToken));

        group.MapPut("/{codigoInterno}/estoque", async (
            string codigoInterno,
            AtualizarEstoqueBody body,
            IUseCase<AtualizarEstoqueRequest, ProdutoDto> useCase,
            CancellationToken cancellationToken) =>
            await UseCaseEndpointExtensions.ExecuteAsync(
                useCase,
                new AtualizarEstoqueRequest(
                    codigoInterno,
                    body.NovaQuantidade,
                    body.Data ?? DataHoraLoja.Agora,
                    DataHoraLoja.Agora),
                Results.Ok,
                cancellationToken));

        return app;
    }

    private sealed record CadastrarProdutoBody(
        string? CodigoInterno,
        string Nome,
        string NomeFornecedor,
        CategoriaProduto Categoria,
        UnidadeMedida UnidadeMedida,
        decimal PrecoUnitario,
        decimal EstoqueInicial,
        DateTime? DataCadastro = null,
        string? CodigoFornecedor = null);

    private sealed record MovimentacaoEstoqueBody(decimal Quantidade, DateTime? Data = null);

    private sealed record AtualizarEstoqueBody(decimal NovaQuantidade, DateTime? Data = null);
}

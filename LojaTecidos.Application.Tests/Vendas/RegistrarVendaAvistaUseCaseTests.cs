using LojaTecidos.Application.Abstractions;
using LojaTecidos.Application.Abstractions.Persistence;
using LojaTecidos.Application.Vendas;
using LojaTecidos.Domain.Entities;
using LojaTecidos.Domain.Entities.Enum;
using LojaTecidos.Domain.Services;
using LojaTecidos.Domain.ValueObjects;

namespace LojaTecidos.Application.Tests.Vendas;

public class RegistrarVendaAvistaUseCaseTests
{
    private static readonly DateTime DataVenda = new(2026, 6, 26);

    [Fact]
    public async Task ExecuteAsync_DadosValidos_DeveRegistrarVendaComUsuarioId()
    {
        var produto = new Produto(
            "INT-001",
            "Tecido",
            new Fornecedor("Janon"),
            CategoriaProduto.Tecido,
            UnidadeMedida.Metro,
            25m,
            100m,
            new DateTime(2026, 1, 1));

        var produtoRepository = new ProdutoRepositoryFake(produto);
        var vendaRepository = new VendaRepositoryFake();
        var useCase = new RegistrarVendaAvistaUseCase(
            vendaRepository,
            produtoRepository,
            new UnitOfWorkFake(),
            new ServicoVenda(),
            new GeradorCodigoVendaFake());

        var resultado = await useCase.ExecuteAsync(new RegistrarVendaAvistaRequest(
            DataVenda,
            [new ItemVendaRequest("INT-001", 10m)],
            UsuarioId: "usuario-teste"));

        Assert.Equal("usuario-teste", resultado.UsuarioId);
        Assert.Equal(90m, produto.QuantidadeAtual);
        Assert.Equal(1, vendaRepository.Adicionadas);
    }

    [Fact]
    public async Task ExecuteAsync_SemUsuarioId_DeveLancarArgumentException()
    {
        var useCase = new RegistrarVendaAvistaUseCase(
            new VendaRepositoryFake(),
            new ProdutoRepositoryFake(),
            new UnitOfWorkFake(),
            new ServicoVenda(),
            new GeradorCodigoVendaFake());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.ExecuteAsync(new RegistrarVendaAvistaRequest(
                DataVenda,
                [new ItemVendaRequest("INT-001", 1m)],
                UsuarioId: null)));
    }

    [Fact]
    public async Task ExecuteAsync_SemItens_DeveLancarArgumentException()
    {
        var useCase = new RegistrarVendaAvistaUseCase(
            new VendaRepositoryFake(),
            new ProdutoRepositoryFake(),
            new UnitOfWorkFake(),
            new ServicoVenda(),
            new GeradorCodigoVendaFake());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.ExecuteAsync(new RegistrarVendaAvistaRequest(
                DataVenda,
                [],
                UsuarioId: "usuario-teste")));
    }
}

internal sealed class GeradorCodigoVendaFake : IGeradorCodigoVenda
{
    public string Gerar(DateTime dataReferencia) => "VND-TEST-001";
}

internal sealed class ProdutoRepositoryFake : IProdutoRepository
{
    private readonly Dictionary<string, Produto> _produtos;

    public ProdutoRepositoryFake(params Produto[] produtos) =>
        _produtos = produtos.ToDictionary(p => p.CodigoInterno, StringComparer.OrdinalIgnoreCase);

    public Task<Produto?> ObterPorCodigoInternoAsync(string codigoInterno, CancellationToken cancellationToken = default) =>
        Task.FromResult(_produtos.GetValueOrDefault(codigoInterno));

    public Task<ResultadoConsultaPaginada<Produto>> ListarPaginadoAsync(
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(new ResultadoConsultaPaginada<Produto>(_produtos.Values.ToList(), _produtos.Count));

    public Task<IReadOnlyList<Produto>> ListarAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Produto>>(_produtos.Values.ToList());

    public Task<IReadOnlyList<Produto>> ObterPorCodigosInternosAsync(
        IEnumerable<string> codigosInternos,
        CancellationToken cancellationToken = default)
    {
        var codigos = codigosInternos.ToHashSet(StringComparer.OrdinalIgnoreCase);
        return Task.FromResult<IReadOnlyList<Produto>>(
            _produtos.Values.Where(p => codigos.Contains(p.CodigoInterno)).ToList());
    }

    public Task AdicionarAsync(Produto produto, CancellationToken cancellationToken = default)
    {
        _produtos[produto.CodigoInterno] = produto;
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Produto produto, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}

internal sealed class VendaRepositoryFake : IVendaRepository
{
    public int Adicionadas { get; private set; }

    public Task AdicionarAsync(Venda venda, Guid? clienteId, CancellationToken cancellationToken = default)
    {
        Adicionadas++;
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Venda venda, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task<Venda?> ObterPorCodigoAsync(string codigoVenda, CancellationToken cancellationToken = default) =>
        Task.FromResult<Venda?>(null);

    public Task<Guid?> ObterClienteIdPorCodigoVendaAsync(string codigoVenda, CancellationToken cancellationToken = default) =>
        Task.FromResult<Guid?>(null);

    public Task<int> ObterProximoNumeroSequencialAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(1);
}

internal sealed class UnitOfWorkFake : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(1);
}

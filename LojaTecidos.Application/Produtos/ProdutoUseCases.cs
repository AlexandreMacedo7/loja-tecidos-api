using LojaTecidos.Application.Abstractions;
using LojaTecidos.Application.Abstractions.Persistence;
using LojaTecidos.Application.Common.Dtos;
using LojaTecidos.Application.Common.Mappings;
using LojaTecidos.Domain.Entities;
using LojaTecidos.Domain.Entities.Enum;
using LojaTecidos.Domain.Exceptions;

namespace LojaTecidos.Application.Produtos;

public sealed record CadastrarProdutoRequest(
    string? CodigoInterno,
    string Nome,
    string NomeFornecedor,
    CategoriaProduto Categoria,
    UnidadeMedida UnidadeMedida,
    decimal PrecoUnitario,
    decimal EstoqueInicial,
    DateTime DataCadastro,
    string? CodigoFornecedor = null);

public sealed class CadastrarProdutoUseCase : IUseCase<CadastrarProdutoRequest, ProdutoDto>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IFornecedorRepository _fornecedorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CadastrarProdutoUseCase(
        IProdutoRepository produtoRepository,
        IFornecedorRepository fornecedorRepository,
        IUnitOfWork unitOfWork)
    {
        _produtoRepository = produtoRepository;
        _fornecedorRepository = fornecedorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProdutoDto> ExecuteAsync(
        CadastrarProdutoRequest request,
        CancellationToken cancellationToken = default)
    {
        var fornecedor = await ObterOuCriarFornecedorAsync(request.NomeFornecedor, cancellationToken);
        var codigoInterno = string.IsNullOrWhiteSpace(request.CodigoInterno)
            ? $"INT-{Guid.NewGuid():N}"[..12].ToUpperInvariant()
            : request.CodigoInterno.Trim();

        var produto = new Produto(
            codigoInterno,
            request.Nome,
            fornecedor,
            request.Categoria,
            request.UnidadeMedida,
            request.PrecoUnitario,
            request.EstoqueInicial,
            request.DataCadastro,
            request.CodigoFornecedor);

        await _produtoRepository.AdicionarAsync(produto, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return DomainMapper.ToDto(produto, request.DataCadastro);
    }

    private async Task<Fornecedor> ObterOuCriarFornecedorAsync(
        string nomeFornecedor,
        CancellationToken cancellationToken)
    {
        var fornecedor = await _fornecedorRepository.ObterPorNomeAsync(nomeFornecedor, cancellationToken);
        if (fornecedor is not null)
            return fornecedor;

        fornecedor = new Fornecedor(nomeFornecedor);
        await _fornecedorRepository.AdicionarAsync(fornecedor, cancellationToken);
        return fornecedor;
    }
}

public sealed record ObterProdutoRequest(string CodigoInterno, DateTime DataReferenciaAlertas);

public sealed class ObterProdutoUseCase : IUseCase<ObterProdutoRequest, ProdutoDto?>
{
    private readonly IProdutoRepository _produtoRepository;

    public ObterProdutoUseCase(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<ProdutoDto?> ExecuteAsync(
        ObterProdutoRequest request,
        CancellationToken cancellationToken = default)
    {
        var produto = await _produtoRepository.ObterPorCodigoInternoAsync(
            request.CodigoInterno,
            cancellationToken);

        return produto is null
            ? null
            : DomainMapper.ToDto(produto, request.DataReferenciaAlertas);
    }
}

public sealed record ListarProdutosRequest(DateTime DataReferenciaAlertas);

public sealed class ListarProdutosUseCase : IUseCase<ListarProdutosRequest, IReadOnlyList<ProdutoDto>>
{
    private readonly IProdutoRepository _produtoRepository;

    public ListarProdutosUseCase(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<IReadOnlyList<ProdutoDto>> ExecuteAsync(
        ListarProdutosRequest request,
        CancellationToken cancellationToken = default)
    {
        var produtos = await _produtoRepository.ListarAsync(cancellationToken);
        return produtos
            .Select(p => DomainMapper.ToDto(p, request.DataReferenciaAlertas))
            .ToList();
    }
}

public sealed record RegistrarEntradaEstoqueRequest(
    string CodigoInterno,
    decimal Quantidade,
    DateTime Data,
    DateTime DataReferenciaAlertas);

public sealed class RegistrarEntradaEstoqueUseCase : IUseCase<RegistrarEntradaEstoqueRequest, ProdutoDto>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarEntradaEstoqueUseCase(IProdutoRepository produtoRepository, IUnitOfWork unitOfWork)
    {
        _produtoRepository = produtoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProdutoDto> ExecuteAsync(
        RegistrarEntradaEstoqueRequest request,
        CancellationToken cancellationToken = default)
    {
        var produto = await ObterProdutoOuFalharAsync(request.CodigoInterno, cancellationToken);

        produto.RegistrarEntrada(request.Quantidade, request.Data);
        await _produtoRepository.AtualizarAsync(produto, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return DomainMapper.ToDto(produto, request.DataReferenciaAlertas);
    }

    private async Task<Produto> ObterProdutoOuFalharAsync(string codigoInterno, CancellationToken cancellationToken) =>
        await _produtoRepository.ObterPorCodigoInternoAsync(codigoInterno, cancellationToken)
        ?? throw new EntidadeNaoEncontradaException($"Produto {codigoInterno} não encontrado.");
}

public sealed record AtualizarEstoqueRequest(
    string CodigoInterno,
    decimal NovaQuantidade,
    DateTime Data,
    DateTime DataReferenciaAlertas);

public sealed class AtualizarEstoqueUseCase : IUseCase<AtualizarEstoqueRequest, ProdutoDto>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AtualizarEstoqueUseCase(IProdutoRepository produtoRepository, IUnitOfWork unitOfWork)
    {
        _produtoRepository = produtoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProdutoDto> ExecuteAsync(
        AtualizarEstoqueRequest request,
        CancellationToken cancellationToken = default)
    {
        var produto = await _produtoRepository.ObterPorCodigoInternoAsync(request.CodigoInterno, cancellationToken)
            ?? throw new EntidadeNaoEncontradaException($"Produto {request.CodigoInterno} não encontrado.");

        produto.AtualizarEstoque(request.NovaQuantidade, request.Data);
        await _produtoRepository.AtualizarAsync(produto, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return DomainMapper.ToDto(produto, request.DataReferenciaAlertas);
    }
}

public sealed record ListarAlertasEstoqueRequest(DateTime DataReferencia);

public sealed class ListarAlertasEstoqueUseCase : IUseCase<ListarAlertasEstoqueRequest, IReadOnlyList<AlertaEstoqueDto>>
{
    private readonly IProdutoRepository _produtoRepository;

    public ListarAlertasEstoqueUseCase(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<IReadOnlyList<AlertaEstoqueDto>> ExecuteAsync(
        ListarAlertasEstoqueRequest request,
        CancellationToken cancellationToken = default)
    {
        var produtos = await _produtoRepository.ListarAsync(cancellationToken);

        return produtos
            .Where(p => p.EstaProximoDoFim() || p.EstaParado(request.DataReferencia))
            .Select(p => new AlertaEstoqueDto(
                p.CodigoInterno,
                p.Nome,
                p.QuantidadeAtual,
                p.QuantidadeReferenciaAlerta,
                p.EstaProximoDoFim(),
                p.EstaParado(request.DataReferencia)))
            .ToList();
    }
}

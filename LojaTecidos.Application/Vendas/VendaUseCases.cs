using LojaTecidos.Application.Abstractions;
using LojaTecidos.Application.Abstractions.Persistence;
using LojaTecidos.Application.Common.Dtos;
using LojaTecidos.Application.Common.Mappings;
using LojaTecidos.Application.Common.Validation;
using LojaTecidos.Domain.Entities;
using LojaTecidos.Domain.Exceptions;
using LojaTecidos.Domain.Services;

namespace LojaTecidos.Application.Vendas;

public sealed record ItemVendaRequest(string CodigoInternoProduto, decimal Quantidade);

public sealed record RegistrarVendaAvistaRequest(
    DateTime DataVenda,
    IReadOnlyList<ItemVendaRequest> Itens,
    decimal DescontoPercentual = 0,
    string? UsuarioId = null);

public sealed class RegistrarVendaAvistaUseCase : IUseCase<RegistrarVendaAvistaRequest, VendaDto>
{
    private readonly IVendaRepository _vendaRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ServicoVenda _servicoVenda;
    private readonly IGeradorCodigoVenda _geradorCodigoVenda;

    public RegistrarVendaAvistaUseCase(
        IVendaRepository vendaRepository,
        IProdutoRepository produtoRepository,
        IUnitOfWork unitOfWork,
        ServicoVenda servicoVenda,
        IGeradorCodigoVenda geradorCodigoVenda)
    {
        _vendaRepository = vendaRepository;
        _produtoRepository = produtoRepository;
        _unitOfWork = unitOfWork;
        _servicoVenda = servicoVenda;
        _geradorCodigoVenda = geradorCodigoVenda;
    }

    public async Task<VendaDto> ExecuteAsync(
        RegistrarVendaAvistaRequest request,
        CancellationToken cancellationToken = default)
    {
        VendaRequestValidator.ValidarUsuarioId(request.UsuarioId);
        VendaRequestValidator.ValidarItens(request.Itens);
        VendaRequestValidator.ValidarDesconto(request.DescontoPercentual);

        var produtos = await VendaProdutoHelper.CarregarProdutosAsync(
            _produtoRepository, request.Itens, cancellationToken);
        var itens = VendaProdutoHelper.CriarItensVenda(request.Itens, produtos);
        var numeroSequencial = await _vendaRepository.ObterProximoNumeroSequencialAsync(cancellationToken);
        var codigoVenda = _geradorCodigoVenda.Gerar(request.DataVenda);

        var venda = _servicoVenda.RegistrarVendaAvista(
            numeroSequencial,
            codigoVenda,
            request.DataVenda,
            itens,
            produtos,
            request.DescontoPercentual,
            request.UsuarioId);

        await _vendaRepository.AdicionarAsync(venda, clienteId: null, cancellationToken);

        foreach (var produto in produtos.Values)
            await _produtoRepository.AtualizarAsync(produto, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return DomainMapper.ToDto(venda);
    }
}

public sealed record RegistrarVendaFiadoRequest(
    Guid ClienteId,
    DateTime DataVenda,
    DateTime DataVencimento,
    IReadOnlyList<ItemVendaRequest> Itens,
    decimal DescontoPercentual = 0,
    string? UsuarioId = null);

public sealed class RegistrarVendaFiadoUseCase : IUseCase<RegistrarVendaFiadoRequest, VendaDto>
{
    private readonly IVendaRepository _vendaRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ServicoVenda _servicoVenda;
    private readonly IGeradorCodigoVenda _geradorCodigoVenda;

    public RegistrarVendaFiadoUseCase(
        IVendaRepository vendaRepository,
        IProdutoRepository produtoRepository,
        IClienteRepository clienteRepository,
        IUnitOfWork unitOfWork,
        ServicoVenda servicoVenda,
        IGeradorCodigoVenda geradorCodigoVenda)
    {
        _vendaRepository = vendaRepository;
        _produtoRepository = produtoRepository;
        _clienteRepository = clienteRepository;
        _unitOfWork = unitOfWork;
        _servicoVenda = servicoVenda;
        _geradorCodigoVenda = geradorCodigoVenda;
    }

    public async Task<VendaDto> ExecuteAsync(
        RegistrarVendaFiadoRequest request,
        CancellationToken cancellationToken = default)
    {
        VendaRequestValidator.ValidarUsuarioId(request.UsuarioId);
        VendaRequestValidator.ValidarItens(request.Itens);
        VendaRequestValidator.ValidarDesconto(request.DescontoPercentual);

        if (request.ClienteId == Guid.Empty)
            throw new ArgumentException("ClienteId é obrigatório.");

        if (request.DataVencimento <= request.DataVenda)
            throw new ArgumentException("Data de vencimento deve ser posterior à data da venda.");

        var cliente = await _clienteRepository.ObterPorIdAsync(request.ClienteId, cancellationToken)
            ?? throw new EntidadeNaoEncontradaException($"Cliente {request.ClienteId} não encontrado.");

        var produtos = await VendaProdutoHelper.CarregarProdutosAsync(
            _produtoRepository, request.Itens, cancellationToken);
        var itens = VendaProdutoHelper.CriarItensVenda(request.Itens, produtos);
        var numeroSequencial = await _vendaRepository.ObterProximoNumeroSequencialAsync(cancellationToken);
        var codigoVenda = _geradorCodigoVenda.Gerar(request.DataVenda);

        var venda = _servicoVenda.RegistrarVendaFiado(
            numeroSequencial,
            codigoVenda,
            request.DataVenda,
            request.DataVencimento,
            cliente,
            itens,
            produtos,
            request.DescontoPercentual,
            request.UsuarioId);

        await _vendaRepository.AdicionarAsync(venda, cliente.Id, cancellationToken);
        await _clienteRepository.AtualizarAsync(cliente, cancellationToken);

        foreach (var produto in produtos.Values)
            await _produtoRepository.AtualizarAsync(produto, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return DomainMapper.ToDto(venda);
    }
}

public sealed record RegistrarDevolucaoRequest(string CodigoVenda, DateTime DataDevolucao);

public sealed class RegistrarDevolucaoUseCase : IUseCase<RegistrarDevolucaoRequest, VendaDto>
{
    private readonly IVendaRepository _vendaRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ServicoVenda _servicoVenda;

    public RegistrarDevolucaoUseCase(
        IVendaRepository vendaRepository,
        IProdutoRepository produtoRepository,
        IClienteRepository clienteRepository,
        IUnitOfWork unitOfWork,
        ServicoVenda servicoVenda)
    {
        _vendaRepository = vendaRepository;
        _produtoRepository = produtoRepository;
        _clienteRepository = clienteRepository;
        _unitOfWork = unitOfWork;
        _servicoVenda = servicoVenda;
    }

    public async Task<VendaDto> ExecuteAsync(
        RegistrarDevolucaoRequest request,
        CancellationToken cancellationToken = default)
    {
        var venda = await _vendaRepository.ObterPorCodigoAsync(request.CodigoVenda, cancellationToken)
            ?? throw new EntidadeNaoEncontradaException($"Venda {request.CodigoVenda} não encontrada.");

        var produtos = await CarregarProdutosDaVendaAsync(venda, cancellationToken);
        Cliente? cliente = null;

        if (venda.Tipo == Domain.Entities.Enum.TipoVenda.Fiado)
        {
            var clienteId = await _vendaRepository.ObterClienteIdPorCodigoVendaAsync(
                request.CodigoVenda,
                cancellationToken)
                ?? throw new EntidadeNaoEncontradaException(
                    $"Cliente da venda fiado {request.CodigoVenda} não encontrado.");

            cliente = await _clienteRepository.ObterPorIdAsync(clienteId, cancellationToken)
                ?? throw new EntidadeNaoEncontradaException($"Cliente {clienteId} não encontrado.");
        }

        _servicoVenda.RegistrarDevolucao(venda, request.DataDevolucao, produtos, cliente);

        await _vendaRepository.AtualizarAsync(venda, cancellationToken);

        foreach (var produto in produtos.Values)
            await _produtoRepository.AtualizarAsync(produto, cancellationToken);

        if (cliente is not null)
            await _clienteRepository.AtualizarAsync(cliente, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return DomainMapper.ToDto(venda);
    }

    private async Task<Dictionary<string, Produto>> CarregarProdutosDaVendaAsync(
        Venda venda,
        CancellationToken cancellationToken)
    {
        var codigos = venda.Itens.Select(i => i.CodigoInternoProduto).ToList();
        var produtos = await _produtoRepository.ObterPorCodigosInternosAsync(codigos, cancellationToken);
        return produtos.ToDictionary(p => p.CodigoInterno, StringComparer.OrdinalIgnoreCase);
    }
}

public sealed record ObterVendaRequest(string CodigoVenda);

public sealed class ObterVendaUseCase : IUseCase<ObterVendaRequest, VendaDto?>
{
    private readonly IVendaRepository _vendaRepository;

    public ObterVendaUseCase(IVendaRepository vendaRepository)
    {
        _vendaRepository = vendaRepository;
    }

    public async Task<VendaDto?> ExecuteAsync(
        ObterVendaRequest request,
        CancellationToken cancellationToken = default)
    {
        var venda = await _vendaRepository.ObterPorCodigoAsync(request.CodigoVenda, cancellationToken);
        return venda is null ? null : DomainMapper.ToDto(venda);
    }
}

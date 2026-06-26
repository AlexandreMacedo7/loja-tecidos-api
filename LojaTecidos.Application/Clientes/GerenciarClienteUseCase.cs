using LojaTecidos.Application.Abstractions;
using LojaTecidos.Application.Abstractions.Persistence;
using LojaTecidos.Application.Common.Dtos;
using LojaTecidos.Application.Common.Mappings;
using LojaTecidos.Domain.Entities;
using LojaTecidos.Domain.Entities.Enum;
using LojaTecidos.Domain.Exceptions;

namespace LojaTecidos.Application.Clientes;

public sealed record AlterarPerfilClienteRequest(Guid ClienteId, CategoriaPerfil Categoria);

public sealed class AlterarPerfilClienteUseCase : IUseCase<AlterarPerfilClienteRequest, ClienteDto>
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AlterarPerfilClienteUseCase(IClienteRepository clienteRepository, IUnitOfWork unitOfWork)
    {
        _clienteRepository = clienteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ClienteDto> ExecuteAsync(
        AlterarPerfilClienteRequest request,
        CancellationToken cancellationToken = default)
    {
        var cliente = await ObterClienteOuFalharAsync(request.ClienteId, cancellationToken);

        cliente.AlterarPerfil(request.Categoria);
        await _clienteRepository.AtualizarAsync(cliente, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return DomainMapper.ToDto(cliente);
    }

    private async Task<Cliente> ObterClienteOuFalharAsync(
        Guid clienteId,
        CancellationToken cancellationToken)
    {
        return await _clienteRepository.ObterPorIdAsync(clienteId, cancellationToken)
            ?? throw new EntidadeNaoEncontradaException($"Cliente {clienteId} não encontrado.");
    }
}

public sealed record AlterarBloqueioClienteRequest(Guid ClienteId, bool Bloqueado);

public sealed class AlterarBloqueioClienteUseCase : IUseCase<AlterarBloqueioClienteRequest, ClienteDto>
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AlterarBloqueioClienteUseCase(IClienteRepository clienteRepository, IUnitOfWork unitOfWork)
    {
        _clienteRepository = clienteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ClienteDto> ExecuteAsync(
        AlterarBloqueioClienteRequest request,
        CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(request.ClienteId, cancellationToken)
            ?? throw new EntidadeNaoEncontradaException($"Cliente {request.ClienteId} não encontrado.");

        if (request.Bloqueado)
            cliente.Bloquear();
        else
            cliente.Desbloquear();

        await _clienteRepository.AtualizarAsync(cliente, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return DomainMapper.ToDto(cliente);
    }
}

public sealed record RegistrarPagamentoFiadoRequest(
    Guid ClienteId,
    decimal Valor,
    DateTime DataPagamento,
    DateTime? NovaDataVencimento = null);

public sealed class RegistrarPagamentoFiadoUseCase : IUseCase<RegistrarPagamentoFiadoRequest, ClienteDto>
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarPagamentoFiadoUseCase(IClienteRepository clienteRepository, IUnitOfWork unitOfWork)
    {
        _clienteRepository = clienteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ClienteDto> ExecuteAsync(
        RegistrarPagamentoFiadoRequest request,
        CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(request.ClienteId, cancellationToken)
            ?? throw new EntidadeNaoEncontradaException($"Cliente {request.ClienteId} não encontrado.");

        cliente.RegistrarPagamentoFiado(
            request.Valor,
            request.DataPagamento,
            request.NovaDataVencimento);

        await _clienteRepository.AtualizarAsync(cliente, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return DomainMapper.ToDto(cliente);
    }
}

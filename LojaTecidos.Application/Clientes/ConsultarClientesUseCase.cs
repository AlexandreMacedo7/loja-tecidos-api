using LojaTecidos.Application.Abstractions;
using LojaTecidos.Application.Abstractions.Persistence;
using LojaTecidos.Application.Common.Dtos;
using LojaTecidos.Application.Common.Mappings;

namespace LojaTecidos.Application.Clientes;

public sealed record ObterClienteRequest(Guid ClienteId);

public sealed class ObterClienteUseCase : IUseCase<ObterClienteRequest, ClienteDto?>
{
    private readonly IClienteRepository _clienteRepository;

    public ObterClienteUseCase(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<ClienteDto?> ExecuteAsync(
        ObterClienteRequest request,
        CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(request.ClienteId, cancellationToken);
        return cliente is null ? null : DomainMapper.ToDto(cliente);
    }
}

public sealed record ListarClientesRequest;

public sealed class ListarClientesUseCase : IUseCase<ListarClientesRequest, IReadOnlyList<ClienteDto>>
{
    private readonly IClienteRepository _clienteRepository;

    public ListarClientesUseCase(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<IReadOnlyList<ClienteDto>> ExecuteAsync(
        ListarClientesRequest request,
        CancellationToken cancellationToken = default)
    {
        var clientes = await _clienteRepository.ListarAsync(cancellationToken);
        return clientes.Select(DomainMapper.ToDto).ToList();
    }
}

using LojaTecidos.Application.Abstractions;
using LojaTecidos.Application.Abstractions.Persistence;
using LojaTecidos.Application.Common.Dtos;
using LojaTecidos.Application.Common.Mappings;
using LojaTecidos.Application.Common.Paginacao;

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

public sealed record ListarClientesRequest(int Pagina = 1, int TamanhoPagina = PaginacaoParametros.TamanhoPaginaPadrao);

public sealed class ListarClientesUseCase : IUseCase<ListarClientesRequest, ResultadoPaginadoDto<ClienteDto>>
{
    private readonly IClienteRepository _clienteRepository;

    public ListarClientesUseCase(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<ResultadoPaginadoDto<ClienteDto>> ExecuteAsync(
        ListarClientesRequest request,
        CancellationToken cancellationToken = default)
    {
        var (pagina, tamanhoPagina) = PaginacaoParametros.Normalizar(request.Pagina, request.TamanhoPagina);
        var resultado = await _clienteRepository.ListarPaginadoAsync(pagina, tamanhoPagina, cancellationToken);

        var itens = resultado.Itens.Select(DomainMapper.ToDto).ToList();
        return PaginacaoParametros.Criar(itens, pagina, tamanhoPagina, resultado.TotalItens);
    }
}

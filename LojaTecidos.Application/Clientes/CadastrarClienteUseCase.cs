using LojaTecidos.Application.Abstractions;
using LojaTecidos.Application.Abstractions.Persistence;
using LojaTecidos.Application.Common.Dtos;
using LojaTecidos.Application.Common.Mappings;
using LojaTecidos.Application.Common.Validation;
using LojaTecidos.Domain.Entities;
using LojaTecidos.Domain.ValueObjects;

namespace LojaTecidos.Application.Clientes;

public sealed record CadastrarClienteRequest(
    string Nome,
    string Telefone,
    string Rua,
    string Numero,
    string Bairro,
    string? Cep = null,
    string? Cpf = null,
    string? Cnpj = null);

public sealed class CadastrarClienteUseCase : IUseCase<CadastrarClienteRequest, ClienteDto>
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CadastrarClienteUseCase(IClienteRepository clienteRepository, IUnitOfWork unitOfWork)
    {
        _clienteRepository = clienteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ClienteDto> ExecuteAsync(
        CadastrarClienteRequest request,
        CancellationToken cancellationToken = default)
    {
        CadastrarClienteRequestValidator.Validar(request);

        var cliente = new Cliente(
            request.Nome,
            request.Telefone,
            new Endereco(
                request.Rua,
                request.Numero,
                request.Bairro,
                request.Cep ?? Endereco.CepPadrao),
            request.Cpf,
            request.Cnpj);

        await _clienteRepository.AdicionarAsync(cliente, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return DomainMapper.ToDto(cliente);
    }
}

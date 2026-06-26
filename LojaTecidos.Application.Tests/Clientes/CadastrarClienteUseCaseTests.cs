using LojaTecidos.Application.Abstractions;
using LojaTecidos.Application.Abstractions.Persistence;
using LojaTecidos.Application.Clientes;
using LojaTecidos.Application.Common.Dtos;
using LojaTecidos.Domain.Entities;
using LojaTecidos.Domain.Entities.Enum;
using LojaTecidos.Domain.ValueObjects;

namespace LojaTecidos.Application.Tests.Clientes;

public class CadastrarClienteUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_DadosValidos_DeveRetornarClienteDto()
    {
        var repository = new ClienteRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new CadastrarClienteUseCase(repository, unitOfWork);

        var resultado = await useCase.ExecuteAsync(new CadastrarClienteRequest(
            "Maria",
            "92999999999",
            "Rua A",
            "10",
            "Centro"));

        Assert.Equal("Maria", resultado.Nome);
        Assert.Equal(CategoriaPerfil.BRONZE, resultado.CategoriaPerfil);
        Assert.Equal(Endereco.CepPadrao, resultado.Endereco.Cep);
        Assert.Equal(1, repository.Adicionados);
        Assert.Equal(1, unitOfWork.Salvamentos);
    }
}

internal sealed class ClienteRepositoryFake : IClienteRepository
{
    public int Adicionados { get; private set; }

    public Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        Adicionados++;
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Cliente cliente, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task<Cliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult<Cliente?>(null);

    public Task<IReadOnlyList<Cliente>> ListarAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Cliente>>([]);
}

internal sealed class UnitOfWorkFake : IUnitOfWork
{
    public int Salvamentos { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        Salvamentos++;
        return Task.FromResult(1);
    }
}

using LojaTecidos.Application.Abstractions.Persistence;
using LojaTecidos.Application.Clientes;
using LojaTecidos.Domain.Entities;
using LojaTecidos.Domain.ValueObjects;

namespace LojaTecidos.Application.Tests.Clientes;

public class ListarClientesUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_DeveRetornarPaginaComMetadados()
    {
        var repository = new ClienteRepositoryFake();
        for (var i = 1; i <= 3; i++)
        {
            await repository.AdicionarAsync(new Cliente(
                $"Cliente {i}",
                "92999999999",
                new Endereco("Rua A", "10", "Centro")));
        }

        var useCase = new ListarClientesUseCase(repository);
        var resultado = await useCase.ExecuteAsync(new ListarClientesRequest(Pagina: 1, TamanhoPagina: 2));

        Assert.Equal(2, resultado.Itens.Count);
        Assert.Equal(1, resultado.Pagina);
        Assert.Equal(2, resultado.TamanhoPagina);
        Assert.Equal(3, resultado.TotalItens);
        Assert.Equal(2, resultado.TotalPaginas);
    }

    [Fact]
    public async Task ExecuteAsync_TamanhoPaginaInvalido_DeveLancarArgumentException()
    {
        var useCase = new ListarClientesUseCase(new ClienteRepositoryFake());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.ExecuteAsync(new ListarClientesRequest(Pagina: 1, TamanhoPagina: 200)));
    }
}

public class CadastrarClienteValidacaoTests
{
    [Fact]
    public async Task ExecuteAsync_NomeVazio_DeveLancarArgumentException()
    {
        var useCase = new CadastrarClienteUseCase(new ClienteRepositoryFake(), new UnitOfWorkFake());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.ExecuteAsync(new CadastrarClienteRequest("", "92999999999", "Rua A", "10", "Centro")));
    }
}

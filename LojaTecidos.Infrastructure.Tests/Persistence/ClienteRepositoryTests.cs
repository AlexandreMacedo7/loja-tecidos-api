using LojaTecidos.Domain.Entities;
using LojaTecidos.Domain.Entities.Enum;
using LojaTecidos.Domain.ValueObjects;

namespace LojaTecidos.Infrastructure.Tests.Persistence;

public class ClienteRepositoryTests : RepositoryTestBase
{
    [Fact]
    public async Task AdicionarEObterCliente_DevePersistirDadosPrincipais()
    {
        var cliente = new Cliente(
            "Maria Silva",
            "92999999999",
            new Endereco("Rua A", "10", "Centro"));

        await ClienteRepository.AdicionarAsync(cliente);
        await UnitOfWork.SaveChangesAsync();

        var carregado = await ClienteRepository.ObterPorIdAsync(cliente.Id);

        Assert.NotNull(carregado);
        Assert.Equal("Maria Silva", carregado.Nome);
        Assert.Equal(CategoriaPerfil.BRONZE, carregado.PerfilCredito.Categoria);
        Assert.Equal(Endereco.CepPadrao, carregado.Endereco.Cep);
    }

    [Fact]
    public async Task AtualizarCliente_ComContaFiado_DevePersistirEstadoFiado()
    {
        var cliente = new Cliente(
            "João",
            "92988888888",
            new Endereco("Rua B", "20", "Centro"));

        await ClienteRepository.AdicionarAsync(cliente);
        await UnitOfWork.SaveChangesAsync();

        cliente.RegistrarCompraFiado(
            100m,
            new DateTime(2026, 6, 1),
            new DateTime(2026, 6, 20));

        await ClienteRepository.AtualizarAsync(cliente);
        await UnitOfWork.SaveChangesAsync();

        var carregado = await ClienteRepository.ObterPorIdAsync(cliente.Id);

        Assert.NotNull(carregado?.ContaFiadoAtiva);
        Assert.Equal(100m, carregado.ContaFiadoAtiva.SaldoDevedor);
    }
}

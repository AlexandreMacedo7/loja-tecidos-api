using LojaTecidos.Domain.Entities;
using LojaTecidos.Domain.Entities.Enum;

namespace LojaTecidos.Domain.Tests.Entities;

public class VendaTests
{
    private static readonly DateTime DataVenda = new(2026, 6, 26);

    private static ItemVenda CriarItem(decimal quantidade = 2m, decimal preco = 50m) =>
        new("INT-001", quantidade, preco);

    [Fact]
    public void CriarVenda_ComDescontoValido_DeveCalcularTotalLiquido()
    {
        var venda = Venda.CriarAvista(
            1,
            "VND-001",
            DataVenda,
            [CriarItem(2m, 100m)],
            descontoPercentual: 10m);

        Assert.Equal(200m, venda.TotalBruto);
        Assert.Equal(20m, venda.ValorDesconto);
        Assert.Equal(180m, venda.TotalLiquido);
        Assert.Equal(TipoVenda.AVista, venda.Tipo);
        Assert.Equal(StatusVenda.Confirmada, venda.Status);
    }

    [Fact]
    public void CriarVenda_DescontoAcimaDeVintePorCento_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() =>
            Venda.CriarAvista(1, "VND-002", DataVenda, [CriarItem()], descontoPercentual: 20.01m));
    }

    [Fact]
    public void CriarVenda_SemItens_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() =>
            Venda.CriarAvista(1, "VND-003", DataVenda, []));
    }

    [Fact]
    public void ValidarPrazoDevolucao_AposSeteDias_DeveLancarExcecao()
    {
        var venda = Venda.CriarAvista(1, "VND-004", DataVenda, [CriarItem()]);
        var dataDevolucao = DataVenda.AddDays(8);

        Assert.Throws<InvalidOperationException>(() => venda.ValidarPrazoDevolucao(dataDevolucao));
    }

    [Fact]
    public void CriarVenda_ComUsuarioOpcional_DevePermitirNulo()
    {
        var venda = Venda.CriarAvista(1, "VND-005", DataVenda, [CriarItem()]);

        Assert.Null(venda.UsuarioId);
    }
}

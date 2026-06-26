using LojaTecidos.Domain.Entities;
using LojaTecidos.Domain.Entities.Enum;
using LojaTecidos.Domain.ValueObjects;

namespace LojaTecidos.Domain.Tests.Entities;

public class ContaFiadoTests
{
    private static readonly DateTime DataEmissao = new(2026, 6, 1);
    private static readonly DateTime DataVencimento = new(2026, 6, 20);

    [Fact]
    public void CriarContaFiado_VencimentoSuperiorATrintaDiasDaEmissao_DeveLancarExcecao()
    {
        var dataVencimentoInvalida = DataEmissao.AddDays(31);

        Assert.Throws<InvalidOperationException>(() =>
            new ContaFiado(DataEmissao, dataVencimentoInvalida, 100m));
    }

    [Fact]
    public void RegistrarPagamento_ValorParcial_DeveReduzirSaldoDevedor()
    {
        var conta = new ContaFiado(DataEmissao, DataVencimento, 100m);
        var dataPagamento = new DateTime(2026, 6, 10);
        var novaDataVencimento = new DateTime(2026, 7, 5);

        conta.RegistrarPagamento(40m, dataPagamento, novaDataVencimento);

        Assert.Equal(60m, conta.SaldoDevedor);
        Assert.Equal(novaDataVencimento, conta.DataVencimento);
        Assert.Equal(StatusContaFiado.Ativa, conta.Status);
    }

    [Fact]
    public void RegistrarPagamento_ParcialSemNovaDataVencimento_DeveLancarExcecao()
    {
        var conta = new ContaFiado(DataEmissao, DataVencimento, 100m);
        var dataPagamento = new DateTime(2026, 6, 10);

        Assert.Throws<InvalidOperationException>(() =>
            conta.RegistrarPagamento(40m, dataPagamento));
    }

    [Fact]
    public void RegistrarPagamento_NovaDataSuperiorATrintaDiasDoPagamento_DeveLancarExcecao()
    {
        var conta = new ContaFiado(DataEmissao, DataVencimento, 100m);
        var dataPagamento = new DateTime(2026, 6, 10);
        var novaDataInvalida = dataPagamento.AddDays(31);

        Assert.Throws<InvalidOperationException>(() =>
            conta.RegistrarPagamento(40m, dataPagamento, novaDataInvalida));
    }

    [Fact]
    public void RegistrarPagamento_ValorMaiorQueDivida_DeveLancarExcecao()
    {
        var conta = new ContaFiado(DataEmissao, DataVencimento, 100m);

        Assert.Throws<InvalidOperationException>(() =>
            conta.RegistrarPagamento(120m, new DateTime(2026, 6, 10)));
    }

    [Fact]
    public void RegistrarPagamento_ValorExatoDaDivida_DeveEncerrarConta()
    {
        var conta = new ContaFiado(DataEmissao, DataVencimento, 100m);

        conta.RegistrarPagamento(100m, new DateTime(2026, 6, 10));

        Assert.Equal(0m, conta.SaldoDevedor);
        Assert.Equal(StatusContaFiado.Quitada, conta.Status);
        Assert.False(conta.EstaAtiva);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public void RegistrarPagamento_ValorZeroOuNegativo_DeveLancarExcecao(decimal valorInvalido)
    {
        var conta = new ContaFiado(DataEmissao, DataVencimento, 100m);

        Assert.Throws<ArgumentException>(() =>
            conta.RegistrarPagamento(valorInvalido, new DateTime(2026, 6, 10)));
    }

    [Fact]
    public void AdicionarCompra_ContaQuitada_DeveLancarExcecao()
    {
        var conta = new ContaFiado(DataEmissao, DataVencimento, 100m);
        conta.RegistrarPagamento(100m, new DateTime(2026, 6, 10));

        Assert.Throws<InvalidOperationException>(() => conta.AdicionarCompra(50m));
    }
}

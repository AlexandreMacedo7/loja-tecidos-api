using LojaTecidos.Domain.Entities;
using LojaTecidos.Domain.Entities.Enum;
using LojaTecidos.Domain.ValueObjects;

namespace LojaTecidos.Domain.Tests.Entities;

public class ClienteTests
{
    private static readonly DateTime DataEmissao = new(2026, 6, 1);
    private static readonly DateTime DataVencimento = new(2026, 6, 20);

    private static Cliente CriarCliente(
        CategoriaPerfil categoria = CategoriaPerfil.BRONZE,
        string? cpf = null,
        string? cnpj = null)
    {
        return new Cliente(
            "Maria Silva",
            "92999999999",
            new Endereco("Rua A", "10", "Centro"),
            cpf,
            cnpj,
            new PerfilCredito(categoria));
    }

    [Fact]
    public void CriarCliente_SemInformarPerfil_DeveUsarBronzeComoPadrao()
    {
        var cliente = new Cliente(
            "João",
            "92988888888",
            new Endereco("Rua B", "20", "Bairro"));

        Assert.Equal(CategoriaPerfil.BRONZE, cliente.PerfilCredito.Categoria);
        Assert.Equal(150m, cliente.PerfilCredito.Limite);
    }

    [Fact]
    public void CriarCliente_ComCpfECnpj_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() =>
            CriarCliente(cpf: "12345678901", cnpj: "12345678000199"));
    }

    [Fact]
    public void CriarCliente_SemDocumento_DevePermitirCadastro()
    {
        var cliente = CriarCliente();

        Assert.Null(cliente.Cpf);
        Assert.Null(cliente.Cnpj);
    }

    [Fact]
    public void RegistrarCompraFiado_ClienteBloqueado_DeveLancarExcecao()
    {
        var cliente = CriarCliente();
        cliente.Bloquear();

        Assert.Throws<InvalidOperationException>(() =>
            cliente.RegistrarCompraFiado(100m, DataEmissao, DataVencimento));
    }

    [Fact]
    public void RegistrarCompraFiado_PrimeiraCompraDentroDoLimite_DeveAbrirContaAtiva()
    {
        var cliente = CriarCliente();

        cliente.RegistrarCompraFiado(130m, DataEmissao, DataVencimento);

        Assert.NotNull(cliente.ContaFiadoAtiva);
        Assert.Equal(130m, cliente.ContaFiadoAtiva.SaldoDevedor);
    }

    [Theory]
    [InlineData(CategoriaPerfil.BRONZE, 150.01)]
    [InlineData(CategoriaPerfil.PRATA, 300.01)]
    [InlineData(CategoriaPerfil.OURO, 500.01)]
    public void RegistrarCompraFiado_ValorMaiorQueLimiteDoPerfil_DeveLancarExcecao(
        CategoriaPerfil categoria,
        decimal valorCompra)
    {
        var cliente = CriarCliente(categoria);

        Assert.Throws<InvalidOperationException>(() =>
            cliente.RegistrarCompraFiado(valorCompra, DataEmissao, DataVencimento));
    }

    [Fact]
    public void RegistrarCompraFiado_ComDividaESomaUltrapassaLimite_DeveLancarExcecao()
    {
        var cliente = CriarCliente();
        cliente.RegistrarCompraFiado(130m, DataEmissao, DataVencimento);

        Assert.Throws<InvalidOperationException>(() =>
            cliente.RegistrarCompraFiado(30m, DataEmissao, DataVencimento));
    }

    [Fact]
    public void RegistrarCompraFiado_ComDividaESomaDentroDoLimite_DeveAdicionarComSucesso()
    {
        var cliente = CriarCliente();
        cliente.RegistrarCompraFiado(100m, DataEmissao, DataVencimento);

        cliente.RegistrarCompraFiado(50m, DataEmissao, DataVencimento);

        Assert.Equal(150m, cliente.ContaFiadoAtiva!.SaldoDevedor);
    }

    [Fact]
    public void RegistrarPagamentoFiado_PagamentoParcial_DevePermitirNovaCompraDentroDoLimite()
    {
        var cliente = CriarCliente();
        cliente.RegistrarCompraFiado(100m, DataEmissao, DataVencimento);

        cliente.RegistrarPagamentoFiado(
            50m,
            new DateTime(2026, 6, 10),
            new DateTime(2026, 7, 5));

        cliente.RegistrarCompraFiado(100m, DataEmissao, DataVencimento);

        Assert.Equal(150m, cliente.ContaFiadoAtiva!.SaldoDevedor);
    }

    [Fact]
    public void RegistrarPagamentoFiado_QuitacaoTotal_DeveEncerrarContaEPermitirNovaCompra()
    {
        var cliente = CriarCliente();
        cliente.RegistrarCompraFiado(100m, DataEmissao, DataVencimento);
        var contaAnterior = cliente.ContaFiadoAtiva;

        cliente.RegistrarPagamentoFiado(100m, new DateTime(2026, 6, 10));

        Assert.Null(cliente.ContaFiadoAtiva);
        Assert.Single(cliente.ContasQuitadas);
        Assert.Same(contaAnterior, cliente.ContasQuitadas.First());
        Assert.Equal(StatusContaFiado.Quitada, contaAnterior!.Status);

        cliente.RegistrarCompraFiado(80m, DataEmissao, DataVencimento);

        Assert.NotNull(cliente.ContaFiadoAtiva);
        Assert.NotSame(contaAnterior, cliente.ContaFiadoAtiva);
        Assert.Equal(80m, cliente.ContaFiadoAtiva.SaldoDevedor);
    }

    [Fact]
    public void CriarCliente_ComCpfFormatado_DeveNormalizarParaDigitos()
    {
        var cliente = CriarCliente(cpf: "055.040.752-49");

        Assert.Equal("05504075249", cliente.Cpf);
    }

    [Fact]
    public void CriarCliente_ComCpfInvalido_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() => CriarCliente(cpf: "123"));
    }

    [Fact]
    public void AlterarPerfil_Manualmente_DeveAtualizarLimite()
    {
        var cliente = CriarCliente();

        cliente.AlterarPerfil(CategoriaPerfil.OURO);

        Assert.Equal(CategoriaPerfil.OURO, cliente.PerfilCredito.Categoria);
        Assert.Equal(500m, cliente.PerfilCredito.Limite);
    }
}

using LojaTecidos.Domain.Entities;
using LojaTecidos.Domain.Entities.Enum;
using LojaTecidos.Domain.Services;
using LojaTecidos.Domain.ValueObjects;

namespace LojaTecidos.Domain.Tests.Services;

public class ServicoVendaTests
{
    private static readonly DateTime DataVenda = new(2026, 6, 26);
    private static readonly DateTime DataVencimento = new(2026, 7, 10);
    private static readonly ServicoVenda Servico = new();

    private static Produto CriarProduto(
        string codigo = "INT-001",
        decimal estoque = 100m,
        decimal preco = 25m)
    {
        return new Produto(
            codigo,
            "Tecido teste",
            new Fornecedor("Janon"),
            CategoriaProduto.Tecido,
            UnidadeMedida.Metro,
            preco,
            estoque,
            new DateTime(2026, 1, 1));
    }

    private static Cliente CriarCliente() =>
        new(
            "Maria",
            "92999999999",
            new Endereco("Rua A", "10", "Centro"));

    private static Dictionary<string, Produto> CriarCatalogo(params Produto[] produtos) =>
        produtos.ToDictionary(p => p.CodigoInterno);

    [Fact]
    public void RegistrarVendaAvista_DeveBaixarEstoque()
    {
        var produto = CriarProduto();
        var produtos = CriarCatalogo(produto);
        var itens = new[] { ItemVenda.Criar(produto, 10m) };

        Servico.RegistrarVendaAvista(1, "VND-100", DataVenda, itens, produtos);

        Assert.Equal(90m, produto.QuantidadeAtual);
    }

    [Fact]
    public void RegistrarVendaAvista_MultiplosItens_DeveBaixarEstoqueDeCadaProduto()
    {
        var tecido = CriarProduto("INT-001", 100m, 20m);
        var toalha = new Produto(
            "INT-002",
            "Toalha",
            new Fornecedor("Malha Rio"),
            CategoriaProduto.Toalha,
            UnidadeMedida.Unidade,
            35m,
            50m,
            new DateTime(2026, 1, 1));

        var produtos = CriarCatalogo(tecido, toalha);
        var itens = new[]
        {
            ItemVenda.Criar(tecido, 5m),
            ItemVenda.Criar(toalha, 2m)
        };

        var venda = Servico.RegistrarVendaAvista(2, "VND-101", DataVenda, itens, produtos, descontoPercentual: 10m);

        Assert.Equal(95m, tecido.QuantidadeAtual);
        Assert.Equal(48m, toalha.QuantidadeAtual);
        Assert.Equal(170m, venda.TotalBruto);
        Assert.Equal(153m, venda.TotalLiquido);
    }

    [Fact]
    public void RegistrarVendaFiado_DeveBaixarEstoqueERegistrarDivida()
    {
        var produto = CriarProduto(preco: 50m);
        var cliente = CriarCliente();
        var produtos = CriarCatalogo(produto);
        var itens = new[] { ItemVenda.Criar(produto, 2m) };

        var venda = Servico.RegistrarVendaFiado(
            3,
            "VND-102",
            DataVenda,
            DataVencimento,
            cliente,
            itens,
            produtos);

        Assert.Equal(TipoVenda.Fiado, venda.Tipo);
        Assert.Equal(98m, produto.QuantidadeAtual);
        Assert.Equal(100m, cliente.ContaFiadoAtiva!.SaldoDevedor);
    }

    [Fact]
    public void RegistrarVendaFiado_TotalAcimaDoLimite_DeveLancarExcecao()
    {
        var produto = CriarProduto(preco: 100m);
        var cliente = CriarCliente();
        var produtos = CriarCatalogo(produto);
        var itens = new[] { ItemVenda.Criar(produto, 2m) };

        Assert.Throws<InvalidOperationException>(() =>
            Servico.RegistrarVendaFiado(
                4,
                "VND-103",
                DataVenda,
                DataVencimento,
                cliente,
                itens,
                produtos));
    }

    [Fact]
    public void RegistrarVendaFiado_ClienteBloqueado_DeveLancarExcecao()
    {
        var produto = CriarProduto(preco: 10m);
        var cliente = CriarCliente();
        cliente.Bloquear();
        var produtos = CriarCatalogo(produto);
        var itens = new[] { ItemVenda.Criar(produto, 1m) };

        Assert.Throws<InvalidOperationException>(() =>
            Servico.RegistrarVendaFiado(
                5,
                "VND-104",
                DataVenda,
                DataVencimento,
                cliente,
                itens,
                produtos));
    }

    [Fact]
    public void RegistrarDevolucao_DentroDoPrazo_DeveRestaurarEstoque()
    {
        var produto = CriarProduto();
        var produtos = CriarCatalogo(produto);
        var itens = new[] { ItemVenda.Criar(produto, 10m) };
        var venda = Servico.RegistrarVendaAvista(6, "VND-105", DataVenda, itens, produtos);

        Servico.RegistrarDevolucao(venda, DataVenda.AddDays(5), produtos);

        Assert.Equal(100m, produto.QuantidadeAtual);
        Assert.Equal(StatusVenda.Devolvida, venda.Status);
    }

    [Fact]
    public void RegistrarDevolucao_VendaFiado_DeveReduzirDividaERestaurarEstoque()
    {
        var produto = CriarProduto(preco: 50m);
        var cliente = CriarCliente();
        var produtos = CriarCatalogo(produto);
        var itens = new[] { ItemVenda.Criar(produto, 2m) };
        var venda = Servico.RegistrarVendaFiado(
            7,
            "VND-106",
            DataVenda,
            DataVencimento,
            cliente,
            itens,
            produtos);

        Servico.RegistrarDevolucao(venda, DataVenda.AddDays(3), produtos, cliente);

        Assert.Equal(100m, produto.QuantidadeAtual);
        Assert.Null(cliente.ContaFiadoAtiva);
        Assert.Single(cliente.ContasQuitadas);
    }

    [Fact]
    public void RegistrarDevolucao_ForaDoPrazo_DeveLancarExcecao()
    {
        var produto = CriarProduto();
        var produtos = CriarCatalogo(produto);
        var itens = new[] { ItemVenda.Criar(produto, 1m) };
        var venda = Servico.RegistrarVendaAvista(8, "VND-107", DataVenda, itens, produtos);

        Assert.Throws<InvalidOperationException>(() =>
            Servico.RegistrarDevolucao(venda, DataVenda.AddDays(8), produtos));
    }
}

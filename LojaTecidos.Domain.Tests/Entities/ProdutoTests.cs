using LojaTecidos.Domain.Entities;
using LojaTecidos.Domain.Entities.Enum;

namespace LojaTecidos.Domain.Tests.Entities;

public class ProdutoTests
{
    private static readonly DateTime DataCadastro = new(2026, 1, 10);
    private static readonly Fornecedor FornecedorJanon = new("Janon");

    private static Produto CriarTecido(
        decimal estoqueInicial = 100m,
        string? codigoFornecedor = "15784T",
        decimal preco = 25.90m)
    {
        return new Produto(
            "INT-001",
            "Oxford para cortina",
            FornecedorJanon,
            CategoriaProduto.Cortinado,
            UnidadeMedida.Metro,
            preco,
            estoqueInicial,
            DataCadastro,
            codigoFornecedor);
    }

    [Fact]
    public void CriarProduto_ComCodigoFornecedorOpcional_DevePermitirCadastroAvulso()
    {
        var produto = CriarTecido(codigoFornecedor: null);

        Assert.Null(produto.CodigoFornecedor);
    }

    [Fact]
    public void CriarProduto_CategoriaComUnidadeIncompativel_DeveLancarExcecao()
    {
        Assert.Throws<InvalidOperationException>(() =>
            new Produto(
                "INT-002",
                "Cetim branco",
                FornecedorJanon,
                CategoriaProduto.Tecido,
                UnidadeMedida.Unidade,
                10m,
                50m,
                DataCadastro));
    }

    [Fact]
    public void CriarProduto_ComEstoqueInicial_DeveDefinirReferenciaDeAlerta()
    {
        var produto = CriarTecido(estoqueInicial: 100m);

        Assert.Equal(100m, produto.QuantidadeAtual);
        Assert.Equal(100m, produto.QuantidadeReferenciaAlerta);
    }

    [Fact]
    public void RegistrarEntrada_DeveAumentarEstoqueSemAlterarReferenciaDeAlerta()
    {
        var produto = CriarTecido(estoqueInicial: 100m);

        produto.RegistrarEntrada(50m, new DateTime(2026, 2, 1));

        Assert.Equal(150m, produto.QuantidadeAtual);
        Assert.Equal(100m, produto.QuantidadeReferenciaAlerta);
    }

    [Fact]
    public void AtualizarEstoque_ParaCima_DeveAtualizarReferenciaDeAlerta()
    {
        var produto = CriarTecido(estoqueInicial: 100m);
        produto.RegistrarSaida(70m, new DateTime(2026, 6, 1));

        produto.AtualizarEstoque(80m, new DateTime(2026, 6, 20));

        Assert.Equal(80m, produto.QuantidadeAtual);
        Assert.Equal(80m, produto.QuantidadeReferenciaAlerta);
    }

    [Fact]
    public void AtualizarEstoque_ParaBaixo_NaoDeveAlterarReferenciaDeAlerta()
    {
        var produto = CriarTecido(estoqueInicial: 100m);

        produto.AtualizarEstoque(30m, new DateTime(2026, 6, 20));

        Assert.Equal(30m, produto.QuantidadeAtual);
        Assert.Equal(100m, produto.QuantidadeReferenciaAlerta);
    }

    [Fact]
    public void RegistrarSaida_QuantidadeMaiorQueEstoque_DeveLancarExcecao()
    {
        var produto = CriarTecido(estoqueInicial: 20m);

        Assert.Throws<InvalidOperationException>(() =>
            produto.RegistrarSaida(25m, new DateTime(2026, 3, 1)));
    }

    [Fact]
    public void EstaProximoDoFim_EstoqueAteVintePorCentoDaReferencia_DeveRetornarVerdadeiro()
    {
        var produto = CriarTecido(estoqueInicial: 100m);
        produto.AtualizarEstoque(80m, new DateTime(2026, 6, 20));

        produto.RegistrarSaida(64m, new DateTime(2026, 6, 21));

        Assert.Equal(16m, produto.QuantidadeAtual);
        Assert.True(produto.EstaProximoDoFim());
    }

    [Fact]
    public void EstaProximoDoFim_EstoqueAcimaDeVintePorCento_DeveRetornarFalso()
    {
        var produto = CriarTecido(estoqueInicial: 100m);

        produto.RegistrarSaida(50m, new DateTime(2026, 3, 1));

        Assert.False(produto.EstaProximoDoFim());
    }

    [Fact]
    public void EstaParado_SemVendaUsaUltimaEntrada_DeveRetornarVerdadeiroAposQuarentaECincoDias()
    {
        var produto = CriarTecido(estoqueInicial: 100m);
        produto.RegistrarEntrada(20m, new DateTime(2026, 1, 20));

        var dataReferencia = new DateTime(2026, 3, 7);

        Assert.True(produto.EstaParado(dataReferencia));
    }

    [Fact]
    public void EstaParado_ComVendaUsaDataDaUltimaVenda_DeveRetornarFalsoAntesDeQuarentaECincoDias()
    {
        var produto = CriarTecido(estoqueInicial: 100m);
        produto.RegistrarSaida(10m, new DateTime(2026, 5, 1));

        var dataReferencia = new DateTime(2026, 6, 1);

        Assert.False(produto.EstaParado(dataReferencia));
    }

    [Fact]
    public void EstaParado_SemVendaNemEntradaUsaDataDeCadastro()
    {
        var produto = new Produto(
            "INT-003",
            "Malha PV",
            new Fornecedor("Malha Rio"),
            CategoriaProduto.Tecido,
            UnidadeMedida.Metro,
            15m,
            0m,
            new DateTime(2026, 1, 1));

        Assert.True(produto.EstaParado(new DateTime(2026, 2, 20)));
    }

    [Fact]
    public void Descontinuar_DeveMarcarProdutoComoDescontinuado()
    {
        var produto = CriarTecido();

        produto.Descontinuar();

        Assert.True(produto.Descontinuado);
        Assert.Equal(100m, produto.QuantidadeAtual);
    }

    [Fact]
    public void CriarProduto_Travesseiro_DeveUsarUnidadePar()
    {
        var produto = new Produto(
            "INT-004",
            "Travesseiro fibra",
            new Fornecedor("Avulso Manaus"),
            CategoriaProduto.Travesseiro,
            UnidadeMedida.Par,
            45m,
            10m,
            DataCadastro);

        Assert.Equal(UnidadeMedida.Par, produto.UnidadeMedida);
    }
}

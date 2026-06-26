using LojaTecidos.Domain.Entities;
using LojaTecidos.Domain.Entities.Enum;

namespace LojaTecidos.Domain.Tests.Entities;

public class RegraUnidadeMedidaProdutoTests
{
    [Theory]
    [InlineData(CategoriaProduto.Tecido, UnidadeMedida.Metro)]
    [InlineData(CategoriaProduto.Cortinado, UnidadeMedida.Metro)]
    [InlineData(CategoriaProduto.Travesseiro, UnidadeMedida.Par)]
    [InlineData(CategoriaProduto.Toalha, UnidadeMedida.Unidade)]
    public void ObterUnidadeEsperada_DeveRetornarUnidadeDaCategoria(
        CategoriaProduto categoria,
        UnidadeMedida unidadeEsperada)
    {
        Assert.Equal(unidadeEsperada, RegraUnidadeMedidaProduto.ObterUnidadeEsperada(categoria));
    }

    [Fact]
    public void Validar_CategoriaComUnidadeIncorreta_DeveLancarExcecao()
    {
        Assert.Throws<InvalidOperationException>(() =>
            RegraUnidadeMedidaProduto.Validar(CategoriaProduto.Tecido, UnidadeMedida.Unidade));
    }
}

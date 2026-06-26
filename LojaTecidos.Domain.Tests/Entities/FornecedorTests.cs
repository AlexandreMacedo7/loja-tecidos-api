using LojaTecidos.Domain.Entities;
using LojaTecidos.Domain.Entities.Enum;

namespace LojaTecidos.Domain.Tests.Entities;

public class FornecedorTests
{
    [Fact]
    public void CriarFornecedor_NomeValido_DeveCriarComSucesso()
    {
        var fornecedor = new Fornecedor("Malha Rio");

        Assert.Equal("Malha Rio", fornecedor.Nome);
    }

    [Fact]
    public void CriarFornecedor_NomeVazio_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() => new Fornecedor(""));
    }
}

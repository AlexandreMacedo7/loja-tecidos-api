using LojaTecidos.Domain.ValueObjects;

namespace LojaTecidos.Domain.Tests.ValueObjects;

public class EnderecoTests
{
    [Fact]
    public void CriarEndereco_SemInformarCep_DeveUsarCepPadrao()
    {
        var endereco = new Endereco("Rua das Flores", "100", "Centro");

        Assert.Equal(Endereco.CepPadrao, endereco.Cep);
    }

    [Theory]
    [InlineData("", "10", "Centro")]
    [InlineData("Rua A", "", "Centro")]
    [InlineData("Rua A", "10", "")]
    public void CriarEndereco_CampoObrigatorioVazio_DeveLancarExcecao(string rua, string numero, string bairro)
    {
        Assert.Throws<ArgumentException>(() => new Endereco(rua, numero, bairro));
    }
}

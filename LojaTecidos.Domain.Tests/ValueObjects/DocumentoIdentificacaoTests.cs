using LojaTecidos.Domain.ValueObjects;

namespace LojaTecidos.Domain.Tests.ValueObjects;

public class DocumentoIdentificacaoTests
{
    [Theory]
    [InlineData("055.040.752-49", "05504075249")]
    [InlineData("05504075249", "05504075249")]
    public void NormalizarCpf_ComEntradaValida_DeveRetornarSomenteDigitos(string entrada, string esperado)
    {
        Assert.Equal(esperado, DocumentoIdentificacao.NormalizarCpf(entrada));
    }

    [Theory]
    [InlineData("12.345.678/0001-99", "12345678000199")]
    public void NormalizarCnpj_ComEntradaValida_DeveRetornarSomenteDigitos(string entrada, string esperado)
    {
        Assert.Equal(esperado, DocumentoIdentificacao.NormalizarCnpj(entrada));
    }

    [Fact]
    public void NormalizarCpf_Vazio_DeveRetornarNull()
    {
        Assert.Null(DocumentoIdentificacao.NormalizarCpf(null));
        Assert.Null(DocumentoIdentificacao.NormalizarCpf("   "));
    }
}

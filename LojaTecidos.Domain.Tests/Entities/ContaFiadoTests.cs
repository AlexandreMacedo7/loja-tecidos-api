using LojaTecidos.Domain.Entities;

namespace LojaTecidos.Domain.Tests.Entities;

public class ContaFiadoTests
{
    
    [Fact]
    public void RegistrarPagamento_ValorParcial_DeveReduzirSaldoDevedor()
    {
        //Arrange
        var valorEsperado = 60.00m;
        var conta = new ContaFiado(new DateTime(2026,02,10), 100.00m);
        
        //Act
        conta.RegistrarPagamento(40.00m);
        
        //Assert
        Assert.Equal(valorEsperado, conta.SaldoDevedor);
    }

    [Fact]
    public void RegistrarPagamento_ValorMaiorQueDividida_DeveLancarExcesao()
    {
        //Arrange
        var conta = new ContaFiado(new DateTime(2026,02,10), 100.00m);

        //Act & Assert
        Assert.Throws<InvalidOperationException>(()=> conta.RegistrarPagamento(120.00m));
    }

    [Fact]
    public void RegistrarPagamento_ValorExatoDaDivida_DeveZerarSaldo()
    {
        //Arrange
        var conta = new ContaFiado(new DateTime(2026,02,10), 100.00m);
        //Act
        conta.RegistrarPagamento(100.00m);
        //Assert
        Assert.Equal(0m, conta.SaldoDevedor); 
    }

    [Theory]
    [InlineData(0.00)]
    [InlineData(-50.00)]
    public void RegistrarPagamento_ValorZeroOuNegativo_DeveLancarExcecao(decimal valorNegativoOuZero)
    {
        //Arrange
        var conta = new ContaFiado(new DateTime(2026,02,10), 100.00m);
        
        //Act & Assert
        Assert.Throws<ArgumentException>(() => conta.RegistrarPagamento(valorNegativoOuZero));

    }
}

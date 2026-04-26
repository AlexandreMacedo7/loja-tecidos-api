using LojaTecidos.Domain.Entities;
using LojaTecidos.Domain.Entities.Enum;
using System;
using Xunit;

namespace LojaTecidos.Domain.Tests.Entities
{
    public class ClienteTests
    {
        [Fact]
        public void AdicionarContaFiado_ClienteComDividaAtiva_DeveLancarExcecao()
        {
            // Arrange
            var cliente = new Cliente(new PerfilCredito(CategoriaPerfil.BRONZE));

            // simulando divida existente
            var contaPendente = new ContaFiado(new DateTime(2026, 02, 02), 130.00m);
            cliente.AdicionarConta(contaPendente);

            // simunlando nova conta
            var novaConta = new ContaFiado(new DateTime(2026, 05, 10), 150.00m);

            // Act & Assert 
            Assert.Throws<InvalidOperationException>(() => cliente.AdicionarConta(novaConta));
        }

        [Fact]
        public void AdicionarContaFiado_ClienteSemDivida_DeveAdicionarComSucesso()
        {
            //Arrange
            var cliente = new Cliente(new PerfilCredito(CategoriaPerfil.BRONZE));
            var contaFiado = new ContaFiado(new DateTime(2026,05,10), 130.00m);

            //Act
            cliente.AdicionarConta(contaFiado);

            //Assert
            Assert.Single(cliente.Contas);
        }
        [Theory]//adicionar theore e inline data
        [InlineData(CategoriaPerfil.BRONZE, 150.01)]
        [InlineData(CategoriaPerfil.PRATA, 350.01)]
        [InlineData(CategoriaPerfil.OURO, 500.01)]
        public void AdicionarContaFiado_ValorMaiorQueLimiteDoPerfil_DeveLancarExcecao(CategoriaPerfil categoriaPerfil, decimal valorConta)
        {
            //Arrange
            var perfilCredito = new PerfilCredito(categoriaPerfil);
            var cliente = new Cliente(perfilCredito);
            var contaFiado = new ContaFiado(new DateTime(2026, 05, 10), valorConta);
            
            //Act & Asset
            Assert.Throws<InvalidOperationException>( ()=> cliente.AdicionarConta(contaFiado));
            
        }

        //Cenario de data muito longa para pagamento
    }
}
using LojaTecidos.Domain.Entities;
using LojaTecidos.Domain.Entities.Enum;
using LojaTecidos.Domain.Services;

namespace LojaTecidos.Infrastructure.Tests.Persistence;

public class VendaRepositoryTests : RepositoryTestBase
{
    [Fact]
    public async Task RegistrarVendaAvista_DevePersistirVendaEBaixarEstoque()
    {
        var produto = new Produto(
            "INT-010",
            "Oxford",
            new Fornecedor("Janon"),
            CategoriaProduto.Tecido,
            UnidadeMedida.Metro,
            20m,
            50m,
            new DateTime(2026, 1, 1));

        await ProdutoRepository.AdicionarAsync(produto);
        await UnitOfWork.SaveChangesAsync();

        var servicoVenda = new ServicoVenda();
        var itens = new[] { ItemVenda.Criar(produto, 5m) };
        var produtos = new Dictionary<string, Produto> { [produto.CodigoInterno] = produto };
        var venda = servicoVenda.RegistrarVendaAvista(
            1,
            "VND-900",
            new DateTime(2026, 6, 26),
            itens,
            produtos);

        await VendaRepository.AdicionarAsync(venda, null);
        await ProdutoRepository.AtualizarAsync(produto);
        await UnitOfWork.SaveChangesAsync();

        var vendaCarregada = await VendaRepository.ObterPorCodigoAsync("VND-900");
        var produtoCarregado = await ProdutoRepository.ObterPorCodigoInternoAsync("INT-010");

        Assert.NotNull(vendaCarregada);
        Assert.Equal(100m, vendaCarregada.TotalBruto);
        Assert.NotNull(produtoCarregado);
        Assert.Equal(45m, produtoCarregado.QuantidadeAtual);
    }
}

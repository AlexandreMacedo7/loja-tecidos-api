using LojaTecidos.Application.Abstractions.Persistence;
using LojaTecidos.Domain.Entities;
using LojaTecidos.Domain.Exceptions;

namespace LojaTecidos.Application.Vendas;

internal static class VendaProdutoHelper
{
    public static List<ItemVenda> CriarItensVenda(
        IReadOnlyList<ItemVendaRequest> itensRequest,
        IReadOnlyDictionary<string, Produto> produtos) =>
        itensRequest
            .Select(item => ItemVenda.Criar(produtos[item.CodigoInternoProduto.Trim()], item.Quantidade))
            .ToList();

    public static async Task<Dictionary<string, Produto>> CarregarProdutosAsync(
        IProdutoRepository produtoRepository,
        IReadOnlyList<ItemVendaRequest> itens,
        CancellationToken cancellationToken)
    {
        var codigos = itens.Select(i => i.CodigoInternoProduto.Trim()).Distinct().ToList();
        var produtos = await produtoRepository.ObterPorCodigosInternosAsync(codigos, cancellationToken);

        if (produtos.Count != codigos.Count)
        {
            var encontrados = produtos.Select(p => p.CodigoInterno).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var faltante = codigos.First(c => !encontrados.Contains(c));
            throw new EntidadeNaoEncontradaException($"Produto {faltante} não encontrado.");
        }

        return produtos.ToDictionary(p => p.CodigoInterno, StringComparer.OrdinalIgnoreCase);
    }
}

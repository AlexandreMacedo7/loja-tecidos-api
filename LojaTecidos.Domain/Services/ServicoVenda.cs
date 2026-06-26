using LojaTecidos.Domain.Entities;
using LojaTecidos.Domain.Entities.Enum;
using LojaTecidos.Domain.Exceptions;

namespace LojaTecidos.Domain.Services;

public class ServicoVenda
{
    public Venda RegistrarVendaAvista(
        int numeroSequencial,
        string codigoVenda,
        DateTime dataVenda,
        IEnumerable<ItemVenda> itens,
        IReadOnlyDictionary<string, Produto> produtos,
        decimal descontoPercentual = 0,
        string? usuarioId = null)
    {
        var venda = Venda.CriarAvista(
            numeroSequencial,
            codigoVenda,
            dataVenda,
            itens,
            descontoPercentual,
            usuarioId);

        BaixarEstoque(venda, produtos, dataVenda);
        return venda;
    }

    public Venda RegistrarVendaFiado(
        int numeroSequencial,
        string codigoVenda,
        DateTime dataVenda,
        DateTime dataVencimento,
        Cliente cliente,
        IEnumerable<ItemVenda> itens,
        IReadOnlyDictionary<string, Produto> produtos,
        decimal descontoPercentual = 0,
        string? usuarioId = null)
    {
        ArgumentNullException.ThrowIfNull(cliente);

        var venda = Venda.CriarFiado(
            numeroSequencial,
            codigoVenda,
            dataVenda,
            itens,
            descontoPercentual,
            usuarioId);

        BaixarEstoque(venda, produtos, dataVenda);
        cliente.RegistrarCompraFiado(venda.TotalLiquido, dataVenda, dataVencimento);

        return venda;
    }

    public void RegistrarDevolucao(
        Venda venda,
        DateTime dataDevolucao,
        IReadOnlyDictionary<string, Produto> produtos,
        Cliente? cliente = null)
    {
        ArgumentNullException.ThrowIfNull(venda);

        venda.ValidarPrazoDevolucao(dataDevolucao);

        if (venda.Tipo == TipoVenda.Fiado)
        {
            if (cliente is null)
                throw new ArgumentException("Cliente é obrigatório para devolução de venda a fiado.", nameof(cliente));

            cliente.RegistrarPagamentoFiado(venda.TotalLiquido, dataDevolucao);
        }

        foreach (var item in venda.Itens)
        {
            var produto = ObterProduto(produtos, item.CodigoInternoProduto);
            produto.RegistrarEntrada(item.Quantidade, dataDevolucao);
        }

        venda.MarcarComoDevolvida();
    }

    private static void BaixarEstoque(Venda venda, IReadOnlyDictionary<string, Produto> produtos, DateTime dataVenda)
    {
        foreach (var item in venda.Itens)
        {
            var produto = ObterProduto(produtos, item.CodigoInternoProduto);
            produto.RegistrarSaida(item.Quantidade, dataVenda);
        }
    }

    private static Produto ObterProduto(IReadOnlyDictionary<string, Produto> produtos, string codigoInterno)
    {
        if (!produtos.TryGetValue(codigoInterno, out var produto))
            throw new EntidadeNaoEncontradaException($"Produto {codigoInterno} não encontrado.");

        return produto;
    }
}

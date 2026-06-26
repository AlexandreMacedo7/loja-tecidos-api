namespace LojaTecidos.Domain.Entities;

public class ItemVenda
{
    public string CodigoInternoProduto { get; }
    public decimal Quantidade { get; }
    public decimal PrecoUnitario { get; }
    public decimal Subtotal => decimal.Round(Quantidade * PrecoUnitario, 2);

    public ItemVenda(string codigoInternoProduto, decimal quantidade, decimal precoUnitario)
    {
        if (string.IsNullOrWhiteSpace(codigoInternoProduto))
            throw new ArgumentException("Código interno do produto é obrigatório.", nameof(codigoInternoProduto));

        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantidade));

        if (precoUnitario <= 0)
            throw new ArgumentException("Preço unitário deve ser maior que zero.", nameof(precoUnitario));

        CodigoInternoProduto = codigoInternoProduto.Trim();
        Quantidade = decimal.Round(quantidade, 2);
        PrecoUnitario = decimal.Round(precoUnitario, 2);
    }

    public static ItemVenda Criar(Produto produto, decimal quantidade) =>
        new(produto.CodigoInterno, quantidade, produto.PrecoUnitario);
}

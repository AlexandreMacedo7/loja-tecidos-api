using LojaTecidos.Application.Vendas;

namespace LojaTecidos.Application.Common.Validation;

public static class VendaRequestValidator
{
    public static void ValidarUsuarioId(string? usuarioId)
    {
        if (string.IsNullOrWhiteSpace(usuarioId))
            throw new ArgumentException("Usuário autenticado não identificado.");
    }

    public static void ValidarItens(IReadOnlyList<ItemVendaRequest>? itens)
    {
        if (itens is null || itens.Count == 0)
            throw new ArgumentException("A venda deve conter ao menos um item.");

        foreach (var item in itens)
        {
            if (string.IsNullOrWhiteSpace(item.CodigoInternoProduto))
                throw new ArgumentException("Código interno do produto é obrigatório em todos os itens.");

            if (item.Quantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero em todos os itens.");
        }
    }

    public static void ValidarDesconto(decimal descontoPercentual)
    {
        if (descontoPercentual is < 0 or > 100)
            throw new ArgumentException("Desconto percentual deve estar entre 0 e 100.");
    }
}

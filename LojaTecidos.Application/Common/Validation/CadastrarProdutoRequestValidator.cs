using LojaTecidos.Application.Produtos;

namespace LojaTecidos.Application.Common.Validation;

public static class CadastrarProdutoRequestValidator
{
    public static void Validar(CadastrarProdutoRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new ArgumentException("Nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.NomeFornecedor))
            throw new ArgumentException("Nome do fornecedor é obrigatório.");

        if (request.PrecoUnitario <= 0)
            throw new ArgumentException("Preço unitário deve ser maior que zero.");

        if (request.EstoqueInicial < 0)
            throw new ArgumentException("Estoque inicial não pode ser negativo.");
    }
}

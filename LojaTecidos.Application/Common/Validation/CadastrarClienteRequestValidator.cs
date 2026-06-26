using LojaTecidos.Application.Clientes;

namespace LojaTecidos.Application.Common.Validation;

public static class CadastrarClienteRequestValidator
{
    public static void Validar(CadastrarClienteRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new ArgumentException("Nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.Telefone))
            throw new ArgumentException("Telefone é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.Rua))
            throw new ArgumentException("Rua é obrigatória.");

        if (string.IsNullOrWhiteSpace(request.Numero))
            throw new ArgumentException("Número é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.Bairro))
            throw new ArgumentException("Bairro é obrigatório.");
    }
}

namespace LojaTecidos.Domain.Entities;

public class Fornecedor
{
    public string Nome { get; }

    public Fornecedor(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do fornecedor é obrigatório.", nameof(nome));

        Nome = nome.Trim();
    }
}

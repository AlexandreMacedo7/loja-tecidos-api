namespace LojaTecidos.Domain.Entities;

public class Fornecedor
{
    public Guid Id { get; }
    public string Nome { get; }

    public Fornecedor(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do fornecedor é obrigatório.", nameof(nome));

        Id = Guid.NewGuid();
        Nome = nome.Trim();
    }

    /// <summary>
    /// Reidrata o fornecedor a partir da persistência. Uso exclusivo da camada Infrastructure.
    /// </summary>
    internal static Fornecedor Reconstituir(Guid id, string nome) =>
        new(id, nome);

    private Fornecedor(Guid id, string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do fornecedor é obrigatório.", nameof(nome));

        Id = id;
        Nome = nome.Trim();
    }
}

namespace LojaTecidos.Infrastructure.Persistence.Entities;

public class FornecedorEntity
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public ICollection<ProdutoEntity> Produtos { get; set; } = [];
}

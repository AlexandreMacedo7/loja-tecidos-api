using LojaTecidos.Domain.Entities.Enum;

namespace LojaTecidos.Infrastructure.Persistence.Entities;

public class ProdutoEntity
{
    public Guid Id { get; set; }
    public string CodigoInterno { get; set; } = string.Empty;
    public string? CodigoFornecedor { get; set; }
    public string Nome { get; set; } = string.Empty;
    public Guid FornecedorId { get; set; }
    public FornecedorEntity Fornecedor { get; set; } = null!;
    public CategoriaProduto Categoria { get; set; }
    public UnidadeMedida UnidadeMedida { get; set; }
    public decimal PrecoUnitario { get; set; }
    public bool Descontinuado { get; set; }
    public decimal QuantidadeAtual { get; set; }
    public decimal QuantidadeReferenciaAlerta { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime? DataUltimaVenda { get; set; }
    public DateTime? DataUltimaEntrada { get; set; }
    public ICollection<MovimentacaoEstoqueEntity> Movimentacoes { get; set; } = [];
}

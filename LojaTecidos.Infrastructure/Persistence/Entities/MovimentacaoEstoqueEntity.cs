using LojaTecidos.Domain.Entities.Enum;

namespace LojaTecidos.Infrastructure.Persistence.Entities;

public class MovimentacaoEstoqueEntity
{
    public Guid Id { get; set; }
    public Guid ProdutoId { get; set; }
    public ProdutoEntity Produto { get; set; } = null!;
    public TipoMovimentacaoEstoque Tipo { get; set; }
    public decimal Quantidade { get; set; }
    public decimal SaldoAposMovimentacao { get; set; }
    public DateTime Data { get; set; }
}

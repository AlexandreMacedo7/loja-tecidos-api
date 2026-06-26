using LojaTecidos.Domain.Entities.Enum;

namespace LojaTecidos.Infrastructure.Persistence.Entities;

public class ContaFiadoEntity
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public ClienteEntity Cliente { get; set; } = null!;
    public DateTime DataEmissao { get; set; }
    public DateTime DataVencimento { get; set; }
    public decimal SaldoDevedor { get; set; }
    public StatusContaFiado Status { get; set; }
}

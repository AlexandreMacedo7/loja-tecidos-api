using LojaTecidos.Domain.Entities.Enum;

namespace LojaTecidos.Infrastructure.Persistence.Entities;

public class VendaEntity
{
    public Guid Id { get; set; }
    public int NumeroSequencial { get; set; }
    public string CodigoVenda { get; set; } = string.Empty;
    public DateTime DataVenda { get; set; }
    public TipoVenda Tipo { get; set; }
    public StatusVenda Status { get; set; }
    public decimal DescontoPercentual { get; set; }
    public string? UsuarioId { get; set; }
    public Guid? ClienteId { get; set; }
    public ClienteEntity? Cliente { get; set; }
    public ICollection<ItemVendaEntity> Itens { get; set; } = [];
}

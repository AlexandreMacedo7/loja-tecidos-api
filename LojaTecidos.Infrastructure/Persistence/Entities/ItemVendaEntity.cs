namespace LojaTecidos.Infrastructure.Persistence.Entities;

public class ItemVendaEntity
{
    public Guid Id { get; set; }
    public Guid VendaId { get; set; }
    public VendaEntity Venda { get; set; } = null!;
    public string CodigoInternoProduto { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}

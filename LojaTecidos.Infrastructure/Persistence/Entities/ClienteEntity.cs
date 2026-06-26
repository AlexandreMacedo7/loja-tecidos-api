using LojaTecidos.Domain.Entities.Enum;

namespace LojaTecidos.Infrastructure.Persistence.Entities;

public class ClienteEntity
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Rua { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string? Cpf { get; set; }
    public string? Cnpj { get; set; }
    public CategoriaPerfil CategoriaPerfil { get; set; }
    public bool Bloqueado { get; set; }
    public ICollection<ContaFiadoEntity> ContasFiado { get; set; } = [];
    public ICollection<VendaEntity> Vendas { get; set; } = [];
}

using LojaTecidos.Domain.Entities;
using LojaTecidos.Infrastructure.Persistence.Entities;

namespace LojaTecidos.Infrastructure.Persistence.Mappers;

internal static class FornecedorMapper
{
    public static Fornecedor ToDomain(FornecedorEntity entity) =>
        Fornecedor.Reconstituir(entity.Id, entity.Nome);

    public static FornecedorEntity ToEntity(Fornecedor fornecedor) =>
        new()
        {
            Id = fornecedor.Id,
            Nome = fornecedor.Nome
        };
}

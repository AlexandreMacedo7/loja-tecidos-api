using LojaTecidos.Domain.Entities;
using LojaTecidos.Infrastructure.Persistence.Entities;

namespace LojaTecidos.Infrastructure.Persistence.Mappers;

internal static class VendaMapper
{
    public static Venda ToDomain(VendaEntity entity)
    {
        var itens = entity.Itens
            .Select(i => new ItemVenda(i.CodigoInternoProduto, i.Quantidade, i.PrecoUnitario))
            .ToList();

        return Venda.Reconstituir(
            entity.Id,
            entity.NumeroSequencial,
            entity.CodigoVenda,
            entity.DataVenda,
            entity.Tipo,
            entity.Status,
            entity.DescontoPercentual,
            itens,
            entity.UsuarioId);
    }

    public static VendaEntity ToEntity(Venda venda, Guid? clienteId)
    {
        var entity = new VendaEntity
        {
            Id = venda.Id,
            NumeroSequencial = venda.NumeroSequencial,
            CodigoVenda = venda.CodigoVenda,
            DataVenda = venda.DataVenda,
            Tipo = venda.Tipo,
            Status = venda.Status,
            DescontoPercentual = venda.DescontoPercentual,
            UsuarioId = venda.UsuarioId,
            ClienteId = clienteId
        };

        SincronizarItens(venda, entity);
        return entity;
    }

    public static void UpdateEntity(Venda venda, VendaEntity entity, Guid? clienteId)
    {
        entity.Status = venda.Status;
        entity.DescontoPercentual = venda.DescontoPercentual;
        entity.UsuarioId = venda.UsuarioId;
        entity.ClienteId = clienteId;
    }

    private static void SincronizarItens(Venda venda, VendaEntity entity)
    {
        entity.Itens.Clear();

        foreach (var item in venda.Itens)
        {
            entity.Itens.Add(new ItemVendaEntity
            {
                Id = Guid.NewGuid(),
                VendaId = entity.Id,
                CodigoInternoProduto = item.CodigoInternoProduto,
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario
            });
        }
    }
}

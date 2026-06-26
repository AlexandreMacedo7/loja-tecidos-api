using LojaTecidos.Application.Abstractions.Persistence;
using LojaTecidos.Domain.Entities;
using LojaTecidos.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace LojaTecidos.Infrastructure.Persistence.Repositories;

public class VendaRepository : IVendaRepository
{
    private readonly LojaTecidosDbContext _context;

    public VendaRepository(LojaTecidosDbContext context)
    {
        _context = context;
    }

    public async Task<Venda?> ObterPorCodigoAsync(string codigoVenda, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Vendas
            .Include(v => v.Itens)
            .FirstOrDefaultAsync(v => v.CodigoVenda == codigoVenda.Trim(), cancellationToken);

        return entity is null ? null : VendaMapper.ToDomain(entity);
    }

    public async Task<int> ObterProximoNumeroSequencialAsync(CancellationToken cancellationToken = default)
    {
        var ultimoNumero = await _context.Vendas
            .MaxAsync(v => (int?)v.NumeroSequencial, cancellationToken);

        return (ultimoNumero ?? 0) + 1;
    }

    public async Task AdicionarAsync(Venda venda, Guid? clienteId, CancellationToken cancellationToken = default)
    {
        var entity = VendaMapper.ToEntity(venda, clienteId);
        await _context.Vendas.AddAsync(entity, cancellationToken);
    }

    public async Task AtualizarAsync(Venda venda, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Vendas
            .Include(v => v.Itens)
            .FirstOrDefaultAsync(v => v.Id == venda.Id, cancellationToken);

        if (entity is null)
            throw new InvalidOperationException($"Venda {venda.CodigoVenda} não encontrada.");

        VendaMapper.UpdateEntity(venda, entity, entity.ClienteId);
    }
}

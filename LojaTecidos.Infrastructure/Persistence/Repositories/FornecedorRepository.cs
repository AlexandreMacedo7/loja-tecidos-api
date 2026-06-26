using LojaTecidos.Application.Abstractions.Persistence;
using LojaTecidos.Domain.Entities;
using LojaTecidos.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace LojaTecidos.Infrastructure.Persistence.Repositories;

public class FornecedorRepository : IFornecedorRepository
{
    private readonly LojaTecidosDbContext _context;

    public FornecedorRepository(LojaTecidosDbContext context)
    {
        _context = context;
    }

    public async Task<Fornecedor?> ObterPorNomeAsync(string nome, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Fornecedores
            .FirstOrDefaultAsync(f => f.Nome == nome.Trim(), cancellationToken);

        return entity is null ? null : FornecedorMapper.ToDomain(entity);
    }

    public async Task<IReadOnlyList<Fornecedor>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.Fornecedores
            .OrderBy(f => f.Nome)
            .ToListAsync(cancellationToken);

        return entities.Select(FornecedorMapper.ToDomain).ToList();
    }

    public async Task AdicionarAsync(Fornecedor fornecedor, CancellationToken cancellationToken = default)
    {
        var entity = FornecedorMapper.ToEntity(fornecedor);
        await _context.Fornecedores.AddAsync(entity, cancellationToken);
    }
}

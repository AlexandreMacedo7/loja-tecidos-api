using LojaTecidos.Application.Abstractions.Persistence;

namespace LojaTecidos.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly LojaTecidosDbContext _context;

    public UnitOfWork(LojaTecidosDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}

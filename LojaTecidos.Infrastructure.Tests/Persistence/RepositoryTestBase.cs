using LojaTecidos.Application.Abstractions.Persistence;
using LojaTecidos.Infrastructure.Persistence;
using LojaTecidos.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LojaTecidos.Infrastructure.Tests.Persistence;

public class RepositoryTestBase : IDisposable
{
    protected RepositoryTestBase()
    {
        var databaseName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<LojaTecidosDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        Context = new LojaTecidosDbContext(options);

        UnitOfWork = new UnitOfWork(Context);
        ClienteRepository = new ClienteRepository(Context);
        ProdutoRepository = new ProdutoRepository(Context);
        VendaRepository = new VendaRepository(Context);
    }

    protected LojaTecidosDbContext Context { get; }
    protected IUnitOfWork UnitOfWork { get; }
    protected ClienteRepository ClienteRepository { get; }
    protected ProdutoRepository ProdutoRepository { get; }
    protected VendaRepository VendaRepository { get; }

    public void Dispose() => Context.Dispose();
}

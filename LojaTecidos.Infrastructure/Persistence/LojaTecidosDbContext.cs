using LojaTecidos.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LojaTecidos.Infrastructure.Persistence;

public class LojaTecidosDbContext : IdentityDbContext<ApplicationUser>
{
    public LojaTecidosDbContext(DbContextOptions<LojaTecidosDbContext> options)
        : base(options)
    {
    }

    public DbSet<FornecedorEntity> Fornecedores => Set<FornecedorEntity>();
    public DbSet<ClienteEntity> Clientes => Set<ClienteEntity>();
    public DbSet<ContaFiadoEntity> ContasFiado => Set<ContaFiadoEntity>();
    public DbSet<ProdutoEntity> Produtos => Set<ProdutoEntity>();
    public DbSet<MovimentacaoEstoqueEntity> MovimentacoesEstoque => Set<MovimentacaoEstoqueEntity>();
    public DbSet<VendaEntity> Vendas => Set<VendaEntity>();
    public DbSet<ItemVendaEntity> ItensVenda => Set<ItemVendaEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LojaTecidosDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

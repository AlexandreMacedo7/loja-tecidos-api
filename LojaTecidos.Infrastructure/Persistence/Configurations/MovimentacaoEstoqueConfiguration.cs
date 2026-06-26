using LojaTecidos.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaTecidos.Infrastructure.Persistence.Configurations;

public class MovimentacaoEstoqueConfiguration : IEntityTypeConfiguration<MovimentacaoEstoqueEntity>
{
    public void Configure(EntityTypeBuilder<MovimentacaoEstoqueEntity> builder)
    {
        builder.ToTable("MovimentacoesEstoque");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Quantidade).HasPrecision(18, 2);
        builder.Property(m => m.SaldoAposMovimentacao).HasPrecision(18, 2);
    }
}

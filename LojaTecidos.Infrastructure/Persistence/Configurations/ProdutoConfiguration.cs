using LojaTecidos.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaTecidos.Infrastructure.Persistence.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<ProdutoEntity>
{
    public void Configure(EntityTypeBuilder<ProdutoEntity> builder)
    {
        builder.ToTable("Produtos");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.CodigoInterno).HasMaxLength(50).IsRequired();
        builder.HasIndex(p => p.CodigoInterno).IsUnique();
        builder.Property(p => p.CodigoFornecedor).HasMaxLength(50);
        builder.Property(p => p.Nome).HasMaxLength(200).IsRequired();
        builder.Property(p => p.PrecoUnitario).HasPrecision(18, 2);
        builder.Property(p => p.QuantidadeAtual).HasPrecision(18, 2);
        builder.Property(p => p.QuantidadeReferenciaAlerta).HasPrecision(18, 2);

        builder.HasOne(p => p.Fornecedor)
            .WithMany(f => f.Produtos)
            .HasForeignKey(p => p.FornecedorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Movimentacoes)
            .WithOne(m => m.Produto)
            .HasForeignKey(m => m.ProdutoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

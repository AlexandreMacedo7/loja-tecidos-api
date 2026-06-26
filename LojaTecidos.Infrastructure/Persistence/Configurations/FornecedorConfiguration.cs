using LojaTecidos.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaTecidos.Infrastructure.Persistence.Configurations;

public class FornecedorConfiguration : IEntityTypeConfiguration<FornecedorEntity>
{
    public void Configure(EntityTypeBuilder<FornecedorEntity> builder)
    {
        builder.ToTable("Fornecedores");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Nome).HasMaxLength(150).IsRequired();
        builder.HasIndex(f => f.Nome).IsUnique();
    }
}

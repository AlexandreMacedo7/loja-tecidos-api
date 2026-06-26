using LojaTecidos.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaTecidos.Infrastructure.Persistence.Configurations;

public class ContaFiadoConfiguration : IEntityTypeConfiguration<ContaFiadoEntity>
{
    public void Configure(EntityTypeBuilder<ContaFiadoEntity> builder)
    {
        builder.ToTable("ContasFiado");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.SaldoDevedor).HasPrecision(18, 2);
    }
}

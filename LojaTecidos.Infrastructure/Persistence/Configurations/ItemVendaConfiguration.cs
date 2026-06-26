using LojaTecidos.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaTecidos.Infrastructure.Persistence.Configurations;

public class ItemVendaConfiguration : IEntityTypeConfiguration<ItemVendaEntity>
{
    public void Configure(EntityTypeBuilder<ItemVendaEntity> builder)
    {
        builder.ToTable("ItensVenda");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.CodigoInternoProduto).HasMaxLength(50).IsRequired();
        builder.Property(i => i.Quantidade).HasPrecision(18, 2);
        builder.Property(i => i.PrecoUnitario).HasPrecision(18, 2);
    }
}

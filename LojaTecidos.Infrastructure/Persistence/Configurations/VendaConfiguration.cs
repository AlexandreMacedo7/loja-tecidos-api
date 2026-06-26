using LojaTecidos.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaTecidos.Infrastructure.Persistence.Configurations;

public class VendaConfiguration : IEntityTypeConfiguration<VendaEntity>
{
    public void Configure(EntityTypeBuilder<VendaEntity> builder)
    {
        builder.ToTable("Vendas");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.CodigoVenda).HasMaxLength(50).IsRequired();
        builder.HasIndex(v => v.CodigoVenda).IsUnique();
        builder.HasIndex(v => v.NumeroSequencial).IsUnique();
        builder.Property(v => v.DescontoPercentual).HasPrecision(5, 2);
        builder.Property(v => v.UsuarioId).HasMaxLength(100);

        builder.HasOne(v => v.Cliente)
            .WithMany(c => c.Vendas)
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(v => v.Itens)
            .WithOne(i => i.Venda)
            .HasForeignKey(i => i.VendaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

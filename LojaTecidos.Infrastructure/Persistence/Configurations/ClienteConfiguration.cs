using LojaTecidos.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaTecidos.Infrastructure.Persistence.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<ClienteEntity>
{
    public void Configure(EntityTypeBuilder<ClienteEntity> builder)
    {
        builder.ToTable("Clientes");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Nome).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Telefone).HasMaxLength(20).IsRequired();
        builder.Property(c => c.Rua).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Numero).HasMaxLength(20).IsRequired();
        builder.Property(c => c.Bairro).HasMaxLength(120).IsRequired();
        builder.Property(c => c.Cep).HasMaxLength(8).IsRequired();
        builder.Property(c => c.Cpf).HasMaxLength(11);
        builder.Property(c => c.Cnpj).HasMaxLength(14);

        builder.HasMany(c => c.ContasFiado)
            .WithOne(c => c.Cliente)
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

using FastWorkshops.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastWorkshops.Infrastructure.Persistence.Configurations;

public class AtaConfiguration : IEntityTypeConfiguration<Ata>
{
    public void Configure(EntityTypeBuilder<Ata> builder)
    {
        builder.ToTable("Atas");
        builder.HasKey(a => a.Id);

        // 1:1 com Workshop — a FK única garante "uma ata por workshop" no banco
        builder.HasOne(a => a.Workshop)
               .WithOne(w => w.Ata)
               .HasForeignKey<Ata>(a => a.WorkshopId)
               .OnDelete(DeleteBehavior.Cascade);

        // N:N com Colaborador, com nome explícito para a tabela de junção
        builder.HasMany(a => a.Colaboradores)
               .WithMany(c => c.Atas)
               .UsingEntity(j => j.ToTable("AtaColaboradores"));

        builder.Property(a => a.RowVersion)
               .IsRowVersion();
    }
}

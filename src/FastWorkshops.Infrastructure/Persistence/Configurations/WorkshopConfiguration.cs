using FastWorkshops.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastWorkshops.Infrastructure.Persistence.Configurations;

public class WorkshopConfiguration : IEntityTypeConfiguration<Workshop>
{
    public void Configure(EntityTypeBuilder<Workshop> builder)
    {
        builder.ToTable("Workshops");
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Nome).IsRequired().HasMaxLength(200);
        builder.Property(w => w.Descricao).IsRequired().HasMaxLength(2000);
        builder.Property(w => w.DataRealizacao).IsRequired();

        // Índices que sustentam os filtros de GET /api/atas
        builder.HasIndex(w => w.Nome);
        builder.HasIndex(w => w.DataRealizacao);
    }
}

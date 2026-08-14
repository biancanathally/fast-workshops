using FastWorkshops.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastWorkshops.Infrastructure.Persistence.Configurations;

public class ColaboradorConfiguration : IEntityTypeConfiguration<Colaborador>
{
    public void Configure(EntityTypeBuilder<Colaborador> builder)
    {
        builder.ToTable("Colaboradores");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
               .IsRequired()
               .HasMaxLength(150);

        builder.HasIndex(c => c.Nome);
    }
}

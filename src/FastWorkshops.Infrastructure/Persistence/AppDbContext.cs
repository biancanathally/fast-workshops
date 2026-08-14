using FastWorkshops.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastWorkshops.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Colaborador> Colaboradores => Set<Colaborador>();
    public DbSet<Workshop> Workshops => Set<Workshop>();
    public DbSet<Ata> Atas => Set<Ata>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

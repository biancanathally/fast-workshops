using FastWorkshops.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using FastWorkshops.Domain.Repositories;
using FastWorkshops.Domain.Abstractions;
using FastWorkshops.Infrastructure.Persistence.Repositories;

namespace FastWorkshops.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SqlServer")));

        services.AddScoped<IUnitOfWork, UnitOfWorkRepository>();
        services.AddScoped<IWorkshopRepository, WorkshopRepository>();
        services.AddScoped<IColaboradorRepository, ColaboradorRepository>();
        services.AddScoped<IAtaRepository, AtaRepository>();

        return services;
    }
}

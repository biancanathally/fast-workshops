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

        services.AddScoped<IUnitOfWork, UnitOfWorkRepositoryImpl>();
        services.AddScoped<IWorkshopRepository, WorkshopRepositoryImpl>();
        services.AddScoped<IColaboradorRepository, ColaboradorRepositoryImpl>();
        services.AddScoped<IAtaRepository, AtaRepositoryImpl>();

        return services;
    }
}

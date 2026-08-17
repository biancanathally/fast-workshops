using FastWorkshops.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FastWorkshops.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IWorkshopService, WorkshopServiceImpl>();
        services.AddScoped<IColaboradorService, ColaboradorServiceImpl>();
        services.AddScoped<IAtaService, AtaServiceImpl>();

        return services;
    }
}

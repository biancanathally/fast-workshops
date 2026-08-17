using FastWorkshops.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FastWorkshops.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<WorkshopService, WorkshopServiceImpl>();
        services.AddScoped<ColaboradorService, ColaboradorServiceImpl>();
        services.AddScoped<AtaService, AtaServiceImpl>();

        return services;
    }
}

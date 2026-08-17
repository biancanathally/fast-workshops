using FastWorkshops.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FastWorkshops.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IWorkshopService, WorkshopService>();
        services.AddScoped<IColaboradorService, ColaboradorService>();
        services.AddScoped<IAtaService, AtaService>();

        return services;
    }
}

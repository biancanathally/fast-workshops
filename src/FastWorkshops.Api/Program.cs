using System.Reflection;
using FastWorkshops.Api.Middleware;
using FastWorkshops.Application;
using FastWorkshops.Infrastructure;
using FastWorkshops.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicy = "FrontendLocal";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "FAST Workshops API",
        Version = "v1",
        Description = "API para rastreamento de participação em workshops trimestrais."
    });

    var xml = Path.Combine(AppContext.BaseDirectory,
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    if (File.Exists(xml)) options.IncludeXmlComments(xml);
});

builder.Services.AddCors(options =>
    options.AddPolicy(CorsPolicy, policy => policy
        .WithOrigins("http://localhost:5173", "http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod()));

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

// Primeiro de todos: captura exceções de tudo que vem depois
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("/swagger/v1/swagger.json", "FAST Workshops API v1");
    o.RoutePrefix = string.Empty;   // Swagger na raiz
});

app.UseCors(CorsPolicy);
app.UseAuthorization();
app.MapControllers();

// Migration + seed no startup: conveniência para avaliação. Ver README.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await DbSeeder.SeedAsync(context);
}

app.Run();

public partial class Program { }   // necessário para WebApplicationFactory nos testes

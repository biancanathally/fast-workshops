using FastWorkshops.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastWorkshops.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Workshops.AnyAsync()) return;

        var colaboradores = new[]
        {
            "Amanda Nunes", "Bianca Lima", "Bruno Martins",
            "Erick Almeida", "Iasmyn Mendes", "Lucas Lorena",
            "Maria Eduarda Mergulhão", "Mayra Schneider", "Júlio César",
            "Priscila Régis", "Alexsandra Lima"
        }.Select(n => new Colaborador { Nome = n }).ToList();

        context.Colaboradores.AddRange(colaboradores);

        var workshops = new List<Workshop>
        {
            new() { Nome = "Clean Architecture na prática",
                    DataRealizacao = new DateTime(2025, 3, 13, 16, 0, 0),
                    Descricao = "Camadas, dependências e limites de responsabilidade." },
            new() { Nome = "Testes automatizados em .NET",
                    DataRealizacao = new DateTime(2025, 6, 12, 16, 0, 0),
                    Descricao = "Pirâmide de testes, xUnit e testes de integração." },
            new() { Nome = "Introdução a LLMs",
                    DataRealizacao = new DateTime(2025, 9, 11, 16, 0, 0),
                    Descricao = "Fundamentos de modelos de linguagem e casos de uso." },
            new() { Nome = "Observabilidade e logs estruturados",
                    DataRealizacao = new DateTime(2025, 12, 11, 16, 0, 0),
                    Descricao = "Métricas, tracing e diagnóstico em produção." }
        };

        context.Workshops.AddRange(workshops);
        await context.SaveChangesAsync();

        // Presenças variadas de propósito: alimenta os gráficos do frontend
        var atas = new List<Ata>
        {
            new() { WorkshopId = workshops[0].Id, Colaboradores = colaboradores.Take(8).ToList() },
            new() { WorkshopId = workshops[1].Id, Colaboradores = colaboradores.Skip(2).Take(6).ToList() },
            new() { WorkshopId = workshops[2].Id, Colaboradores = colaboradores.Take(10).ToList() },
            new() { WorkshopId = workshops[3].Id, Colaboradores = colaboradores.Skip(5).Take(4).ToList() }
        };

        context.Atas.AddRange(atas);
        await context.SaveChangesAsync();
    }
}

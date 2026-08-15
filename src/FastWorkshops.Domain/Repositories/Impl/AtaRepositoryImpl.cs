using FastWorkshops.Domain.Entities;
using FastWorkshops.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FastWorkshops.Infrastructure.Persistence.Repositories;

public class AtaRepositoryImpl(AppDbContext context) : IAtaRepository
{
    public async Task<Ata?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        await context.Atas
            .Include(a => a.Colaboradores)
            .FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<Ata?> ObterCompletaAsync(int id, CancellationToken ct = default) =>
        await context.Atas
            .Include(a => a.Workshop)
            .Include(a => a.Colaboradores)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, ct);

    public Task<bool> ExistePorWorkshopAsync(int workshopId, CancellationToken ct = default) =>
        context.Atas.AnyAsync(a => a.WorkshopId == workshopId, ct);

    public async Task<List<Ata>> ListarComFiltrosAsync(
        string? workshopNome, DateOnly? data, string? colaboradorNome,
        CancellationToken ct = default)
    {
        var query = context.Atas
            .Include(a => a.Workshop)
            .Include(a => a.Colaboradores)
            .AsNoTracking()
            .AsSplitQuery()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(workshopNome))
            query = query.Where(a => EF.Functions.Like(a.Workshop.Nome, $"%{workshopNome}%"));

        if (data.HasValue)
        {
            var inicio = data.Value.ToDateTime(TimeOnly.MinValue);
            var fim = inicio.AddDays(1);
            query = query.Where(a => a.Workshop.DataRealizacao >= inicio
                                  && a.Workshop.DataRealizacao < fim);
        }

        if (!string.IsNullOrWhiteSpace(colaboradorNome))
            query = query.Where(a => a.Colaboradores
                .Any(c => EF.Functions.Like(c.Nome, $"%{colaboradorNome}%")));

        return await query
            .OrderByDescending(a => a.Workshop.DataRealizacao)
            .ToListAsync(ct);
    }

    public async Task AdicionarAsync(Ata ata, CancellationToken ct = default) =>
        await context.Atas.AddAsync(ata, ct);

    public Task SalvarAsync(CancellationToken ct = default) => context.SaveChangesAsync(ct);
}

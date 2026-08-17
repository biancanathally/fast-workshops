using FastWorkshops.Domain.Entities;
using FastWorkshops.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FastWorkshops.Infrastructure.Persistence.Repositories;

public class WorkshopRepositoryImpl(AppDbContext context) : IWorkshopRepository
{
    public async Task<Workshop?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        await context.Workshops
            .FirstOrDefaultAsync(w => w.Id == id, ct);

    public Task<bool> ExisteAsync(int id, CancellationToken ct = default) =>
        context.Workshops.AnyAsync(w => w.Id == id, ct);

    public async Task<List<Workshop>> ListarComFiltrosAsync(
        string? nome, DateOnly? data, CancellationToken ct = default)
    {
        var query = context.Workshops
            .AsNoTracking()
            .AsSplitQuery()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(w => EF.Functions.Like(w.Nome, $"%{nome}%"));

        if (data.HasValue)
        {
            var inicio = data.Value.ToDateTime(TimeOnly.MinValue);
            var fim = inicio.AddDays(1);
            query = query.Where(w => w.DataRealizacao >= inicio
                                  && w.DataRealizacao < fim);
        }

        return await query
            .OrderByDescending(w => w.DataRealizacao)
            .ToListAsync(ct);
    }

    public async Task AdicionarAsync(Workshop workshop, CancellationToken ct = default) =>
        await context.Workshops.AddAsync(workshop, ct);
}

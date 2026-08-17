using FastWorkshops.Domain.Entities;
using FastWorkshops.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FastWorkshops.Infrastructure.Persistence.Repositories;

public class ColaboradorRepository(AppDbContext context) : IColaboradorRepository
{
    public async Task<Colaborador?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        await context.Colaboradores
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<List<Colaborador>> ObterPorIdsAsync(IEnumerable<int> ids, CancellationToken ct = default) =>
        await context.Colaboradores
            .Where(c => ids.Contains(c.Id))
            .ToListAsync(ct);

    public async Task<List<Colaborador>> ListarComAtasAsync(CancellationToken ct = default) =>
        await context.Colaboradores
            .Include(c => c.Atas).ThenInclude(a => a.Workshop)
            .AsNoTracking()
            .AsSplitQuery()
            .ToListAsync(ct);

    public async Task AdicionarAsync(Colaborador colaborador, CancellationToken ct = default) =>
        await context.Colaboradores.AddAsync(colaborador, ct);
}

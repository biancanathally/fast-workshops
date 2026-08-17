using FastWorkshops.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FastWorkshops.Infrastructure.Persistence.Repositories;

public class UnitOfWorkRepositoryImpl(AppDbContext context) : IUnitOfWork
{
    public async Task<int> CommitAsync(CancellationToken ct = default) =>
        await context.SaveChangesAsync(ct);
}

using FastWorkshops.Domain.Entities;

namespace FastWorkshops.Domain.Repositories;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}

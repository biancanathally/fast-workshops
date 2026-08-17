namespace FastWorkshops.Domain.Abstractions;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}

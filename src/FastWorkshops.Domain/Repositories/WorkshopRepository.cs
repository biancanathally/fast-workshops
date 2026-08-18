using FastWorkshops.Domain.Entities;

namespace FastWorkshops.Domain.Repositories;

public interface IWorkshopRepository
{
    Task<Workshop?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<Workshop?> ObterPorIdSomenteLeituraAsync(int id, CancellationToken ct = default);
    Task<bool> ExisteAsync(int id, CancellationToken ct = default);
    Task<List<Workshop>> ListarComFiltrosAsync(
        string? nome, DateOnly? data, CancellationToken ct = default);
    Task AdicionarAsync(Workshop workshop, CancellationToken ct = default);
}

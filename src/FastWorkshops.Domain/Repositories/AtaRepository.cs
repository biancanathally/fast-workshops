using FastWorkshops.Domain.Entities;

namespace FastWorkshops.Domain.Repositories;

public interface IAtaRepository
{
    Task<Ata?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<Ata?> ObterCompletaAsync(int id, CancellationToken ct = default);
    Task<bool> ExistePorWorkshopAsync(int workshopId, CancellationToken ct = default);
    Task<List<Ata>> ListarComFiltrosAsync(
        string? workshopNome, DateOnly? data, string? colaboradorNome,
        CancellationToken ct = default);
    Task AdicionarAsync(Ata ata, CancellationToken ct = default);
    Task SalvarAsync(CancellationToken ct = default);
}

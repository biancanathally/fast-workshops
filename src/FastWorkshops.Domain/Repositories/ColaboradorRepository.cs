using FastWorkshops.Domain.Entities;

namespace FastWorkshops.Domain.Repositories;

public interface IColaboradorRepository
{
    Task<Colaborador?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<Colaborador?> ObterPorIdSomenteLeituraAsync(int id, CancellationToken ct = default);
    Task<List<Colaborador>> ObterPorIdsAsync(IEnumerable<int> ids, CancellationToken ct = default);
    Task<List<Colaborador>> ListarComAtasAsync(CancellationToken ct = default);
    Task AdicionarAsync(Colaborador colaborador, CancellationToken ct = default);
}

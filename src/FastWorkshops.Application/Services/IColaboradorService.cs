using FastWorkshops.Application.DTOs;

namespace FastWorkshops.Application.Services;

public interface IColaboradorService
{
    Task<ColaboradorDto> CriarAsync(CriarColaboradorRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<ColaboradorComWorkshopsDto>> ListarAsync(CancellationToken ct = default);

    Task<ColaboradorDto> ObterPorIdAsync(int id, CancellationToken ct = default);
}
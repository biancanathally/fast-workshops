using FastWorkshops.Application.DTOs;

namespace FastWorkshops.Application.Services;

public interface WorkshopService
{
    Task<WorkshopDto> CriarAsync(CriarWorkshopRequest request, CancellationToken ct = default);
    Task<WorkshopDto> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<WorkshopDto>> ListarAsync(
        string? nome, DateOnly? data, CancellationToken ct = default);
}
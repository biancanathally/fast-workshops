using FastWorkshops.Application.DTOs;

namespace FastWorkshops.Application.Services;

public interface AtaService
{
    Task<AtaDto> CriarAsync(CriarAtaRequest request, CancellationToken ct = default);
    Task<AtaDto> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task AdicionarColaboradorAsync(int ataId, int colaboradorId, CancellationToken ct = default);
    Task RemoverColaboradorAsync(int ataId, int colaboradorId, CancellationToken ct = default);
    Task<IReadOnlyList<AtaDto>> ListarAsync(
        string? workshopNome, DateOnly? data, string? colaboradorNome, CancellationToken ct = default);
}
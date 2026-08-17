using FastWorkshops.Application.DTOs;
using FastWorkshops.Domain.Entities;
using FastWorkshops.Domain.Exceptions;
using FastWorkshops.Domain.Repositories;
using FastWorkshops.Domain.Abstractions;

namespace FastWorkshops.Application.Services;

public class AtaServiceImpl(
    IAtaRepository atas,
    IWorkshopRepository workshops,
    IColaboradorRepository colaboradores,
    IUnitOfWork uow) : IAtaService
{
    public async Task<AtaDto> CriarAsync(CriarAtaRequest request, CancellationToken ct = default)
    {
        if (!await workshops.ExisteAsync(request.WorkshopId, ct))
            throw new NotFoundException("Workshop", request.WorkshopId);

        // Regra: um workshop tem no máximo uma ata (índice único em Atas.WorkshopId)
        if (await atas.ExistePorWorkshopAsync(request.WorkshopId, ct))
            throw new ConflictException(
                $"O workshop {request.WorkshopId} já possui uma ata de presença.");

        var ids = request.ColaboradorIds?.Distinct().ToList() ?? [];
        var presentes = await colaboradores.ObterPorIdsAsync(ids, ct);

        if (presentes.Count != ids.Count)
        {
            var faltantes = ids.Except(presentes.Select(c => c.Id));
            throw new NotFoundException("Colaborador(es)", string.Join(", ", faltantes));
        }

        var ata = new Ata { WorkshopId = request.WorkshopId, Colaboradores = presentes };

        await atas.AdicionarAsync(ata, ct);
        await uow.CommitAsync(ct);

        // Releitura para trazer o Workshop navegado no DTO de resposta
        return (await atas.ObterCompletaAsync(ata.Id, ct))!.ToDto();
    }

    public async Task<AtaDto> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var ata = await atas.ObterCompletaAsync(id, ct)
            ?? throw new NotFoundException("Ata", id);

        return ata.ToDto();
    }

    public async Task AdicionarColaboradorAsync(
        int ataId, int colaboradorId, CancellationToken ct = default)
    {
        var ata = await atas.ObterPorIdAsync(ataId, ct)
            ?? throw new NotFoundException("Ata", ataId);

        // PUT é idempotente: repetir a chamada não duplica nem falha
        if (ata.Colaboradores.Any(c => c.Id == colaboradorId))
            return;

        var colaborador = await colaboradores.ObterPorIdAsync(colaboradorId, ct)
            ?? throw new NotFoundException("Colaborador", colaboradorId);

        ata.Colaboradores.Add(colaborador);
        await uow.CommitAsync(ct);
    }

    public async Task RemoverColaboradorAsync(
        int ataId, int colaboradorId, CancellationToken ct = default)
    {
        var ata = await atas.ObterPorIdAsync(ataId, ct)
            ?? throw new NotFoundException("Ata", ataId);

        var colaborador = ata.Colaboradores.FirstOrDefault(c => c.Id == colaboradorId)
            ?? throw new NotFoundException("Colaborador na ata", colaboradorId);

        ata.Colaboradores.Remove(colaborador);
        await uow.CommitAsync(ct);
    }

    public async Task<IReadOnlyList<AtaDto>> ListarAsync(
        string? workshopNome, DateOnly? data, string? colaboradorNome,
        CancellationToken ct = default)
    {
        var resultado = await atas.ListarComFiltrosAsync(workshopNome, data, colaboradorNome, ct);
        return resultado.Select(a => a.ToDto()).ToList();
    }
}
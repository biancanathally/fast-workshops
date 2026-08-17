using FastWorkshops.Application.DTOs;
using FastWorkshops.Domain.Entities;
using FastWorkshops.Domain.Exceptions;
using FastWorkshops.Domain.Repositories;
using FastWorkshops.Domain.Abstractions;

namespace FastWorkshops.Application.Services;

public class WorkshopServiceImpl(IWorkshopRepository workshops, IUnitOfWork uow) : IWorkshopService
{
    public async Task<WorkshopDto> CriarAsync(
        CriarWorkshopRequest request, CancellationToken ct = default)
    {
        var workshop = new Workshop
        {
            Nome = request.Nome.Trim(),
            DataRealizacao = request.DataRealizacao,
            Descricao = request.Descricao.Trim()
        };

        await workshops.AdicionarAsync(workshop, ct);
        await uow.CommitAsync(ct);

        // Após o commit o EF preenche o Id gerado pelo banco na própria instância
        return workshop.ToDto();
    }

    public async Task<WorkshopDto> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var workshop = await workshops.ObterPorIdAsync(id, ct)
            ?? throw new NotFoundException("Workshop", id);

        return workshop.ToDto();
    }

    public async Task<IReadOnlyList<WorkshopDto>> ListarAsync(
        string? nome, DateOnly? data, CancellationToken ct = default)
    {
        var resultado = await workshops.ListarComFiltrosAsync(nome, data, ct);
        return resultado.Select(w => w.ToDto()).ToList();
    }
}
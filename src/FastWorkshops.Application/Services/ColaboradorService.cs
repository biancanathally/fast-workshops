using System.Globalization;
using FastWorkshops.Application.DTOs;
using FastWorkshops.Domain.Entities;
using FastWorkshops.Domain.Repositories;
using FastWorkshops.Domain.Abstractions;
using FastWorkshops.Domain.Exceptions;

namespace FastWorkshops.Application.Services;

public class ColaboradorService(IColaboradorRepository colaboradores, IUnitOfWork uow)
    : IColaboradorService
{
    // Comparação linguística pt-BR: garante Álvaro < Ana < Bruno.
    // Ordenar no SQL dependeria do collation do banco; aqui o resultado é determinístico.
    private static readonly StringComparer ComparadorPtBr =
        StringComparer.Create(new CultureInfo("pt-BR"), ignoreCase: true);

    public async Task<ColaboradorDto> CriarAsync(
        CriarColaboradorRequest request, CancellationToken ct = default)
    {
        var colaborador = new Colaborador { Nome = request.Nome.Trim() };

        await colaboradores.AdicionarAsync(colaborador, ct);
        await uow.CommitAsync(ct);

        return colaborador.ToDto();
    }

    public async Task<IReadOnlyList<ColaboradorComWorkshopsDto>> ListarAsync(
        CancellationToken ct = default)
    {
        var lista = await colaboradores.ListarComAtasAsync(ct);

        return lista
            .OrderBy(c => c.Nome, ComparadorPtBr)
            .Select(c => new ColaboradorComWorkshopsDto(
                c.Id,
                c.Nome,
                c.Atas.Count,
                c.Atas.Select(a => a.Workshop.ToDto())
                      .OrderByDescending(w => w.DataRealizacao)
                      .ToList()))
            .ToList();
    }

    public async Task<ColaboradorDto> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var colaborador = await colaboradores.ObterPorIdSomenteLeituraAsync(id, ct)
            ?? throw new NotFoundException("Colaborador", id);

        return colaborador.ToDto();
    }
}
using FastWorkshops.Domain.Entities;

namespace FastWorkshops.Application.DTOs;

public static class Mappers
{
    public static WorkshopDto ToDto(this Workshop w) =>
        new(w.Id, w.Nome, w.DataRealizacao, w.Descricao);

    public static ColaboradorDto ToDto(this Colaborador c) => new(c.Id, c.Nome);

    public static AtaDto ToDto(this Ata a)
    {
        if (a.Workshop is null)
            throw new InvalidOperationException(
                $"Ata {a.Id} foi mapeada sem o Workshop carregado. " +
                "Certifique-se de usar .Include(a => a.Workshop) antes de chamar ToDto().");

        return new(a.Id, a.Workshop.ToDto(), a.Colaboradores.Count,
            a.Colaboradores.Select(c => c.ToDto()).ToList());
    }
}

using FastWorkshops.Domain.Entities;

namespace FastWorkshops.Application.DTOs;

public static class Mappers
{
    public static WorkshopDto ToDto(this Workshop w) =>
        new(w.Id, w.Nome, w.DataRealizacao, w.Descricao);

    public static ColaboradorDto ToDto(this Colaborador c) => new(c.Id, c.Nome);

    public static AtaDto ToDto(this Ata a) =>
        new(a.Id, a.Workshop.ToDto(), a.Colaboradores.Count,
            a.Colaboradores.Select(c => c.ToDto()).ToList());
}

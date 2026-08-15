namespace FastWorkshops.Application.DTOs;

public record WorkshopDto(int Id, string Nome, DateTime DataRealizacao, string Descricao);

public record ColaboradorDto(int Id, string Nome);

// GET /api/colaboradores — colaborador + workshops que participou
public record ColaboradorComWorkshopsDto(
    int Id,
    string Nome,
    int TotalWorkshops,
    IReadOnlyList<WorkshopDto> Workshops);

// GET /api/atas — ata completa
public record AtaDto(
    int Id,
    WorkshopDto Workshop,
    int TotalColaboradores,
    IReadOnlyList<ColaboradorDto> Colaboradores);

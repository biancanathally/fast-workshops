using System.ComponentModel.DataAnnotations;

namespace FastWorkshops.Application.DTOs;

public record CriarWorkshopRequest(
    [Required, StringLength(200, MinimumLength = 3)] string Nome,
    [Required] DateTime DataRealizacao,
    [Required, StringLength(2000, MinimumLength = 3)] string Descricao);

public record CriarColaboradorRequest(
    [Required, StringLength(150, MinimumLength = 2)] string Nome);

public record CriarAtaRequest(
    [Required, Range(1, int.MaxValue)] int WorkshopId,
    List<int>? ColaboradorIds);

using System.Globalization;
using FastWorkshops.Application.DTOs;
using FastWorkshops.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FastWorkshops.Api.Controllers;

[ApiController]
[Route("api/workshops")]
[Produces("application/json")]
public class WorkshopsController(IWorkshopService service) : ControllerBase
{
    /// <summary>Cadastra um novo workshop.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(WorkshopDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WorkshopDto>> Criar(
        CriarWorkshopRequest request, CancellationToken ct)
    {
        var dto = await service.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = dto.Id }, dto);
    }

    /// <summary>Obtém um workshop pelo identificador.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(WorkshopDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkshopDto>> ObterPorId(int id, CancellationToken ct)
        => Ok(await service.ObterPorIdAsync(id, ct));

    /// <summary>Lista workshops com filtros opcionais por nome e data.</summary>
    /// <param name="nome">Filtro opcional por nome do workshop.</param>
    /// <param name="data">Data no formato yyyy-MM-dd (ex.: 2025-06-12).</param>
    /// <param name="ct">Token para cancelamento da operação.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<WorkshopDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<WorkshopDto>>> Listar(
        [FromQuery] string? nome,
        [FromQuery] string? data,
        CancellationToken ct)
    {
        if (!TentarConverterData(data, out var dataFiltro))
            return ValidationProblem(ModelState);

        return Ok(await service.ListarAsync(nome, dataFiltro, ct));
    }

    private bool TentarConverterData(string? entrada, out DateOnly? resultado)
    {
        resultado = null;
        if (string.IsNullOrWhiteSpace(entrada)) return true;

        if (!DateOnly.TryParseExact(entrada, "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            ModelState.AddModelError("data",
                "Formato inválido. Utilize yyyy-MM-dd (ex.: 2025-06-12).");
            return false;
        }

        resultado = parsed;
        return true;
    }
}

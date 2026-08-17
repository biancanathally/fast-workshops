using System.Globalization;
using FastWorkshops.Application.DTOs;
using FastWorkshops.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FastWorkshops.Api.Controllers;

[ApiController]
[Route("api/atas")]
[Produces("application/json")]
public class AtasController(IAtaService service) : ControllerBase
{
    /// <summary>Lista atas de presença com filtros opcionais e combináveis.</summary>
    /// <param name="workshopNome">Busca parcial, sem diferenciar maiúsculas.</param>
    /// <param name="data">Data no formato yyyy-MM-dd (ex.: 2025-06-12).</param>
    /// <param name="colaboradorNome">Busca parcial pelo nome de um presente.</param>
    /// <param name="ct">Token para cancelamento da requisição.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AtaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<AtaDto>>> Listar(
        [FromQuery] string? workshopNome,
        [FromQuery] string? data,
        [FromQuery] string? colaboradorNome,
        CancellationToken ct)
    {
        DateOnly? dataFiltro = null;

        if (!string.IsNullOrWhiteSpace(data))
        {
            if (!DateOnly.TryParseExact(data, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            {
                ModelState.AddModelError(nameof(data),
                    "Formato inválido. Utilize yyyy-MM-dd (ex.: 2025-06-12).");
                return ValidationProblem(ModelState);
            }
            dataFiltro = parsed;
        }

        return Ok(await service.ListarAsync(workshopNome, dataFiltro, colaboradorNome, ct));
    }

    /// <summary>Obtém uma ata pelo identificador.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AtaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AtaDto>> ObterPorId(int id, CancellationToken ct)
        => Ok(await service.ObterPorIdAsync(id, ct));

    /// <summary>Cria a ata de presença de um workshop.</summary>
    /// <remarks>ColaboradorIds é opcional: a ata pode nascer vazia.</remarks>
    [HttpPost]
    [ProducesResponseType(typeof(AtaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AtaDto>> Criar(CriarAtaRequest request, CancellationToken ct)
    {
        var dto = await service.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = dto.Id }, dto);
    }

    /// <summary>Adiciona um colaborador à ata. Operação idempotente.</summary>
    [HttpPut("{ataId:int}/colaboradores/{colaboradorId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdicionarColaborador(
        int ataId, int colaboradorId, CancellationToken ct)
    {
        await service.AdicionarColaboradorAsync(ataId, colaboradorId, ct);
        return NoContent();
    }

    /// <summary>Remove um colaborador da ata.</summary>
    [HttpDelete("{ataId:int}/colaboradores/{colaboradorId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoverColaborador(
        int ataId, int colaboradorId, CancellationToken ct)
    {
        await service.RemoverColaboradorAsync(ataId, colaboradorId, ct);
        return NoContent();
    }
}

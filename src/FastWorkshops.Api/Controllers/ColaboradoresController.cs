using FastWorkshops.Application.DTOs;
using FastWorkshops.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FastWorkshops.Api.Controllers;

[ApiController]
[Route("api/colaboradores")]
[Produces("application/json")]
public class ColaboradoresController(IColaboradorService service) : ControllerBase
{
    /// <summary>
    /// Lista colaboradores em ordem alfabética (pt-BR) com os workshops que participaram.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ColaboradorComWorkshopsDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ColaboradorComWorkshopsDto>>> Listar(
        CancellationToken ct)
        => Ok(await service.ListarAsync(ct));

    /// <summary>Obtém um colaborador pelo identificador.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ColaboradorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ColaboradorDto>> ObterPorId(int id, CancellationToken ct)
        => Ok(await service.ObterPorIdAsync(id, ct));

    /// <summary>Cadastra um novo colaborador.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ColaboradorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ColaboradorDto>> Criar(
        CriarColaboradorRequest request, CancellationToken ct)
    {
        var dto = await service.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = dto.Id }, dto);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rte.Api.DTOs.Unidades;
using Rte.Api.Services;

namespace Rte.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/unidades")]
public class UnidadesController : ControllerBase
{
    private readonly IUnidadeService _unidadeService;

    public UnidadesController(IUnidadeService unidadeService)
    {
        _unidadeService = unidadeService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UnidadeResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UnidadeResponse>>> Listar(CancellationToken cancellationToken)
    {
        return Ok(await _unidadeService.ListarAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UnidadeDetalheResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UnidadeDetalheResponse>> ObterPorId(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await _unidadeService.ObterPorIdAsync(id, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType(typeof(UnidadeResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<UnidadeResponse>> Criar(
        [FromBody] CreateUnidadeRequest request,
        CancellationToken cancellationToken)
    {
        var created = await _unidadeService.CriarAsync(request, cancellationToken);
        return Created($"/api/unidades/{created.Id}", created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UnidadeResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UnidadeResponse>> Atualizar(
        Guid id,
        [FromBody] UpdateUnidadeRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _unidadeService.AtualizarAsync(id, request, cancellationToken));
    }
}

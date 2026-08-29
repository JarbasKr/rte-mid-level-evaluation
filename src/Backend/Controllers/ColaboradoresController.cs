using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rte.Api.DTOs.Colaboradores;
using Rte.Api.Services;

namespace Rte.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/colaboradores")]
public class ColaboradoresController : ControllerBase
{
    private readonly IColaboradorService _colaboradorService;

    public ColaboradoresController(IColaboradorService colaboradorService)
    {
        _colaboradorService = colaboradorService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ColaboradorResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ColaboradorResponse>>> Listar(CancellationToken cancellationToken)
    {
        return Ok(await _colaboradorService.ListarAsync(cancellationToken));
    }

    [HttpGet("{codigo}")]
    [ProducesResponseType(typeof(ColaboradorResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ColaboradorResponse>> ObterPorCodigo(string codigo, CancellationToken cancellationToken)
    {
        return Ok(await _colaboradorService.ObterPorCodigoAsync(codigo, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ColaboradorResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<ColaboradorResponse>> Criar(
        [FromBody] CreateColaboradorRequest request,
        CancellationToken cancellationToken)
    {
        var created = await _colaboradorService.CriarAsync(request, cancellationToken);
        return Created($"/api/colaboradores/{created.Codigo}", created);
    }

    [HttpPut("{codigo}")]
    [ProducesResponseType(typeof(ColaboradorResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ColaboradorResponse>> Atualizar(
        string codigo,
        [FromBody] UpdateColaboradorRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _colaboradorService.AtualizarAsync(codigo, request, cancellationToken));
    }

    [HttpDelete("{codigo}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remover(string codigo, CancellationToken cancellationToken)
    {
        await _colaboradorService.RemoverAsync(codigo, cancellationToken);
        return NoContent();
    }
}

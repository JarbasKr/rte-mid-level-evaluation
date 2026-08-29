using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rte.Api.DTOs.Usuarios;
using Rte.Api.Entities;
using Rte.Api.Services;

namespace Rte.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/usuarios")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UsuarioResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UsuarioResponse>>> Listar(
        [FromQuery] StatusRegistro? status,
        CancellationToken cancellationToken)
    {
        return Ok(await _usuarioService.ListarAsync(status, cancellationToken));
    }

    [HttpGet("{codigo}")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UsuarioResponse>> ObterPorCodigo(string codigo, CancellationToken cancellationToken)
    {
        return Ok(await _usuarioService.ObterPorCodigoAsync(codigo, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<UsuarioResponse>> Criar(
        [FromBody] CreateUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        var created = await _usuarioService.CriarAsync(request, cancellationToken);
        return Created($"/api/usuarios/{created.Codigo}", created);
    }

    [HttpPut("{codigo}")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UsuarioResponse>> Atualizar(
        string codigo,
        [FromBody] UpdateUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _usuarioService.AtualizarAsync(codigo, request, cancellationToken));
    }
}

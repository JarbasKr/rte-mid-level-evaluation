using Rte.Api.Entities;

namespace Rte.Api.DTOs.Usuarios;

public class UpdateUsuarioRequest
{
    public string? Senha { get; set; }
    public StatusRegistro Status { get; set; }
}

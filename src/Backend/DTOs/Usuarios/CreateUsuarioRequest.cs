using Rte.Api.Entities;

namespace Rte.Api.DTOs.Usuarios;

public class CreateUsuarioRequest
{
    public string Codigo { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public StatusRegistro Status { get; set; } = StatusRegistro.Ativo;
}

using Rte.Api.Entities;

namespace Rte.Api.DTOs.Usuarios;

public class UsuarioResponse
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public StatusRegistro Status { get; set; }
}

using Rte.Api.Entities;

namespace Rte.Api.DTOs.Unidades;

public class CreateUnidadeRequest
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public StatusRegistro Status { get; set; } = StatusRegistro.Ativo;
}

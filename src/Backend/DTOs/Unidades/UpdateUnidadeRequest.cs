using Rte.Api.Entities;

namespace Rte.Api.DTOs.Unidades;

public class UpdateUnidadeRequest
{
    public string Nome { get; set; } = string.Empty;
    public StatusRegistro Status { get; set; }
}

using Rte.Api.DTOs.Colaboradores;
using Rte.Api.Entities;

namespace Rte.Api.DTOs.Unidades;

public class UnidadeResponse
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public StatusRegistro Status { get; set; }
    public int QuantidadeColaboradores { get; set; }
}

public class UnidadeDetalheResponse : UnidadeResponse
{
    public List<ColaboradorResponse> Colaboradores { get; set; } = [];
}

namespace Rte.Api.DTOs.Colaboradores;

public class UpdateColaboradorRequest
{
    public string Nome { get; set; } = string.Empty;
    public Guid UnidadeId { get; set; }
}

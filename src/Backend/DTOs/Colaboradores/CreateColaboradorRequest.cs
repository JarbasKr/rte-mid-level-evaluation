namespace Rte.Api.DTOs.Colaboradores;

public class CreateColaboradorRequest
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public Guid UnidadeId { get; set; }
    public Guid UsuarioId { get; set; }
}

namespace Rte.Api.DTOs.Colaboradores;

public class ColaboradorResponse
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public Guid UnidadeId { get; set; }
    public string UnidadeCodigo { get; set; } = string.Empty;
    public string UnidadeNome { get; set; } = string.Empty;
    public Guid UsuarioId { get; set; }
    public string UsuarioLogin { get; set; } = string.Empty;
}

namespace Rte.Api.Entities;

public class Colaborador : EntityBase
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public Guid UnidadeId { get; set; }
    public Guid UsuarioId { get; set; }

    public Unidade Unidade { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}

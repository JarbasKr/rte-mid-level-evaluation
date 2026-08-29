namespace Rte.Api.Entities;

public class Usuario : StatusEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;

    public Colaborador? Colaborador { get; set; }
}

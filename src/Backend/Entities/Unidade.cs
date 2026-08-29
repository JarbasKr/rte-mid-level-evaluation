namespace Rte.Api.Entities;

public class Unidade : StatusEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;

    public ICollection<Colaborador> Colaboradores { get; set; } = new List<Colaborador>();
}

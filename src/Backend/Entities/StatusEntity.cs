namespace Rte.Api.Entities;

/// <summary>
/// Usuários e unidades compartilham ciclo de vida ativo/inativo.
/// Colaborador não herda este tipo porque o enunciado não prevê inativação de colaborador.
/// </summary>
public abstract class StatusEntity : EntityBase
{
    public StatusRegistro Status { get; set; } = StatusRegistro.Ativo;

    public bool EstaAtiva() => Status == StatusRegistro.Ativo;
}

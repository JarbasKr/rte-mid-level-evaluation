namespace Rte.Api.Entities;

/// <summary>
/// Entidade de persistência compartilhada. A herança evita repetir
/// identificador e auditoria em cada agregado, sem inventar hierarquia de domínio artificial.
/// </summary>
public abstract class EntityBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

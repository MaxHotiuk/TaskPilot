namespace Domain.Entities;

public interface IEntity<TId>
{
    TId Id { get; set; }
}

public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}

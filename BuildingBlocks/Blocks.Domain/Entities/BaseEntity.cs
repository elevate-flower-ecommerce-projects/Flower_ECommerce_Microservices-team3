namespace Blocks.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
}

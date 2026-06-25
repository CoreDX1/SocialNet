namespace SocialNet.Domain.Common;

public abstract class BaseEntity
{
    public Guid id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

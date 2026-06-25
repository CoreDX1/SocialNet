using SocialNet.Domain.Common;

namespace SocialNet.Domain.Entities;

public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }

    public User User { get; set; } = null!;
    public Post Post { get; set; } = null!;
}

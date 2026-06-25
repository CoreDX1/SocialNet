using SocialNet.Domain.Common;

namespace SocialNet.Domain.Entities;

public class Like : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }

    public User User { get; set; } = null!;
    public Post Post { get; set; } = null!;
}

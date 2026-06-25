using SocialNet.Domain.Common;

namespace SocialNet.Domain.Entities;

public class Follow : BaseEntity
{
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }

    public User Follower { get; set; } = null!;
    public User Following { get; set; } = null!;
}

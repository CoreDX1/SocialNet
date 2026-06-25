using SocialNet.Domain.Common;

namespace SocialNet.Domain.Entities;

public class Post : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
}

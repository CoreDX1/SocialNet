using Microsoft.EntityFrameworkCore;
using SocialNet.Application.Interfaces;
using SocialNet.Domain.Entities;

namespace SocialNet.Application.Services;

public class LikeService : ILikeService
{
    private readonly IUnitOfWork _uow;
    private readonly DbContext _db;

    public LikeService(IUnitOfWork uow, DbContext db)
    {
        _uow = uow;
        _db = db;
    }

    public async Task<bool> ToggleLikeAsync(
        Guid postId,
        Guid userId,
        CancellationToken ct = default
    )
    {
        var postExists = await _db.Set<Post>().AnyAsync(p => p.Id == postId, ct);

        if (!postExists)
            throw new ApplicationException("El post no existe.");

        var existingLike = await _db.Set<Like>()
            .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId, ct);

        if (existingLike is not null)
        {
            // Ya existe → quitar like
            _db.Set<Like>().Remove(existingLike);
            await _uow.SaveChangesAsync(ct);
            return false; // ahora no tiene like
        }
        else
        {
            // No existe → dar like
            var like = new Like { PostId = postId, UserId = userId };

            _db.Set<Like>().Add(like);
            await _uow.SaveChangesAsync(ct);
            return true; // ahora sí tiene like
        }
    }
}

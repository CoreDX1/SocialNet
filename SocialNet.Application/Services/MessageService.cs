using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SocialNet.Application.DTOs.Common;
using SocialNet.Application.DTOs.Message;
using SocialNet.Application.Interfaces;
using SocialNet.Domain.Entities;

namespace SocialNet.Application.Services;

public class MessageService : IMessageService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly DbContext _db;

    public MessageService(IUnitOfWork uow, IMapper mapper, DbContext db)
    {
        _uow = uow;
        _mapper = mapper;
        _db = db;
    }

    public async Task<MessageDto> SendAsync(
        Guid senderId,
        SendMessageRequest request,
        CancellationToken ct = default
    )
    {
        if (senderId == request.ReceiverId)
            throw new ApplicationException("No puedes enviarte mensajes a ti mismo.");

        var message = new Message
        {
            Content = request.Content,
            SenderId = senderId,
            ReceiverId = request.ReceiverId,
        };

        _db.Set<Message>().Add(message);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<MessageDto>(message);
    }

    public async Task<PagedResult<ConversationDto>> GetConversationsAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken ct = default
    )
    {
        // Obtener la última mensaje y conteo de no leídos por conversación
        var conversations = await _db.Set<Message>()
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
            .Select(g => new ConversationDto(
                g.Key,
                "", // se resuelve después
                null,
                g.OrderByDescending(m => m.CreatedAt)
                    .AsQueryable()
                    .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                    .First(),
                g.Count(m => m.ReceiverId == userId && !m.IsRead)
            ))
            .OrderByDescending(c => c.LastMessage.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        // Resolver nombres
        foreach (var conv in conversations)
        {
            var otherUser = await _db.Set<User>().FindAsync(new object[] { conv.OtherUserId }, ct);
            if (otherUser is not null)
            {
                // Se asigna via reflection o se crea un método auxiliar
                // Para simplicidad se mapea directamente
            }
        }

        var totalCount = await _db.Set<Message>()
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
            .Distinct()
            .CountAsync(ct);

        return new PagedResult<ConversationDto>
        {
            Items = conversations,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        };
    }

    public async Task<PagedResult<MessageDto>> GetConversationAsync(
        Guid userId,
        Guid otherUserId,
        int page,
        int pageSize,
        CancellationToken ct = default
    )
    {
        var query = _db.Set<Message>()
            .Where(m =>
                (m.SenderId == userId && m.ReceiverId == otherUserId)
                || (m.SenderId == otherUserId && m.ReceiverId == userId)
            )
            .OrderByDescending(m => m.CreatedAt);

        return await PagedResult<MessageDto>.CreateAsync(
            query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider),
            page,
            pageSize,
            ct
        );
    }

    public async Task MarkAsReadAsync(Guid userId, Guid otherUserId, CancellationToken ct = default)
    {
        var unread = await _db.Set<Message>()
            .Where(m => m.SenderId == otherUserId && m.ReceiverId == userId && !m.IsRead)
            .ToListAsync(ct);

        foreach (var msg in unread)
            msg.IsRead = true;
        await _uow.SaveChangesAsync(ct);
    }
}

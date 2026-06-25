using AutoMapper;
using SocialNet.Application.DTOs.Comment;
using SocialNet.Application.DTOs.Message;
using SocialNet.Application.DTOs.Post;
using SocialNet.Application.DTOs.User;
using SocialNet.Domain.Entities;

namespace SocialNet.Application.Mappings;

public class MappingProfile: Profile{

    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<Post, PostDto>()
            .ForMember(d => d.Username, opt => opt.MapFrom(s => s.User.Username))
            .ForMember(d => d.UserAvatarUrl, opt => opt.MapFrom(s => s.User.AvatarUrl))
            .ForMember(d => d.LikesCount, opt => opt.MapFrom(s => s.Likes.Count))
            .ForMember(d => d.CommentCount, opt => opt.MapFrom(s => s.Comments.Count));
        CreateMap<Comment,CommentDto>()
            .ForMember(d => d.Username, opt => opt.MapFrom(s => s.User.Username))
            .ForMember(d => d.UserAvatarUrl, opt => opt.MapFrom(s => s.User.AvatarUrl));

        CreateMap<Message, MessageDto>()
            .ForMember(d => d.SenderUsername, opt => opt.MapFrom(s => s.Sender.Username))
            .ForMember(d => d.ReceiverUsername, opt => opt.MapFrom(s => s.Receiver.Username));
    }
}

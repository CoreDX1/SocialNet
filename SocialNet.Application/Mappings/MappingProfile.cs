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
        CreateMap<Post, PostDto>();
    }
}

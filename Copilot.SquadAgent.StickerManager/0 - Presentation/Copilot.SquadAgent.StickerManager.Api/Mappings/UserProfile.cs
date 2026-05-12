using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;
using Copilot.SquadAgent.StickerManager.Domain.Models.User;

namespace Copilot.SquadAgent.StickerManager.Api.Mappings;

[ExcludeFromCodeCoverage]
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<RegisterUserRequest, RegisterUserModel>();
        CreateMap<UserModel, RegisterUserResponse>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));

        CreateMap<LoginUserRequest, LoginUserModel>();
        CreateMap<TokenModel, LoginUserResponse>();

        CreateMap<UserModel, UserProfileResponse>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));

        CreateMap<UpdateUserProfileRequest, UpdateUserProfileModel>();
    }
}

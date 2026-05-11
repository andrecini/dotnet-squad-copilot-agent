using AutoMapper;
using Copilot.SquadAgent.StickerManager.Domain.Entities;
using Copilot.SquadAgent.StickerManager.Domain.Models.User;

namespace Copilot.SquadAgent.StickerManager.Domain.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<RegisterUserModel, User>();
        CreateMap<User, UserModel>();
    }
}

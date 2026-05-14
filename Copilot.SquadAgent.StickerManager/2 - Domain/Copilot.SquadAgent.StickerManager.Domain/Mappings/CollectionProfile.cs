using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Copilot.SquadAgent.StickerManager.Domain.Entities;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

namespace Copilot.SquadAgent.StickerManager.Domain.Mappings;

[ExcludeFromCodeCoverage]
public class CollectionProfile : Profile
{
    public CollectionProfile()
    {
        CreateMap<UserCollection, UserCollectionModel>();
    }
}

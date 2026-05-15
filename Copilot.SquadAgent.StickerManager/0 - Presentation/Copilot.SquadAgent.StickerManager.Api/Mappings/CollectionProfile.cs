using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

namespace Copilot.SquadAgent.StickerManager.Api.Mappings;

[ExcludeFromCodeCoverage]
public class CollectionProfile : Profile
{
    public CollectionProfile()
    {
        CreateMap<AddToCollectionRequest, AddToCollectionModel>();

        CreateMap<UserCollectionModel, AddToCollectionResponse>()
            .ForMember(dest => dest.CollectionId, opt => opt.MapFrom(src => src.Id));

        CreateMap<UserCollectionModel, ToggleDuplicateResponse>()
            .ForMember(dest => dest.CollectionId, opt => opt.MapFrom(src => src.Id));

        CreateMap<CollectionQueryRequest, ListCollectionModel>();

        CreateMap<CollectionItemModel, CollectionItemResponse>()
            .ForMember(dest => dest.Team, opt => opt.MapFrom(src => src.TeamName));

        CreateMap<MissingStickerItemModel, MissingStickerItemResponse>()
            .ForMember(dest => dest.Team, opt => opt.MapFrom(src => src.TeamName));
    }
}

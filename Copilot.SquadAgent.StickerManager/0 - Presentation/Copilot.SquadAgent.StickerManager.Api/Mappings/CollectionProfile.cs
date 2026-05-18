using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests.Queries;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Responses.Paged;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using Copilot.SquadAgent.StickerManager.Domain.Result;

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

        CreateMap<MissingStickersQueryRequest, MissingStickersModel>();

        CreateMap<CollectionItemModel, CollectionItemResponse>()
            .ForMember(dest => dest.Team, opt => opt.MapFrom(src => src.TeamName));

        CreateMap<MissingStickerItemModel, MissingStickerItemResponse>()
            .ForMember(dest => dest.Team, opt => opt.MapFrom(src => src.TeamName));

        CreateMap<TeamStatsModel, TeamStatsResponse>()
            .ForMember(dest => dest.Team, opt => opt.MapFrom(src => src.TeamName));

        CreateMap<CollectionStatsModel, CollectionStatsResponse>();

        CreateMap<AlbumStickerModel, AlbumItemResponse>()
            .ForMember(dest => dest.Team, opt => opt.MapFrom(src => src.TeamName));

        CreateMap<AlbumQueryRequest, AlbumQueryModel>();

        CreateMap<PagedResult<AlbumStickerModel>, PagedAlbumResponse>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.TotalPages, opt => opt.MapFrom(src => src.TotalPages));

        CreateMap<PagedResult<CollectionItemModel>, PagedCollectionResponse>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.TotalPages, opt => opt.MapFrom(src => src.TotalPages));

        CreateMap<PagedResult<MissingStickerItemModel>, PagedMissingStickersResponse>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.TotalPages, opt => opt.MapFrom(src => src.TotalPages));

        CreateMap<TeamProgressModel, TeamProgressResponse>();
    }
}

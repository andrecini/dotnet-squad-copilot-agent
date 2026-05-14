using AutoMapper;
using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;
using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Services;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

namespace Copilot.SquadAgent.StickerManager.Api.AppServices;

public class CollectionAppService(ICollectionService collectionService, IMapper mapper) : ICollectionAppService
{
    public async Task<IResult> AddStickerAsync(Guid userId, AddToCollectionRequest request, CancellationToken cancellationToken)
    {
        var model = mapper.Map<AddToCollectionModel>(request);
        model.UserId = userId;

        var result = await collectionService.AddStickerAsync(model, cancellationToken);

        if (result.IsFailure)
            return Results.Problem(result.Message, statusCode: result.StatusCode ?? 500);

        var response = mapper.Map<AddToCollectionResponse>(result.Value);
        return TypedResults.Created($"/api/v1/collection/{response.CollectionId}", response);
    }

    public async Task<IResult> RemoveStickerFromCollectionAsync(Guid userId, Guid collectionId, CancellationToken cancellationToken)
    {
        var model = new RemoveStickerFromCollectionModel
        {
            CollectionId = collectionId,
            UserId = userId
        };

        var result = await collectionService.RemoveStickerFromCollectionAsync(model, cancellationToken);

        if (result.IsFailure)
            return Results.Problem(result.Message, statusCode: result.StatusCode ?? 500);

        return TypedResults.NoContent();
    }
}

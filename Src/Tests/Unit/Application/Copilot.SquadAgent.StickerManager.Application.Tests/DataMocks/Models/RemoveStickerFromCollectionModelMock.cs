using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;

public static class RemoveStickerFromCollectionModelMock
{
    public static RemoveStickerFromCollectionModel Valid(Guid? userId = null, Guid? collectionId = null) => new()
    {
        CollectionId = collectionId ?? Guid.NewGuid(),
        UserId = userId ?? Guid.NewGuid()
    };
}

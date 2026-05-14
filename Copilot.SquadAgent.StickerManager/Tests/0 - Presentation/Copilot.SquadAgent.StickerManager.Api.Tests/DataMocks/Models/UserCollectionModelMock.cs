using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Models;

public static class UserCollectionModelMock
{
    public static UserCollectionModel Valid() => new()
    {
        Id = Guid.NewGuid(),
        UserId = Guid.NewGuid(),
        StickerId = Guid.NewGuid(),
        QuantityOwned = 1,
        QuantityDuplicate = 0,
        CreatedAt = DateTime.UtcNow
    };
}

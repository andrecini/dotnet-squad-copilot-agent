using UserCollectionEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.UserCollection;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Entities;

public static class UserCollectionEntityMock
{
    public static UserCollectionEntity Valid() => new()
    {
        UserId = Guid.NewGuid(),
        StickerId = Guid.NewGuid(),
        QuantityOwned = 1,
        QuantityDuplicate = 0
    };

    public static UserCollectionEntity WithMaxDuplicates() => new()
    {
        UserId = Guid.NewGuid(),
        StickerId = Guid.NewGuid(),
        QuantityOwned = 11,
        QuantityDuplicate = 10
    };

    public static UserCollectionEntity WithDuplicates(int quantityDuplicate) => new()
    {
        UserId = Guid.NewGuid(),
        StickerId = Guid.NewGuid(),
        QuantityOwned = quantityDuplicate + 1,
        QuantityDuplicate = quantityDuplicate
    };
}

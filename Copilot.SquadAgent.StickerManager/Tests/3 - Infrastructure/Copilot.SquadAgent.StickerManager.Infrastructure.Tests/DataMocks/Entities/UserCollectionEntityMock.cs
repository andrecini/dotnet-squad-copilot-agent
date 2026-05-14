using UserCollectionEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.UserCollection;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Tests.DataMocks.Entities;

public static class UserCollectionEntityMock
{
    public static UserCollectionEntity Valid(Guid userId, Guid stickerId) => new()
    {
        UserId = userId,
        StickerId = stickerId,
        QuantityOwned = 1,
        QuantityDuplicate = 0
    };

    public static UserCollectionEntity Valid() => Valid(Guid.NewGuid(), Guid.NewGuid());
}

using StickerEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.Sticker;
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

    public static UserCollectionEntity ValidWithSticker(Guid userId, StickerEntity sticker) => new()
    {
        UserId = userId,
        StickerId = sticker.Id,
        Sticker = sticker,
        QuantityOwned = 1,
        QuantityDuplicate = 0
    };

    public static UserCollectionEntity Deleted(Guid userId, Guid stickerId) => new()
    {
        UserId = userId,
        StickerId = stickerId,
        QuantityOwned = 1,
        QuantityDuplicate = 0,
        DeletedAt = DateTime.UtcNow.AddDays(-1)
    };
}

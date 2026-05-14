using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;

public static class AddToCollectionModelMock
{
    public static AddToCollectionModel Valid() => new()
    {
        UserId = Guid.NewGuid(),
        StickerId = Guid.NewGuid()
    };
}

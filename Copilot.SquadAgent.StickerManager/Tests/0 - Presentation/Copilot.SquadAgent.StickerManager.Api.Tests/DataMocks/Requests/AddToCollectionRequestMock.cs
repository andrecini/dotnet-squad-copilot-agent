using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;

public static class AddToCollectionRequestMock
{
    public static AddToCollectionRequest Valid() => new()
    {
        StickerId = Guid.NewGuid()
    };

    public static AddToCollectionRequest WithEmptyStickerId() => new()
    {
        StickerId = Guid.Empty
    };
}

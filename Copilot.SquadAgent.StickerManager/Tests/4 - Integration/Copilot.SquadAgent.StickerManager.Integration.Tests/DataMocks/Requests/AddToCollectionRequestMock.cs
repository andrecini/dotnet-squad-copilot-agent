using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Integration.Tests.DataMocks.Requests;

[ExcludeFromCodeCoverage]
public static class AddToCollectionRequestMock
{
    public static AddToCollectionRequest Valid(Guid stickerId) => new()
    {
        StickerId = stickerId
    };

    public static AddToCollectionRequest WithEmptyStickerId() => new()
    {
        StickerId = Guid.Empty
    };
}

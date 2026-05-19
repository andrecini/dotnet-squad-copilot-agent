using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Integration.Tests.DataMocks.Requests;

[ExcludeFromCodeCoverage]
public static class ToggleDuplicateRequestMock
{
    public static ToggleDuplicateRequest Mark() => new()
    {
        Action = DuplicateAction.Mark
    };

    public static ToggleDuplicateRequest Unmark() => new()
    {
        Action = DuplicateAction.Unmark
    };
}

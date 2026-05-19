using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Domain.Enums;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;

public static class ToggleDuplicateRequestMock
{
    public static ToggleDuplicateRequest Valid() => new()
    {
        Action = DuplicateAction.Mark
    };

    public static ToggleDuplicateRequest WithUnmarkAction() => new()
    {
        Action = DuplicateAction.Unmark
    };
}

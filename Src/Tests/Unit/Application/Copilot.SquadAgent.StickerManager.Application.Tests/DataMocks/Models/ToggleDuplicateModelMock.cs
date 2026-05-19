using Copilot.SquadAgent.StickerManager.Domain.Enums;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;

public static class ToggleDuplicateModelMock
{
    public static ToggleDuplicateModel Valid(Guid? userId = null, Guid? collectionId = null) => new()
    {
        CollectionId = collectionId ?? Guid.NewGuid(),
        UserId = userId ?? Guid.NewGuid(),
        Action = DuplicateAction.Mark
    };

    public static ToggleDuplicateModel WithUnmarkAction(Guid? userId = null, Guid? collectionId = null) => new()
    {
        CollectionId = collectionId ?? Guid.NewGuid(),
        UserId = userId ?? Guid.NewGuid(),
        Action = DuplicateAction.Unmark
    };
}

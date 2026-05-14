using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;

namespace Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;

public interface ICollectionAppService
{
    Task<IResult> AddStickerAsync(Guid userId, AddToCollectionRequest request, CancellationToken cancellationToken);
}

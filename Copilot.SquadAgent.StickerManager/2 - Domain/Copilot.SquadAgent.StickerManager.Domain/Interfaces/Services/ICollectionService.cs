using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using ResultNs = Copilot.SquadAgent.StickerManager.Domain.Result;

namespace Copilot.SquadAgent.StickerManager.Domain.Interfaces.Services;

public interface ICollectionService
{
    Task<ResultNs.Result<UserCollectionModel>> AddStickerAsync(AddToCollectionModel model, CancellationToken cancellationToken);
}

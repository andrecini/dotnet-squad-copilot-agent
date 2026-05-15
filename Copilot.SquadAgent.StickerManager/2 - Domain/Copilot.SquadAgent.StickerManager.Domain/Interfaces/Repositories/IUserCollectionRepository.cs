using Copilot.SquadAgent.StickerManager.Domain.Entities;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using ResultNs = Copilot.SquadAgent.StickerManager.Domain.Result;

namespace Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;

public interface IUserCollectionRepository
{
    Task<ResultNs.Result<UserCollection?>> GetByUserAndStickerAsync(Guid userId, Guid stickerId, CancellationToken cancellationToken);
    Task<ResultNs.Result<UserCollection?>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ResultNs.Result<UserCollection>> CreateAsync(UserCollection userCollection, CancellationToken cancellationToken);
    Task<ResultNs.Result<UserCollection>> UpdateAsync(UserCollection userCollection, CancellationToken cancellationToken);
    Task<ResultNs.Result> SoftDeleteAsync(UserCollection userCollection, CancellationToken cancellationToken);
    Task<ResultNs.Result<IReadOnlyList<CollectionItemModel>>> ListByUserAsync(ListCollectionModel filter, CancellationToken cancellationToken);
}

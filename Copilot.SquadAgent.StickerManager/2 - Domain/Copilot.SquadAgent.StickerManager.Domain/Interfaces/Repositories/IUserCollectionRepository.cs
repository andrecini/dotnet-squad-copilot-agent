using Copilot.SquadAgent.StickerManager.Domain.Entities;
using Copilot.SquadAgent.StickerManager.Domain.Result;

namespace Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;

public interface IUserCollectionRepository
{
    Task<Result<UserCollection?>> GetByUserAndStickerAsync(Guid userId, Guid stickerId, CancellationToken cancellationToken);
    Task<Result<UserCollection>> CreateAsync(UserCollection userCollection, CancellationToken cancellationToken);
    Task<Result<UserCollection>> UpdateAsync(UserCollection userCollection, CancellationToken cancellationToken);
}

using Copilot.SquadAgent.StickerManager.Domain.Result;
using UserEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.User;

namespace Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Result<UserEntity>> CreateAsync(UserEntity user, CancellationToken cancellationToken);
}

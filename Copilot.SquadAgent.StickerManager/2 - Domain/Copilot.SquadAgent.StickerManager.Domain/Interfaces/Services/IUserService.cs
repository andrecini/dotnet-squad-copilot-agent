using Copilot.SquadAgent.StickerManager.Domain.Models.User;
using Copilot.SquadAgent.StickerManager.Domain.Result;

namespace Copilot.SquadAgent.StickerManager.Domain.Interfaces.Services;

public interface IUserService
{
    Task<Result<UserModel>> RegisterAsync(RegisterUserModel model, CancellationToken cancellationToken);
}

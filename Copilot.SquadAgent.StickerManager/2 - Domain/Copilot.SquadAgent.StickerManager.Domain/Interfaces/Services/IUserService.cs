using Copilot.SquadAgent.StickerManager.Domain.Models.User;
using Copilot.SquadAgent.StickerManager.Domain.Result;

namespace Copilot.SquadAgent.StickerManager.Domain.Interfaces.Services;

public interface IUserService
{
    Task<Result<UserModel>> RegisterAsync(RegisterUserModel model, CancellationToken cancellationToken);
    Task<Result<TokenModel>> LoginAsync(LoginUserModel model, CancellationToken cancellationToken);
    Task<Result<UserModel>> GetProfileAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<UserModel>> UpdateProfileAsync(UpdateUserProfileModel model, CancellationToken cancellationToken);
}

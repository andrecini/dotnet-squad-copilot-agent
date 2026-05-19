using Copilot.SquadAgent.StickerManager.Domain.Models.User;
using ResultNs = Copilot.SquadAgent.StickerManager.Domain.Result;

namespace Copilot.SquadAgent.StickerManager.Domain.Interfaces.Services;

public interface IUserService
{
    Task<ResultNs.Result<UserModel>> RegisterAsync(RegisterUserModel model, CancellationToken cancellationToken);
    Task<ResultNs.Result<TokenModel>> LoginAsync(LoginUserModel model, CancellationToken cancellationToken);
    Task<ResultNs.Result<UserModel>> GetProfileAsync(Guid userId, CancellationToken cancellationToken);
    Task<ResultNs.Result<UserModel>> UpdateProfileAsync(UpdateUserProfileModel model, CancellationToken cancellationToken);
    Task<ResultNs.Result> ForgotPasswordAsync(ForgotPasswordModel model, CancellationToken cancellationToken);
    Task<ResultNs.Result> ResetPasswordAsync(ResetPasswordModel model, CancellationToken cancellationToken);
}

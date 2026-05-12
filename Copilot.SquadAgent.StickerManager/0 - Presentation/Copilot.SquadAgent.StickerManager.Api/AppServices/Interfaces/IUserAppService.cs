using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;

namespace Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;

public interface IUserAppService
{
    Task<IResult> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken);
    Task<IResult> LoginAsync(LoginUserRequest request, CancellationToken cancellationToken);
    Task<IResult> GetProfileAsync(Guid userId, CancellationToken cancellationToken);
    Task<IResult> UpdateProfileAsync(Guid userId, UpdateUserProfileRequest request, CancellationToken cancellationToken);
}

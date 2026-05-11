using AutoMapper;
using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;
using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Security;
using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Services;
using Copilot.SquadAgent.StickerManager.Domain.Models.User;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using UserEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.User;

namespace Copilot.SquadAgent.StickerManager.Application.Services.User;

public class UserService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IMapper mapper) : IUserService
{
    public async Task<Result<UserModel>> RegisterAsync(RegisterUserModel model, CancellationToken cancellationToken)
    {
        var emailExists = await userRepository.ExistsByEmailAsync(model.Email, cancellationToken);

        if (emailExists)
            return Result<UserModel>.Failure(ResultCode.Conflict, "E-mail já cadastrado.", statusCode: 409);

        var user = mapper.Map<UserEntity>(model);
        user.PasswordHash = passwordHasher.Hash(model.Password);

        var result = await userRepository.CreateAsync(user, cancellationToken);

        if (result.IsFailure)
            return Result<UserModel>.Failure(result.Code, result.Message!, result.StatusCode);

        return Result<UserModel>.Success(mapper.Map<UserModel>(result.Value));
    }
}

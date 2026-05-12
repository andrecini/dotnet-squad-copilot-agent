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
    IJwtTokenGenerator jwtTokenGenerator,
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

    public async Task<Result<TokenModel>> LoginAsync(LoginUserModel model, CancellationToken cancellationToken)
    {
        var result = await userRepository.GetByEmailAsync(model.Email, cancellationToken);

        if (result.IsFailure)
            return Result<TokenModel>.Failure(ResultCode.Unauthorized, "Credenciais inválidas.", statusCode: 401);

        var user = result.Value!;
        var passwordValid = passwordHasher.Verify(model.Password, user.PasswordHash);

        if (!passwordValid)
            return Result<TokenModel>.Failure(ResultCode.Unauthorized, "Credenciais inválidas.", statusCode: 401);

        var userModel = mapper.Map<UserModel>(user);
        var token = jwtTokenGenerator.Generate(userModel);

        return Result<TokenModel>.Success(token);
    }

    public async Task<Result<UserModel>> GetProfileAsync(Guid userId, CancellationToken cancellationToken)
    {
        var result = await userRepository.GetByIdAsync(userId, cancellationToken);

        if (result.IsFailure)
            return Result<UserModel>.Failure(result.Code, result.Message!, result.StatusCode);

        return Result<UserModel>.Success(mapper.Map<UserModel>(result.Value));
    }

    public async Task<Result<UserModel>> UpdateProfileAsync(UpdateUserProfileModel model, CancellationToken cancellationToken)
    {
        var result = await userRepository.GetByIdAsync(model.UserId, cancellationToken);

        if (result.IsFailure)
            return Result<UserModel>.Failure(result.Code, result.Message!, result.StatusCode);

        var user = result.Value!;

        if (model.Email is not null && model.Email != user.Email)
        {
            var emailExists = await userRepository.ExistsByEmailAsync(model.Email, cancellationToken);

            if (emailExists)
                return Result<UserModel>.Failure(ResultCode.Conflict, "E-mail já cadastrado.", statusCode: 409);

            user.Email = model.Email;
        }

        if (model.Name is not null)
            user.Name = model.Name;

        var updateResult = await userRepository.UpdateAsync(user, cancellationToken);

        if (updateResult.IsFailure)
            return Result<UserModel>.Failure(updateResult.Code, updateResult.Message!, updateResult.StatusCode);

        return Result<UserModel>.Success(mapper.Map<UserModel>(updateResult.Value));
    }
}

using AutoMapper;
using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;
using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Services;
using Copilot.SquadAgent.StickerManager.Domain.Models.User;

namespace Copilot.SquadAgent.StickerManager.Api.AppServices;

public class UserAppService(IUserService userService, IMapper mapper) : IUserAppService
{
    public async Task<IResult> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var model = mapper.Map<RegisterUserModel>(request);
        var result = await userService.RegisterAsync(model, cancellationToken);

        if (result.IsFailure)
            return Results.Problem(result.Message, statusCode: result.StatusCode ?? 500);

        var response = mapper.Map<RegisterUserResponse>(result.Value);
        return TypedResults.Created($"/api/v1/users/{response.UserId}", response);
    }

    public async Task<IResult> LoginAsync(LoginUserRequest request, CancellationToken cancellationToken)
    {
        var model = mapper.Map<LoginUserModel>(request);
        var result = await userService.LoginAsync(model, cancellationToken);

        if (result.IsFailure)
            return Results.Problem(result.Message, statusCode: result.StatusCode ?? 500);

        var response = mapper.Map<LoginUserResponse>(result.Value);
        return TypedResults.Ok(response);
    }

    public async Task<IResult> GetProfileAsync(Guid userId, CancellationToken cancellationToken)
    {
        var result = await userService.GetProfileAsync(userId, cancellationToken);

        if (result.IsFailure)
            return Results.Problem(result.Message, statusCode: result.StatusCode ?? 500);

        var response = mapper.Map<UserProfileResponse>(result.Value);
        return TypedResults.Ok(response);
    }

    public async Task<IResult> UpdateProfileAsync(Guid userId, UpdateUserProfileRequest request, CancellationToken cancellationToken)
    {
        var model = mapper.Map<UpdateUserProfileModel>(request);
        model.UserId = userId;

        var result = await userService.UpdateProfileAsync(model, cancellationToken);

        if (result.IsFailure)
            return Results.Problem(result.Message, statusCode: result.StatusCode ?? 500);

        var response = mapper.Map<UserProfileResponse>(result.Value);
        return TypedResults.Ok(response);
    }
}

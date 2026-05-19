using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.Filters;
using Copilot.SquadAgent.StickerManager.Api.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Copilot.SquadAgent.StickerManager.Api.Endpoints.Users;

[ExcludeFromCodeCoverage]
public static class UpdateUserProfileEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/users/profile", async (
            UpdateUserProfileRequest request,
            ClaimsPrincipal principal,
            IUserAppService appService,
            CancellationToken cancellationToken) =>
        {
            var userIdResult = AuthorizationHelper.GetUserIdFromClaims(principal);

            if (userIdResult.IsFailure) return Results.Unauthorized();

            return await appService.UpdateProfileAsync(userIdResult.Value, request, cancellationToken);
        })
        .AddEndpointFilter<ValidationFilter<UpdateUserProfileRequest>>()
        .WithName("UpdateUserProfile")
        .WithSummary("Atualiza o perfil do usuário autenticado")
        .WithTags("Users")
        .RequireAuthorization()
        .WithOpenApi();
    }
}

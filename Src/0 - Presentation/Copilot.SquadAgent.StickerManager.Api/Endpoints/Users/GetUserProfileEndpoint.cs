using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Copilot.SquadAgent.StickerManager.Api.Endpoints.Users;

[ExcludeFromCodeCoverage]
public static class GetUserProfileEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/users/me", async (
            ClaimsPrincipal principal,
            IUserAppService appService,
            CancellationToken cancellationToken) =>
        {
            var userIdResult = AuthorizationHelper.GetUserIdFromClaims(principal);

            if (userIdResult.IsFailure) return Results.Unauthorized();

            return await appService.GetProfileAsync(userIdResult.Value, cancellationToken);
        })
        .WithName("GetUserProfile")
        .WithSummary("Retorna o perfil do usuário autenticado")
        .WithTags("Users")
        .RequireAuthorization()
        .WithOpenApi();
    }
}

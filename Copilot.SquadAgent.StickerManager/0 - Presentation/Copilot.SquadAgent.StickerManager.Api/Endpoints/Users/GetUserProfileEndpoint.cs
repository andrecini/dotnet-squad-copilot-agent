using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;

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
            var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? principal.FindFirstValue("sub");

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            return await appService.GetProfileAsync(userId, cancellationToken);
        })
        .WithName("GetUserProfile")
        .WithSummary("Retorna o perfil do usuário autenticado")
        .WithTags("Users")
        .RequireAuthorization()
        .WithOpenApi();
    }
}

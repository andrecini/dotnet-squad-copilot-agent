using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.Filters;

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
            var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? principal.FindFirstValue("sub");

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            return await appService.UpdateProfileAsync(userId, request, cancellationToken);
        })
        .AddEndpointFilter<ValidationFilter<UpdateUserProfileRequest>>()
        .WithName("UpdateUserProfile")
        .WithSummary("Atualiza o perfil do usuário autenticado")
        .WithTags("Users")
        .RequireAuthorization()
        .WithOpenApi();
    }
}

using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;

namespace Copilot.SquadAgent.StickerManager.Api.Endpoints.Collection;

[ExcludeFromCodeCoverage]
public static class RemoveStickerFromCollectionEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/collection/{id:guid}", async (
            Guid id,
            ClaimsPrincipal principal,
            ICollectionAppService appService,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? principal.FindFirstValue("sub");

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            return await appService.RemoveStickerFromCollectionAsync(userId, id, cancellationToken);
        })
        .WithName("RemoveStickerFromCollection")
        .WithSummary("Remove uma figurinha da coleção do usuário autenticado (soft delete)")
        .WithTags("Collection")
        .RequireAuthorization()
        .WithOpenApi();
    }
}

using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

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
            var userIdResult = AuthorizationHelper.GetUserIdFromClaims(principal);

            if (userIdResult.IsFailure) return Results.Unauthorized();

            return await appService.RemoveStickerFromCollectionAsync(userIdResult.Value, id, cancellationToken);
        })
        .WithName("RemoveStickerFromCollection")
        .WithSummary("Remove uma figurinha da coleção do usuário autenticado (soft delete)")
        .WithTags("Collection")
        .RequireAuthorization()
        .WithOpenApi();
    }
}

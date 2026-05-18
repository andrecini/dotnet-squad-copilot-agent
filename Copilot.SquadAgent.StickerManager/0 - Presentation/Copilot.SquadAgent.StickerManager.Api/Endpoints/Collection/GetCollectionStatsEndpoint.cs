using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Copilot.SquadAgent.StickerManager.Api.Endpoints.Collection;

[ExcludeFromCodeCoverage]
public static class GetCollectionStatsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/collection/stats", async (
            ClaimsPrincipal principal,
            ICollectionAppService appService,
            CancellationToken cancellationToken) =>
        {
            var userIdResult = AuthorizationHelper.GetUserIdFromClaims(principal);

            if (userIdResult.IsFailure) return Results.Unauthorized();

            return await appService.GetCollectionStatsAsync(userIdResult.Value, cancellationToken);
        })
        .WithName("GetCollectionStats")
        .WithSummary("Retorna estatísticas da coleção do usuário autenticado, incluindo progresso por time")
        .WithTags("Collection")
        .RequireAuthorization()
        .WithOpenApi();
    }
}

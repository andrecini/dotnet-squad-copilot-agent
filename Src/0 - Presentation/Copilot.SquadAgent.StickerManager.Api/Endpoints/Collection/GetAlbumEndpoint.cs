using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests.Queries;
using Copilot.SquadAgent.StickerManager.Api.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Copilot.SquadAgent.StickerManager.Api.Endpoints.Collection;

[ExcludeFromCodeCoverage]
public static class GetAlbumEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/album", async (
            [AsParameters] AlbumQueryRequest query,
            ClaimsPrincipal principal,
            ICollectionAppService appService,
            CancellationToken cancellationToken) =>
        {
            var userIdResult = AuthorizationHelper.GetUserIdFromClaims(principal);

            if (userIdResult.IsFailure) return Results.Unauthorized();

            return await appService.GetAlbumAsync(userIdResult.Value, query, cancellationToken);
        })
        .WithName("GetAlbum")
        .WithSummary("Retorna todas as figurinhas do álbum com paginação, ordenação e filtro opcional por time")
        .WithTags("Album")
        .RequireAuthorization()
        .WithOpenApi();
    }
}

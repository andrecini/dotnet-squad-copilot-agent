using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;

namespace Copilot.SquadAgent.StickerManager.Api.Endpoints.Collection;

[ExcludeFromCodeCoverage]
public static class GetAlbumEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/album", async (
            ClaimsPrincipal principal,
            ICollectionAppService appService,
            CancellationToken cancellationToken,
            int page = 1,
            int pageSize = 20,
            bool sortByTeam = false,
            Guid? teamId = null) =>
        {
            var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? principal.FindFirstValue("sub");

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            var query = new AlbumQueryRequest
            {
                Page       = page,
                PageSize   = pageSize,
                SortByTeam = sortByTeam,
                TeamId     = teamId
            };

            return await appService.GetAlbumAsync(userId, query, cancellationToken);
        })
        .WithName("GetAlbum")
        .WithSummary("Retorna todas as figurinhas do álbum com paginação, ordenação e filtro opcional por time")
        .WithTags("Album")
        .RequireAuthorization()
        .WithOpenApi();
    }
}

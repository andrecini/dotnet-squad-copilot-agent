using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests.QueryRequest;
using Copilot.SquadAgent.StickerManager.Api.Filters;
using Copilot.SquadAgent.StickerManager.Api.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Copilot.SquadAgent.StickerManager.Api.Endpoints.Collection;

[ExcludeFromCodeCoverage]
public static class ListCollectionEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/collection", async (
            [AsParameters] CollectionQueryRequest query,
            ClaimsPrincipal principal,
            ICollectionAppService appService,
            CancellationToken cancellationToken) =>
        {
            var userIdResult = AuthorizationHelper.GetUserIdFromClaims(principal);

            if (userIdResult.IsFailure) return Results.Unauthorized();

            return await appService.ListCollectionAsync(userIdResult.Value, query, cancellationToken);
        })
        .AddEndpointFilter<ValidationFilter<CollectionQueryRequest>>()
        .WithName("ListCollection")
        .WithSummary("Lista a coleção do usuário autenticado com filtros e paginação")
        .WithTags("Collection")
        .RequireAuthorization()
        .WithOpenApi();
    }
}

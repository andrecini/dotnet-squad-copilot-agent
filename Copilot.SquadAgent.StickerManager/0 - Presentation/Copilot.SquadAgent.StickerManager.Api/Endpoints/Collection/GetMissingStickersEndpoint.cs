using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests.QueryRequest;
using Copilot.SquadAgent.StickerManager.Api.Filters;
using Copilot.SquadAgent.StickerManager.Api.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Copilot.SquadAgent.StickerManager.Api.Endpoints.Collection;

[ExcludeFromCodeCoverage]
public static class GetMissingStickersEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/collection/missing", async (
            [AsParameters] MissingStickersQueryRequest query,
            ClaimsPrincipal principal,
            ICollectionAppService appService,
            CancellationToken cancellationToken) =>
        {
            var userIdResult = AuthorizationHelper.GetUserIdFromClaims(principal);

            if (userIdResult.IsFailure) return Results.Unauthorized();

            return await appService.ListMissingStickersAsync(userIdResult.Value, query, cancellationToken);
        })
        .AddEndpointFilter<ValidationFilter<MissingStickersQueryRequest>>()
        .WithName("GetMissingStickers")
        .WithSummary("Lista todas as figurinhas que o usuário autenticado ainda não possui, com paginação")
        .WithTags("Collection")
        .RequireAuthorization()
        .WithOpenApi();
    }
}

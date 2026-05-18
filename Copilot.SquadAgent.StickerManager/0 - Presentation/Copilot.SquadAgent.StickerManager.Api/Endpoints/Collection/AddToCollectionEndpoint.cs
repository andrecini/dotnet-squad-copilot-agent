using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.Filters;
using Copilot.SquadAgent.StickerManager.Api.Utils;

namespace Copilot.SquadAgent.StickerManager.Api.Endpoints.Collection;

[ExcludeFromCodeCoverage]
public static class AddToCollectionEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/collection", async (
            AddToCollectionRequest request,
            ClaimsPrincipal principal,
            ICollectionAppService appService,
            CancellationToken cancellationToken) =>
        {
            var userIdResult = AuthorizationHelper.GetUserIdFromClaims(principal);

            if (userIdResult.IsFailure) return Results.Unauthorized();

            return await appService.AddStickerAsync(userIdResult.Value, request, cancellationToken);
        })
        .AddEndpointFilter<ValidationFilter<AddToCollectionRequest>>()
        .WithName("AddToCollection")
        .WithSummary("Adiciona uma figurinha à coleção do usuário autenticado")
        .WithTags("Collection")
        .RequireAuthorization()
        .WithOpenApi();
    }
}

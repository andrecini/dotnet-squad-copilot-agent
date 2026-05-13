using System.Diagnostics.CodeAnalysis;
using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.Filters;

namespace Copilot.SquadAgent.StickerManager.Api.Endpoints.Auth;

[ExcludeFromCodeCoverage]
public static class ResetPasswordEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/auth/reset-password", async (
            ResetPasswordRequest request,
            IUserAppService appService,
            CancellationToken cancellationToken) =>
        {
            return await appService.ResetPasswordAsync(request, cancellationToken);
        })
        .AddEndpointFilter<ValidationFilter<ResetPasswordRequest>>()
        .WithName("ResetPassword")
        .WithSummary("Redefine a senha usando token de recuperação")
        .WithTags("Auth")
        .WithOpenApi();
    }
}

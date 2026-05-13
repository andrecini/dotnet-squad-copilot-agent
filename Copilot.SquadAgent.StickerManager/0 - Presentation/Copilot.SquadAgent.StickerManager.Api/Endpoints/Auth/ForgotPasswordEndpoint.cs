using System.Diagnostics.CodeAnalysis;
using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.Filters;

namespace Copilot.SquadAgent.StickerManager.Api.Endpoints.Auth;

[ExcludeFromCodeCoverage]
public static class ForgotPasswordEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/auth/forgot-password", async (
            ForgotPasswordRequest request,
            IUserAppService appService,
            CancellationToken cancellationToken) =>
        {
            return await appService.ForgotPasswordAsync(request, cancellationToken);
        })
        .AddEndpointFilter<ValidationFilter<ForgotPasswordRequest>>()
        .WithName("ForgotPassword")
        .WithSummary("Solicita recuperação de senha via e-mail")
        .WithTags("Auth")
        .WithOpenApi();
    }
}

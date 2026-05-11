using Copilot.SquadAgent.StickerManager.Api.AppServices;
using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.Endpoints.Auth;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Api;

[ExcludeFromCodeCoverage]
public static class ApiDependency
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ApiDependency).Assembly);
        services.AddValidatorsFromAssembly(typeof(ApiDependency).Assembly);

        services.AddScoped<IUserAppService, UserAppService>();

        return services;
    }

    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        RegisterUserEndpoint.Map(app);

        return app;
    }
}

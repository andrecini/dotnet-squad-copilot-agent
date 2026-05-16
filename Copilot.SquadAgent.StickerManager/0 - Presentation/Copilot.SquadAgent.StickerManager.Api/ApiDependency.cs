using System.Diagnostics.CodeAnalysis;
using System.Text;
using Copilot.SquadAgent.StickerManager.Api.AppServices;
using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.Endpoints.Auth;
using Copilot.SquadAgent.StickerManager.Api.Endpoints.Collection;
using Copilot.SquadAgent.StickerManager.Api.Endpoints.Users;
using Copilot.SquadAgent.StickerManager.Api.Mappings;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Copilot.SquadAgent.StickerManager.Api;

[ExcludeFromCodeCoverage]
public static class ApiDependency
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(ApiDependency).Assembly);
        services.AddValidatorsFromAssembly(typeof(ApiDependency).Assembly);

        services.AddScoped<IUserAppService, UserAppService>();
        services.AddScoped<ICollectionAppService, CollectionAppService>();

        var jwtSettings = configuration.GetSection("Jwt");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"]!,
                    ValidAudience = jwtSettings["Audience"]!,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!)) // NOSONAR - secret provided exclusively via environment variable (Jwt__Secret), never in appsettings
                };
            });

        services.AddAuthorization();

        return services;
    }

    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        RegisterUserEndpoint.Map(app);
        LoginUserEndpoint.Map(app);
        GetUserProfileEndpoint.Map(app);
        UpdateUserProfileEndpoint.Map(app);
        ForgotPasswordEndpoint.Map(app);
        ResetPasswordEndpoint.Map(app);
        AddToCollectionEndpoint.Map(app);
        RemoveStickerFromCollectionEndpoint.Map(app);
        ToggleDuplicateEndpoint.Map(app);
        ListCollectionEndpoint.Map(app);
        GetMissingStickersEndpoint.Map(app);
        GetCollectionStatsEndpoint.Map(app);

        return app;
    }
}

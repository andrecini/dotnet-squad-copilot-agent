using System.Diagnostics.CodeAnalysis;
using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;
using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Security;
using Copilot.SquadAgent.StickerManager.Infrastructure.Data;
using Copilot.SquadAgent.StickerManager.Infrastructure.Repositories.PasswordResetToken;
using Copilot.SquadAgent.StickerManager.Infrastructure.Repositories.Sticker;
using Copilot.SquadAgent.StickerManager.Infrastructure.Repositories.User;
using Copilot.SquadAgent.StickerManager.Infrastructure.Repositories.UserCollection;
using Copilot.SquadAgent.StickerManager.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Copilot.SquadAgent.StickerManager.Infrastructure;

[ExcludeFromCodeCoverage]
public static class InfrastructureDependency
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IStickerRepository, StickerRepository>();
        services.AddScoped<IUserCollectionRepository, UserCollectionRepository>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}

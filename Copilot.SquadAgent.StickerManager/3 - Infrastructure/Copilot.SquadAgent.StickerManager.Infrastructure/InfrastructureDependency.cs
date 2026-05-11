using System.Diagnostics.CodeAnalysis;
using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;
using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Security;
using Copilot.SquadAgent.StickerManager.Infrastructure.Data;
using Copilot.SquadAgent.StickerManager.Infrastructure.Repositories.User;
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
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        return services;
    }
}

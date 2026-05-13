using Copilot.SquadAgent.StickerManager.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Integration.Tests.Fixtures;

public class IntegrationTestFixture : WebApplicationFactory<Copilot.SquadAgent.StickerManager.Api.Program>, IAsyncLifetime
{
    private readonly string _dbName = $"TestDb_{Guid.NewGuid()}";
    public HttpClient Client { get; private set; } = null!;

    static IntegrationTestFixture()
    {
        // Variáveis de ambiente são lidas ANTES do registro de serviços (AddApi, AddInfrastructure)
        Environment.SetEnvironmentVariable("Jwt__Secret", "integration-test-secret-key-32chars!");
        Environment.SetEnvironmentVariable("Jwt__Issuer", "StickerManagerApi");
        Environment.SetEnvironmentVariable("Jwt__Audience", "StickerManagerClient");
        // Conexão válida sintaticamente para o Npgsql não rejeitar; será substituída pelo InMemory abaixo
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", "Host=localhost;Database=test_db;Username=test;Password=test");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove todas as opções e registros do AppDbContext para substituir pelo InMemory
            var toRemove = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                    d.ServiceType == typeof(AppDbContext))
                .ToList();

            foreach (var descriptor in toRemove)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));
        });
    }

    public async Task InitializeAsync()
    {
        Client = CreateClient();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        Client?.Dispose();
        await base.DisposeAsync();
    }
}

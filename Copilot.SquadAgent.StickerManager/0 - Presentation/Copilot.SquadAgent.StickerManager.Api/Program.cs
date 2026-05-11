using Copilot.SquadAgent.StickerManager.Application;
using Copilot.SquadAgent.StickerManager.Domain;
using Copilot.SquadAgent.StickerManager.Infrastructure;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Api
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateBootstrapLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .WriteTo.Console());

                builder.Services
                    .AddDomain()
                    .AddApplication()
                    .AddInfrastructure(builder.Configuration)
                    .AddApi();

                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new() { Title = "Sticker Manager API", Version = "v1" });
                });

                var app = builder.Build();

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseSerilogRequestLogging();
                app.UseHttpsRedirection();

                app.MapApiEndpoints();

                app.Run();
            }
            catch (Exception ex) when (ex is not HostAbortedException)
            {
                Log.Fatal(ex, "Aplicação encerrada inesperadamente.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
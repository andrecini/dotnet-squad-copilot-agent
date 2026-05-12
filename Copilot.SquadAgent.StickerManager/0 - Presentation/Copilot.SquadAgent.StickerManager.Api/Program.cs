using Copilot.SquadAgent.StickerManager.Application;
using Copilot.SquadAgent.StickerManager.Domain;
using Copilot.SquadAgent.StickerManager.Domain.Entities;
using Copilot.SquadAgent.StickerManager.Infrastructure;
using Microsoft.OpenApi.Models;
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
                    .AddApi(builder.Configuration);

                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new() { Title = "Sticker Manager API", Version = "v1" });

                    var securityScheme = new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Informe o token JWT. Exemplo: Bearer {token}",
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    };

                    options.AddSecurityDefinition("Bearer", securityScheme);
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        { securityScheme, [] }
                    });
                });

                var app = builder.Build();

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseSerilogRequestLogging();
                app.UseHttpsRedirection();
                app.UseAuthentication();
                app.UseAuthorization();

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
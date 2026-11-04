using LibrarySystem.Api.Extensions;
using LibrarySystem.Api.Filters;
using LibrarySystem.Api.Identity;
using LibrarySystem.Api.Middlewares;
using Microsoft.OpenApi;
using ServiceCollectionExtensions = LibrarySystem.Api.Extensions.ServiceCollectionExtensions;

namespace LibrarySystem.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Controllers + Global Action Filter
        builder.Services.AddControllers(options =>
        {
            options.Filters.AddService<ActionLoggingFilter>();
        });

        // OpenAPI
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();

        // Swagger + JWT Bearer
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Enter your JWT token."
                });

            options.AddSecurityRequirement(document =>
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(
                        "Bearer", document)] = []
                });
        });

        // Register all application services.
        builder.Services.AddLibrarySystem(
            builder.Configuration);

        // Logging
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        var app = builder.Build();

        // Seed Identity Roles and initial Admin.
        using (var scope = app.Services.CreateScope())
        {
            await IdentitySeeder.SeedAsync(
                scope.ServiceProvider,
                app.Configuration);
        }

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Middlewares
        app.UseMiddleware<RequestLoggingMiddleware>();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();

        app.UseRouting();

        // CORS
        app.UseCors(
            ServiceCollectionExtensions.FrontendCorsPolicy);

        // Authentication must run before Authorization.
        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }
}
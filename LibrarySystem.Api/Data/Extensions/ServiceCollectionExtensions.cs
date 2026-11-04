using AutoMapper;
using LibrarySystem.Api.Data;
using LibrarySystem.Api.Data.Interceptors;
using LibrarySystem.Api.Data.Repositories;
using LibrarySystem.Api.Data.Services;
using LibrarySystem.Api.Filters;
using LibrarySystem.Api.Mappings;
using LibrarySystem.Api.Models;
using LibrarySystem.Api.Services;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Api.Authentication;
using LibrarySystem.Api.Data.Services;
using LibrarySystem.Api.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace LibrarySystem.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public const string FrontendCorsPolicy = "FrontendCorsPolicy";
    public static IServiceCollection AddLibrarySystem(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");
        }

        //cors

        services.AddCors(opt =>
        {
            opt.AddPolicy(FrontendCorsPolicy, policy =>
            {
                policy.WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173",
                "http://localhost:3000",
                "https://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
            });
        });

        //actionfilter
            services.AddScoped<ActionLoggingFilter>();

        // EF Core Interceptor
        services.AddSingleton<LoggingSaveChangesInterceptor>();

        // DbContext
        services.AddDbContext<LibraryDbContext>(
            (serviceProvider, options) =>
            {
                options.UseSqlServer(connectionString);

                options.AddInterceptors(
                    serviceProvider.GetRequiredService<
                        LoggingSaveChangesInterceptor>());
            });

        // AutoMapper
        services.AddAutoMapper(
            cfg => { },
            typeof(MappingProfile).Assembly);

        // Generic Repository
        services.AddScoped(
            typeof(IGenericRepository<>),
            typeof(GenericRepository<>));

        // Specific Repositories
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IBorrowerRepository, BorrowerRepository>();
        services.AddScoped<ILoanRepository, LoanRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IBorrowerService, BorrowerService>();
        services.AddScoped<ILoanService, LoanService>();

        // JWT settings
        var jwtSection = configuration.GetSection(
            JwtOptions.SectionName);

        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var secretKey = jwtSection["SecretKey"];

        if (string.IsNullOrWhiteSpace(issuer) ||
            string.IsNullOrWhiteSpace(audience) ||
            string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException(
                "JWT settings are missing.");
        }

        if (Encoding.UTF8.GetByteCount(secretKey) < 32)
        {
            throw new InvalidOperationException(
                "JWT SecretKey must be at least 32 bytes for HS256.");
        }

        services.Configure<JwtOptions>(jwtSection);

        // Identity
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;

            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan =
                TimeSpan.FromMinutes(10);

            options.SignIn.RequireConfirmedEmail = false;
        })
        .AddRoles<ApplicationRole>()
        .AddEntityFrameworkStores<LibraryDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        // JWT Authentication
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = issuer,

                        ValidateAudience = true,
                        ValidAudience = audience,

                        ValidateIssuerSigningKey = true,

                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(secretKey)),

                        ValidateLifetime = true,

                        ClockSkew = TimeSpan.FromSeconds(30),

                        NameClaimType =
                            JwtRegisteredClaimNames.UniqueName,

                        RoleClaimType = "role"
                    };
            });

        // Authorization
        services.AddAuthorization();

        // Authentication Services
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
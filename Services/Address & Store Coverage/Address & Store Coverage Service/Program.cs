using Address___Store_Coverage_Service.Features.Addresses.CreateAddress;
using Address___Store_Coverage_Service.Features.Addresses.DeleteAddress;
using Address___Store_Coverage_Service.Features.Addresses.GetAddressById;
using Address___Store_Coverage_Service.Features.Admin.Stores.CoverageArea.GetCoverageArea;
using Address___Store_Coverage_Service.Features.Admin.Stores.CoverageArea.SetCoverageArea;
using Address___Store_Coverage_Service.Features.Admin.Stores.CreateStore;
using Address___Store_Coverage_Service.Features.Admin.Stores.DeleteStore;
using Address___Store_Coverage_Service.Features.Admin.Stores.GetStoreById;
using Address___Store_Coverage_Service.Features.Admin.Stores.GetStores;
using Address___Store_Coverage_Service.Features.Admin.Stores.UpdateStore;
using Address___Store_Coverage_Service.Features.Addresses.SetDefaultAddress;
using Address___Store_Coverage_Service.Features.Addresses.GetAddresses;
using Address___Store_Coverage_Service.Features.Addresses.UpdateAddress;
using Address___Store_Coverage_Service.Features.Areas;
using Address___Store_Coverage_Service.Features.NearestCoveringStore;
using Address___Store_Coverage_Service.Persistence;
using Address___Store_Coverage_Service.Persistence.Repositories;
using Address___Store_Coverage_Service.Persistence.Seeding;
using Blocks.Contracts.Behaviors;
using Blocks.Contracts.Http;
using Blocks.Contracts.Interfaces;
using Blocks.Contracts.Security;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Text;

namespace Address___Store_Coverage_Service;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. Database Context
        builder.Services.AddDbContext<FlowersAddressStoreCoverageDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null)));

        // Unit of Work & Generic Repository
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // 2. MediatR & FluentValidation Pipeline
        var assembly = typeof(Program).Assembly;
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        builder.Services.AddValidatorsFromAssembly(assembly);

        // 3. Global Exception Handling
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        // JSON options (string enum serialization)
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        });

        // 4. Localization
        builder.Services.AddLocalization();
        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("ar")
            };

            options.DefaultRequestCulture = new RequestCulture("en");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });

        // 5. API & Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Address & Store Coverage API",
                Version = "v1"
            });

            // Swagger Bearer Token support
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // 6. JWT Authentication
        var jwtSection = builder.Configuration.GetSection("JwtSettings");
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSection["Issuer"],
                ValidAudience = jwtSection["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSection["Secret"]!)),
                ClockSkew = TimeSpan.Zero
            };
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(FlowerClaimTypes.AdminPolicy, policy => policy.RequireRole(FlowerClaimTypes.AdminRole));
        });

        var app = builder.Build();

        // Middleware Pipeline
        app.UseExceptionHandler();
        app.UseRequestLocalization();
        app.UseAuthentication();
        app.UseAuthorization();

        // Database Migration on Startup
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();

            var retryCount = 0;
            const int maxRetries = 5;
            while (retryCount < maxRetries)
            {
                try
                {
                    var db = services.GetRequiredService<FlowersAddressStoreCoverageDbContext>();
                    await db.Database.MigrateAsync();
                    await CityAreaSeeder.SeedAsync(db);
                    await StoreSeeder.SeedAsync(db);
                    await CoverageAreaSeeder.SeedAsync(db);
                    logger.LogInformation("Database migrations and data seeding completed successfully.");
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    logger.LogWarning(ex, "Attempt {Retry} of {MaxRetries} failed while applying database migrations or seeding data.", retryCount, maxRetries);
                    if (retryCount >= maxRetries)
                    {
                        logger.LogError(ex, "Failed to apply database migrations or seed data after {MaxRetries} attempts.", maxRetries);
                    }
                    else
                    {
                        await Task.Delay(2000);
                    }
                }
            }
        }

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint(
                "/swagger/v1/swagger.json",
                "Address & Store Coverage API v1");
        });

        app.UseHttpsRedirection();

        app.MapGetAreasWithCitiesEndpoint();
        app.MapCreateAddressEndpoint();
        app.MapGetAddressesEndpoint();
        app.MapGetAddressByIdEndpoint();
        app.MapSetDefaultAddressEndpoint();
        app.MapDeleteAddressEndpoint();
        app.MapUpdateAddressEndpoint();
        app.MapFindNearestCoveringStoreEndpoint();

        // Admin - Store & Coverage Area Endpoints
        app.MapGetStoresEndpoint();
        app.MapCreateStoreEndpoint();
        app.MapGetStoreByIdEndpoint();
        app.MapUpdateStoreEndpoint();
        app.MapDeleteStoreEndpoint();
        app.MapGetCoverageAreaEndpoint();
        app.MapSetCoverageAreaEndpoint();

        app.MapGet("/", () => Results.Redirect("/swagger"));
        app.MapGet("/health", () => Results.Ok(new
        {
            status = "Healthy",
            service = "Address & Store Coverage Service",
            timestamp = DateTime.UtcNow
        }));

        await app.RunAsync();
    }
}

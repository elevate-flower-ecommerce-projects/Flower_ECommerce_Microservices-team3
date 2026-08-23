using Address___Store_Coverage_Service.Features.Addresses.CreateAddress;
using Address___Store_Coverage_Service.Features.Addresses.GetAddresses;
using Address___Store_Coverage_Service.Features.Addresses.GetAddressById;
using Address___Store_Coverage_Service.Features.Cities;
using Address___Store_Coverage_Service.Features.NearestCoveringStore;
using Address___Store_Coverage_Service.Persistence;
using Address___Store_Coverage_Service.Persistence.Repositories;
using Address___Store_Coverage_Service.Persistence.Seeding;
using Blocks.Contracts.Behaviors;
using Blocks.Contracts.Http;
using Blocks.Contracts.Interfaces;
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
                builder.Configuration.GetConnectionString("DefaultConnection")));

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

        builder.Services.AddAuthorization();

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

            try
            {
                var db = services.GetRequiredService<FlowersAddressStoreCoverageDbContext>();
                await db.Database.MigrateAsync();
                await CityAreaSeeder.SeedAsync(db);
                await StoreSeeder.SeedAsync(db);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while applying database migrations.");
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

        app.MapGetCitiesWithAreasEndpoint();
        app.MapCreateAddressEndpoint();
        app.MapGetAddressesEndpoint();
        app.MapGetAddressByIdEndpoint();
        app.MapFindNearestCoveringStoreEndpoint();

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

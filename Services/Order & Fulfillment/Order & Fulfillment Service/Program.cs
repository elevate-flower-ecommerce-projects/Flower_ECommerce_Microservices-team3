using Blocks.Contracts.Behaviors;
using Blocks.Contracts.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Persistence;
using Order___Fulfillment_Service.Persistence.Repositories;
using Order___Fulfillment_Service.Services;
using System.Globalization;
using System.Text;

namespace Order___Fulfillment_Service;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. Database Context
        builder.Services.AddDbContext<FlowersOrderDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null)));

        // 2. Unit of Work & Generic Repository
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // 3. Typed HTTP Clients for Upstream Microservices
        builder.Services.AddHttpClient<ICartServiceClient, CartServiceClient>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["CartService:BaseUrl"] ?? "http://cart-service:8080");
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        builder.Services.AddHttpClient<IAddressServiceClient, AddressServiceClient>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["AddressService:BaseUrl"] ?? "http://address-service:8080");
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        builder.Services.AddHttpClient<IPaymentServiceClient, PaymentServiceClient>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["PaymentService:BaseUrl"] ?? "http://payment-service:8080");
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        // 4. MediatR & FluentValidation Pipeline (CQRS)
        var assembly = typeof(Program).Assembly;
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        builder.Services.AddValidatorsFromAssembly(assembly);

        // 5. Global Exception Handling
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        // JSON options (string enum serialization)
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        });

        // 6. Localization
        builder.Services.AddLocalization();
        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("ar-EG")
            };

            options.DefaultRequestCulture = new RequestCulture("en-US");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });

        // 7. Authentication & Authorization
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var secret = builder.Configuration["JwtSettings:Secret"] ?? "YOUR_SUPER_SECRET_KEY_CHANGE_IN_PRODUCTION_MIN_32_CHARS";
            var issuer = builder.Configuration["JwtSettings:Issuer"] ?? "FlowersAuth";
            var audience = builder.Configuration["JwtSettings:Audience"] ?? "FlowersApp";

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ClockSkew = TimeSpan.Zero
            };
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(FlowerClaimTypes.AdminPolicy, policy => policy.RequireRole(FlowerClaimTypes.AdminRole));
        });

        // 8. Swagger / OpenAPI Configuration
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Order & Fulfillment API",
                Version = "v1",
                Description = "Order & Fulfillment Microservice handling checkout, order placement, and order management."
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Paste the raw JWT only. Do not include the 'Bearer' prefix.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
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

        var app = builder.Build();

        // Middleware Pipeline
        app.UseExceptionHandler();

        var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("ar-EG") };
        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("en-US"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        });

        app.UseHttpsRedirection();
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
                    var db = services.GetRequiredService<FlowersOrderDbContext>();
                    await db.Database.MigrateAsync();
                    logger.LogInformation("Database migrations for Order Service completed successfully.");
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount >= maxRetries)
                    {
                        logger.LogError(ex, "Failed to apply database migrations for Order Service after {MaxRetries} attempts.", maxRetries);
                    }
                    else
                    {
                        logger.LogWarning("Database migration attempt {Retry}/{MaxRetries} failed. Retrying in 2s...", retryCount, maxRetries);
                        await Task.Delay(2000);
                    }
                }
            }
        }

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order & Fulfillment API v1");
        });

        app.MapGet("/", () => Results.Redirect("/swagger"));
        app.MapGet("/health", () => Results.Ok(new { status = "Healthy", service = "Order & Fulfillment Service", timestamp = DateTime.UtcNow }));

        await app.RunAsync();
    }
}

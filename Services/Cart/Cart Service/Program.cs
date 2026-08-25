using Blocks.Contracts.Behaviors;
using Blocks.Contracts.Http;
using Blocks.Contracts.Interfaces;
using Cart_Service.Persistence;
using Cart_Service.Persistence.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Globalization;

namespace Cart_Service;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. Database Context
        builder.Services.AddDbContext<FlowersCartDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"))
                   .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

        // 2. Unit of Work & Generic Repository
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // 3. MediatR & FluentValidation Pipeline
        var assembly = typeof(Program).Assembly;
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        builder.Services.AddValidatorsFromAssembly(assembly);

        // 4. Global Exception Handling (From Blocks)
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        // 5. Localization
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

        // 6. API & Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Cart API",
                Version = "v1"
            });
        });

        var app = builder.Build();

        app.UseExceptionHandler();
        app.UseRequestLocalization();

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
                    var db = services.GetRequiredService<FlowersCartDbContext>();
                    await db.Database.MigrateAsync();
                    logger.LogInformation("Database migrations completed successfully.");
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    logger.LogWarning(ex, "Attempt {Retry} of {MaxRetries} failed while applying database migrations.", retryCount, maxRetries);
                    if (retryCount >= maxRetries)
                    {
                        logger.LogError(ex, "Failed to apply database migrations after {MaxRetries} attempts.", maxRetries);
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
                "Cart API v1");
        });

        app.UseHttpsRedirection();

        app.MapGet("/", () => Results.Redirect("/swagger"));
        app.MapGet("/health", () => Results.Ok(new { status = "Healthy", service = "Cart Service", timestamp = DateTime.UtcNow }));

        // TODO: هنحط هنا الـ Endpoints بتاعة السلة لما نعملها
        // app.MapUpdateCartItemEndpoint();

        await app.RunAsync();
    }
}
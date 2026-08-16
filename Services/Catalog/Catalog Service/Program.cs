using Blocks.Contracts.Http;
using Blocks.Contracts.Interfaces;
using Catalog_Service.Features.Occasions.GetPaginatedOccasions.Endpoints;
using Catalog_Service.Features.Products.GetProductsByOccasionId.Endpoints;
using Catalog_Service.Persistence;
using Catalog_Service.Persistence.Repositories;
using Catalog_Service.Persistence.Seeding;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using FluentValidation;
using Blocks.Contracts.Behaviors;

namespace Catalog_Service;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. Database Context
        builder.Services.AddDbContext<FlowersCatalogDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")));

        // Unit of Work & Generic Repository
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // 2. Global Exception Handling
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        // 3. Localization
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

        // 4. API & Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Middleware Pipeline
        app.UseExceptionHandler();
        app.UseRequestLocalization();

        // Development-only Auto Migration & Seeding with error handling
        if (app.Environment.IsDevelopment())
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();

                try
                {
                    var db = services.GetRequiredService<FlowersCatalogDbContext>();
                    await db.Database.MigrateAsync();
                    await CatalogDataSeeder.SeedAsync(db);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while applying database migrations or seeding data.");
                    throw;
                }
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    "/swagger/v1/swagger.json",
                    "Catalog API v1");
            });
        }

        app.UseHttpsRedirection();

        app.MapGetActiveOccasionsEndpoint();
        app.MapGetProductsEndpoint();

        await app.RunAsync();
    }
}
using Microsoft.OpenApi.Models;

using Blocks.Contracts.Behaviors;
using Blocks.Contracts.Http;
using Blocks.Contracts.Interfaces;

using Catalog_Service.Features.Categories.GetActiveCategories.Endpoints;
using Catalog_Service.Features.Home.GetSections;
using Catalog_Service.Features.Products.GetProductById;
using Catalog_Service.Features.Occasions.GetPaginatedOccasions.Endpoints;
using Catalog_Service.Features.Products.GetProductsByOccasionId.Endpoints;
using Catalog_Service.Features.Products.GetProductByCategory.Endpoints;

using Catalog_Service.Persistence;
using Catalog_Service.Persistence.Repositories;
using Catalog_Service.Persistence.Seeding;

using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

using System.Globalization;

namespace Catalog_Service;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // =========================================================
        // 1. Database Context
        // =========================================================

        builder.Services.AddDbContext<FlowersCatalogDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")));


        // =========================================================
        // 2. Unit of Work & Generic Repository
        // =========================================================

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddScoped(
            typeof(IGenericRepository<>),
            typeof(GenericRepository<>));


        // =========================================================
        // 3. MediatR & FluentValidation
        // =========================================================

        var assembly = typeof(Program).Assembly;

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        builder.Services.AddValidatorsFromAssembly(assembly);


        // =========================================================
        // 4. Global Exception Handling
        // =========================================================

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        builder.Services.AddProblemDetails();


        // =========================================================
        // 5. Localization
        // =========================================================

        builder.Services.AddLocalization();

        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("ar")
            };

            options.DefaultRequestCulture =
                new RequestCulture("en");

            options.SupportedCultures =
                supportedCultures;

            options.SupportedUICultures =
                supportedCultures;
        });


        // =========================================================
        // 6. Swagger
        // =========================================================

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Catalog API",
                Version = "v1"
            });
        });


        // =========================================================
        // 7. Build Application
        // =========================================================

        var app = builder.Build();


        // =========================================================
        // 8. Middleware
        // =========================================================

        app.UseExceptionHandler();

        app.UseRequestLocalization();

        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint(
                "/swagger/v1/swagger.json",
                "Catalog API v1");
        });

        app.UseHttpsRedirection();


        // =========================================================
        // 9. Database Migration & Seeding
        // =========================================================

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var logger =
                services.GetRequiredService<ILogger<Program>>();

            try
            {
                var db =
                    services.GetRequiredService<FlowersCatalogDbContext>();

                await db.Database.MigrateAsync();

                await CatalogDataSeeder.SeedAsync(db);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "An error occurred while applying database migrations or seeding data.");
            }
        }


        // =========================================================
        // 10. Endpoints
        // =========================================================

        app.MapGet(
            "/",
            () => Results.Redirect("/swagger"));

        // Home
        app.MapGetHomeSectionsEndpoint();

        // Products
        app.MapProductEndpoints();

        // Occasions
        app.MapGetActiveOccasionsEndpoint();

        // Products by Occasion
        app.MapGetProductsEndpoint();

        // Categories
        app.MapGetActiveCategoriesEndpoint();

        // Products by Category
        app.MapGetProductsByCategoryEndpoint();


        // =========================================================
        // 11. Run Application
        // =========================================================

        await app.RunAsync();
    }
}
using Blocks.Contracts.Behaviors;
using Blocks.Contracts.Http;
using Blocks.Contracts.Payment;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Payment_Service.Application.Abstractions;
using Payment_Service.Features;
using Payment_Service.Features.Commands.CreateCheckoutSession;
using Payment_Service.Features.Commands.HandlePaymentWebhook;
using Payment_Service.Features.Queries.PaymentStatus;
using Payment_Service.Infrastructure;
using Payment_Service.Infrastructure.Paymob;
using Payment_Service.Persistence;
using Payment_Service.Persistence.Repositories;
using Payment_Service.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Payment API",
        Version = "v1",
        Description = "Payment Microservice handling Paymob card checkout sessions, Cash on Delivery, and payment webhooks."
    });
});

// Database
builder.Services.AddDbContext<FlowersPaymentDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)));

// Paymob Options & Typed HTTP Client
builder.Services.Configure<PaymobOptions>(
    builder.Configuration.GetSection(PaymobOptions.SectionName));

builder.Services.AddHttpClient<IPaymobClient, PaymobClient>(client =>
{
    var baseUrl = builder.Configuration["Paymob:BaseUrl"] ?? "https://accept.paymob.com/api/";
    if (!baseUrl.EndsWith('/'))
    {
        baseUrl += "/";
    }
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IOrderServiceClient, OrderServiceClient>(client =>
{
    var orderUrl = builder.Configuration["OrderService:BaseUrl"]
                ?? builder.Configuration["Services:Order"]
                ?? "http://order-service:8080";
    client.BaseAddress = new Uri(orderUrl);
    client.Timeout = TimeSpan.FromSeconds(15);
});

// Repositories & Unit of Work
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Payment Gateway & Checkout URL Builder
builder.Services.AddScoped<IPaymentGateway, PaymentGateway>();
builder.Services.AddScoped<IPaymobCheckoutUrlBuilder, PaymobCheckoutUrlBuilder>();

// MediatR & FluentValidation Pipeline
var assembly = typeof(CreateCheckoutSessionCommand).Assembly;
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(assembly);

// Exception Handling & JSON Options
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

var app = builder.Build();

app.UseExceptionHandler();

// Apply Database Migrations on Startup
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
            var db = services.GetRequiredService<FlowersPaymentDbContext>();
            await db.Database.MigrateAsync();
            logger.LogInformation("Database migrations for Payment Service completed successfully.");
            break;
        }
        catch (Exception ex)
        {
            retryCount++;
            if (retryCount >= maxRetries)
            {
                logger.LogError(ex, "Failed to apply database migrations for Payment Service after {MaxRetries} attempts.", maxRetries);
            }
            else
            {
                logger.LogWarning("Database migration attempt {Retry}/{MaxRetries} failed. Retrying in 2s...", retryCount, maxRetries);
                await Task.Delay(2000);
            }
        }
    }
}

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint(
        "/swagger/v1/swagger.json",
        "Payment API v1");
});

app.UseHttpsRedirection();

// Root & Health
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet(
    "/health",
    () => Results.Ok(new
    {
        status = "Healthy",
        service = "Payment Service",
        timestamp = DateTime.UtcNow
    }));

// Endpoints
app.MapCreateCheckoutSession();
app.MapCreateCodPayment();
app.MapHandlePaymentWebhook();
app.MapPaymentStatusEndpoints();

await app.RunAsync();
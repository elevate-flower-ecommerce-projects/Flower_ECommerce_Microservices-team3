using Blocks.Contracts.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Payment_Service.Application.Abstractions;
using Payment_Service.Features;
using Payment_Service.Features.Commands.CreateCheckoutSession;
using Payment_Service.Features.Commands.HandlePaymentWebhook;
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
        Version = "v1"
    });
});

// Database
builder.Services.AddDbContext<FlowersPaymentDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Paymob
builder.Services.AddHttpClient<IPaymobClient, PaymobClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["Paymob:BaseUrl"]!);
});

// Repositories & Unit of Work
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Payment Gateway
builder.Services.AddScoped<IPaymentGateway, PaymentGateway>();
builder.Services.AddScoped<IPaymobCheckoutUrlBuilder, PaymobCheckoutUrlBuilder>();

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(CreateCheckoutSessionCommand).Assembly);
});

var app = builder.Build();

// Swagger
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint(
        "/swagger/v1/swagger.json",
        "Payment API v1");
});

app.UseHttpsRedirection();

// Health
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

app.Run();
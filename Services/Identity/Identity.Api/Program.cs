using Blocks.Contracts.Interfaces;
using FluentValidation;
using Identity.Api.Authorization;
using Identity.Api.Features.Admin;
using Identity.Api.Features.AdminLogin;
using Identity.Api.Features.ChangePassword;
using Identity.Api.Features.DriverApplication;
using Identity.Api.Features.RefreshToken;
using Identity.Api.Features.Register;
using Identity.Application;
using Identity.Application.Interfaces;
using Identity.Application.Settings;
using Identity.Infrastructure.InfrastructureDependencyInjection;
using Identity.Infrastructure.Persistence.Data;
using Identity.Infrastructure.Persistence.Repositories;
using Identity.Infrastructure.Services;
using Identity.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;

namespace Identity.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<FlowersAuthDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IPasswordService, PasswordService>();
            builder.Services.AddScoped<ISessionService, SessionService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddSingleton<ILoginRateLimiter, LoginRateLimiter>();
            builder.Services.AddMemoryCache();

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings
            {
                Secret = "YOUR_SUPER_SECRET_KEY_CHANGE_IN_PRODUCTION_MIN_32_CHARS",
                Issuer = "FlowersAuth",
                Audience = "FlowersApp",
                AccessTokenExpirationMinutes = 15,
                RefreshTokenExpirationDays = 7
            };

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
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.AdminOnly, policy => policy.RequireRole("Admin"));
            });
            builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, AdminAuthorizationMiddlewareResultHandler>();

            builder.Services.AddLocalization();

            builder.Services.AddApplication();
            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
            builder.Services.AddAppMassTransit(builder.Configuration);
            builder.Services.AddControllers();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("ar-EG")
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<FlowersAuthDbContext>();
                    var passwordService = services.GetRequiredService<IPasswordService>();

                    await context.Database.MigrateAsync();
                    await FlowersAuthSeeder.SeedAsync(context, passwordService);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity API v1");
                });
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRegisterEndpoint();
            app.MapDriverApplicationReviewEndpoints();
            app.MapChangePasswordEndpoint();
            app.MapSubmitDriverApplicationEndpoint();
            app.MapAdminLoginEndpoint();
            app.MapRefreshTokenEndpoint();
            app.MapGet("/health", () => Results.Ok(new { status = "Healthy", service = "Identity Service", timestamp = DateTime.UtcNow }));

            app.Run();
        }
    }
}
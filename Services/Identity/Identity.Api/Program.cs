using System.Globalization;
using System.Text;
using Microsoft.OpenApi.Models;
using Blocks.Contracts.Interfaces;
using FluentValidation;
using Identity.Api.Authorization;
using Identity.Api.Exceptions;
using Identity.Api.Features.Admin;
using Identity.Api.Features.AdminLogin;
using Identity.Api.Features.ChangePassword;
using Identity.Api.Features.RegisterDriver;
using Identity.Api.Features.Forgot_Password;
using Identity.Api.Features.Login;
using Identity.Api.Features.Logout;
using Identity.Api.Features.RefreshToken;
using Identity.Api.Features.Register;
using Identity.Api.Features.Verify_OTP;
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

namespace Identity.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<FlowersAuthDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IDriverRepository, DriverRepository>();
            builder.Services.AddScoped<IDriverApplicationRepository, DriverApplicationRepository>();
            builder.Services.AddScoped<IFileStorageService, FileStorageService>();

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IPasswordService, PasswordService>();
            builder.Services.AddScoped<ISessionService, SessionService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<ILoginRateLimiter, LoginRateLimiter>();
            builder.Services.AddScoped<IDeviceRegistrationService, DeviceRegistrationService>();
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

            builder.Services.AddScoped<AdminLoginRequestVmValidator>();
            builder.Services.AddScoped<RefreshTokenRequestVmValidator>();

            builder.Services.AddScoped<IOtpService, OtpService>();
            builder.Services.AddScoped<IResetTokenService, ResetTokenService>();
            builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            builder.Services.AddSingleton<IHmacService, HmacService>();

            builder.Services.AddApplication();
            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
            builder.Services.AddAppMassTransit(builder.Configuration);
            builder.Services.AddControllers();
            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
            
            
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Identity API",
                    Version = "v1"
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
            app.UseExceptionHandler();

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
                var logger = services.GetRequiredService<ILogger<Program>>();
                const int maxRetries = 10;
                var delay = TimeSpan.FromSeconds(3);

                for (int retry = 1; retry <= maxRetries; retry++)
                {
                    try
                    {
                        var context = services.GetRequiredService<FlowersAuthDbContext>();
                        var passwordService = services.GetRequiredService<IPasswordService>();

                        logger.LogInformation("Applying database migrations for FlowersAuthDbContext (Attempt {Retry}/{MaxRetries})...", retry, maxRetries);
                        try
                        {
                            await context.Database.ExecuteSqlRawAsync(@"
                                IF OBJECT_ID(N'[AdminLoginAudits]') IS NOT NULL
                                BEGIN
                                    IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
                                    BEGIN
                                        CREATE TABLE [__EFMigrationsHistory] (
                                            [MigrationId] nvarchar(150) NOT NULL,
                                            [ProductVersion] nvarchar(32) NOT NULL,
                                            CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
                                        );
                                    END;
                                    IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260813122742_InitialCreate')
                                    BEGIN
                                        INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                                        VALUES ('20260813122742_InitialCreate', '9.0.0');
                                    END;
                                END;

                                IF OBJECT_ID(N'[LoginAttempts]') IS NULL
                                BEGIN
                                    CREATE TABLE [LoginAttempts] (
                                        [Id] uniqueidentifier NOT NULL,
                                        [Email] nvarchar(256) NOT NULL,
                                        [IpAddress] nvarchar(45) NOT NULL,
                                        [IsSuccessful] bit NOT NULL,
                                        [AttemptedAt] datetime2 NOT NULL,
                                        CONSTRAINT [PK_LoginAttempts] PRIMARY KEY ([Id])
                                    );
                                    CREATE INDEX [IX_LoginAttempts_Email] ON [LoginAttempts] ([Email]);
                                    CREATE INDEX [IX_LoginAttempts_IpAddress] ON [LoginAttempts] ([IpAddress]);
                                    CREATE INDEX [IX_LoginAttempts_AttemptedAt] ON [LoginAttempts] ([AttemptedAt]);
                                END;

                                IF OBJECT_ID(N'[UserDevices]') IS NULL
                                BEGIN
                                    CREATE TABLE [UserDevices] (
                                        [Id] uniqueidentifier NOT NULL,
                                        [UserId] uniqueidentifier NOT NULL,
                                        [DeviceId] varchar(128) NOT NULL,
                                        [FcmToken] varchar(512) NOT NULL,
                                        [UpdatedAt] datetime2 NOT NULL,
                                        CONSTRAINT [PK_UserDevices] PRIMARY KEY ([Id]),
                                        CONSTRAINT [FK_UserDevices_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
                                    );
                                    CREATE UNIQUE INDEX [UX_UserDevices_UserId_DeviceId] ON [UserDevices] ([UserId], [DeviceId]);
                                    CREATE UNIQUE INDEX [UX_UserDevices_FcmToken] ON [UserDevices] ([FcmToken]);
                                    CREATE INDEX [IX_UserDevices_UserId_UpdatedAt] ON [UserDevices] ([UserId], [UpdatedAt]);
                                END;

                                IF COL_LENGTH('RefreshTokens', 'DeviceId') IS NULL
                                BEGIN
                                    ALTER TABLE [RefreshTokens] ADD [DeviceId] varchar(128) NULL;
                                    CREATE INDEX [IX_RefreshTokens_UserId_DeviceId] ON [RefreshTokens] ([UserId], [DeviceId]);
                                END;
                            ");
                        }
                        catch (Exception histEx)
                        {
                            logger.LogWarning("Migration history check warning: {Message}", histEx.Message);
                        }

                        await context.Database.MigrateAsync();

                        await FlowersAuthSeeder.SeedAsync(context, passwordService);
                        logger.LogInformation("Database migration and seeding completed successfully.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (retry == maxRetries)
                        {
                            logger.LogError(ex, "An error occurred while applying database migrations or seeding data after {MaxRetries} attempts.", maxRetries);
                            throw;
                        }
                        logger.LogWarning("Database migration attempt {Retry}/{MaxRetries} failed: {Message}. Retrying in {Delay}s...", retry, maxRetries, ex.Message, delay.TotalSeconds);
                        await Task.Delay(delay);
                    }
                }
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity API v1");
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRegisterEndpoint();
            app.MapDriverApplicationReviewEndpoints();
            app.MapChangePasswordEndpoint();
            app.MapSubmitDriverApplicationEndpoint();
            app.MapForgotPasswordEndpoint();
            app.MapVerifyOTPEndpoint();
            app.MapResetPasswordEndpoint();
            app.MapAdminLoginEndpoint();
            app.MapLoginEndpoint();
            app.MapRefreshTokenEndpoint();
            app.MapLogoutEndpoint();
            app.MapGet("/", () => Results.Redirect("/swagger"));
            app.MapGet("/health", () => Results.Ok(new { status = "Healthy", service = "Identity Service", timestamp = DateTime.UtcNow }));

            app.Run();
        }
    }
}

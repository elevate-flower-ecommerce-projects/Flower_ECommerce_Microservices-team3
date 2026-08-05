using Blocks.Contracts.Interfaces;
using FluentValidation;
using Identity.Api.Features.Register;
using Identity.Application;
using Identity.Application.Interfaces;
using Identity.Infrastructure.Persistence.Data;
using Identity.Infrastructure.Persistence.Repositories;
using Identity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

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

            builder.Services.AddApplication();
            builder.Services.AddControllers();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<FlowersAuthDbContext>();
                    var passwordService = services.GetRequiredService<IPasswordService>();

                    await context.Database.EnsureCreatedAsync();
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
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseAuthorization();
            app.MapRegisterEndpoint();
            app.Run();
        }
    }
}
using Cart_Service.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Cart_Service;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. Database Context
        builder.Services.AddDbContext<FlowersCartDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")));
        // Add services to the container.
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

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cart API v1");
        });

        app.UseHttpsRedirection();

        app.MapGet("/", () => Results.Redirect("/swagger"));
        app.MapGet("/health", () => 
                Results.Ok(new { status = "Healthy", 
                                 service = "Cart Service", 
                                 timestamp = DateTime.UtcNow 
                               }
                  ));

        app.Run();
    }
}
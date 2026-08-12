using Catalog_Service.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //builder.Services.AddDbContext<FlowersCatalogDbContext>(options =>
        //    options.UseSqlServer(
        //        builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    "/swagger/v1/swagger.json",
                    "Catalog API v1");
            });
        }

        app.UseHttpsRedirection();

        app.Run();
    }
}
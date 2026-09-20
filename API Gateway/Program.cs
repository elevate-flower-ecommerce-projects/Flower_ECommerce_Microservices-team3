using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste the raw JWT only. Do not include the 'Bearer' prefix."
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

// Add CORS policy to allow Swagger Editor and frontend requests
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configure YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway");
    c.SwaggerEndpoint("/identity/swagger/v1/swagger.json", "Identity API");
    c.SwaggerEndpoint("/catalog/swagger/v1/swagger.json", "Catalog API");
    c.SwaggerEndpoint("/cart/swagger/v1/swagger.json", "Cart API");
    c.SwaggerEndpoint("/address/swagger/v1/swagger.json", "Address & Store Coverage API");
    c.SwaggerEndpoint("/order/swagger/v1/swagger.json", "Order API");
    c.SwaggerEndpoint("/payment/swagger/v1/swagger.json", "Payment API");
});

// Enable CORS middleware before HttpsRedirection & ReverseProxy
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", service = "API Gateway", timestamp = DateTime.UtcNow }));

// Map YARP Reverse Proxy
app.MapReverseProxy();

app.Run();


using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using PersonalBlogServer.Models;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// Database
// ============================================

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

Console.WriteLine("====================================");
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"Connection: {connectionString}");
Console.WriteLine("====================================");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);
// ============================================
// Controllers
// ============================================

builder.Services.AddControllers();

// ============================================
// Swagger
// ============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:9000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token: Bearer {your token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();
var enableSwagger =
    app.Environment.IsDevelopment() ||
    app.Configuration.GetValue<bool>("Swagger:Enabled");

// ============================================
// Swagger
// ============================================

if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ============================================
// Middleware
// ============================================

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapControllers();

app.Run();

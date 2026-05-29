using System.Text;
using LensApi.Models;
using LensApi.Messaging;
using LensApi.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using LensApi;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on port 5126 on all interfaces
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5126);
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:Configuration"] ?? "localhost:6379";
    options.InstanceName = "lensapi:";
});
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddSingleton<RabbitMqCacheInvalidationPublisher>();
var lensConnection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=lens_bd;Username=postgres;Password=postgres";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(lensConnection));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = builder.Configuration["Jwt:Key"] ?? "dev-super-secret-key-change-me";
        var issuer = builder.Configuration["Jwt:Issuer"] ?? "lens-app";
        var audience = builder.Configuration["Jwt:Audience"] ?? "lens-app-users";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };

        // Логирование для отладки
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"[JWT] Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var sub = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                          ?? context.Principal?.FindFirst("sub")?.Value;
                Console.WriteLine($"[JWT] Token validated successfully. UserId: {sub}");
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (token != null)
                {
                    Console.WriteLine($"[JWT] Token received from header: {token.Substring(0, Math.Min(20, token.Length))}...");
                }
                else
                {
                    Console.WriteLine("[JWT] No token in Authorization header");
                }
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddScoped<LensApi.Repositories.ILensRepository, LensApi.Repositories.LensRepository>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                             ?? new[] { "http://localhost:4200", "http://127.0.0.1:4200" };

        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

Console.WriteLine("[STARTUP] Application built successfully");
Console.Out.Flush();

using (var scope = app.Services.CreateScope())
{
    Console.WriteLine("[STARTUP] Creating DB scope...");
    Console.Out.Flush();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    Console.WriteLine("[STARTUP] Got context, ensuring DB created...");
    Console.Out.Flush();
    context.Database.EnsureCreated();
    Console.WriteLine("[STARTUP] DB ensured created");

    // Ensure new auth table exists for existing SQLite databases created before refresh-flow.
    try
    {
        Console.WriteLine("[STARTUP] Creating refresh_tokens table...");
        Console.Out.Flush();
        context.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS refresh_tokens (
                id TEXT PRIMARY KEY,
                user_id TEXT NOT NULL,
                token_hash TEXT NOT NULL,
                expires_at TEXT NOT NULL,
                created_at TEXT NOT NULL,
                revoked_at TEXT NULL,
                replaced_by_token_hash TEXT NULL,
                FOREIGN KEY(user_id) REFERENCES users(id) ON DELETE CASCADE
            );
        ");
        Console.WriteLine("[STARTUP] refresh_tokens table ensured");
        Console.Out.Flush();
    }
    catch (SqliteException ex)
    {
        Console.WriteLine($"[DB] refresh_tokens ensure failed: {ex.Message}");
    }

    if (!context.Users.Any(u => u.Username == "user"))
    {
        Console.WriteLine("[STARTUP] Creating default user...");
        Console.Out.Flush();
        context.Users.Add(new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "user",
            PasswordHash = "1234",
            Mode = "user",
            CreatedAt = DateTime.UtcNow
        });
        context.SaveChanges();
        Console.WriteLine("[STARTUP] Default user created");
        Console.Out.Flush();
    }

    if (!context.Users.Any(u => u.Username == "admin"))
    {
        Console.WriteLine("[STARTUP] Creating default admin...");
        Console.Out.Flush();
        context.Users.Add(new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "admin",
            PasswordHash = "admin",
            Mode = "user",
            CreatedAt = DateTime.UtcNow
        });
        context.SaveChanges();
        Console.WriteLine("[STARTUP] Default admin created");
        Console.Out.Flush();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Add global exception handling middleware as first middleware
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "LensApi"
}));

Console.WriteLine("[STARTUP] All middleware configured, about to run...");
Console.Out.Flush();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

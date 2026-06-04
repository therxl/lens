using System.Text;
using IdentityApi;
using IdentityApi.Data;
using IdentityApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on port 5128
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5128);
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:Configuration"] ?? "localhost:6379";
    options.InstanceName = "identityapi:";
});

var conn = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=lens_bd;Username=postgres;Password=postgres";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(conn));

var jwtKey = builder.Configuration["Jwt:Key"] ?? "dev-super-secret-key-change-me-please";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "lens-app";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "lens-app-users";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGateway", policy =>
    {
        policy
            .WithOrigins("http://localhost:5000", "http://127.0.0.1:5000", "http://localhost:4200", "http://127.0.0.1:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

var forwardOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardOptions.KnownNetworks.Clear();
forwardOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardOptions);

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // Use EF Core migrations in all environments. Replace EnsureCreated() with Migrate()
    // to allow proper schema evolution and rollback via migrations.
    // To create the initial migration locally run:
    //   dotnet tool install --global dotnet-ef
    //   dotnet ef migrations add InitialCreate -p backend/IdentityApi -s backend/IdentityApi
    //   dotnet ef database update -p backend/IdentityApi -s backend/IdentityApi
    try
    {
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Warning: database migration failed at startup: " + ex.Message);
        // In development, fall back to EnsureCreated to allow fast startups
        if (app.Environment.IsDevelopment())
        {
            db.Database.EnsureCreated();
        }
        else
        {
            throw;
        }
    }

    // Seed default users with hashed passwords if not exists
    try
    {
        if (!db.Users.Any(u => u.Username == "user"))
        {
            db.Users.Add(new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "user",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
                Mode = "user",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!db.Users.Any(u => u.Username == "admin"))
        {
            db.Users.Add(new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"),
                Mode = "user",
                CreatedAt = DateTime.UtcNow
            });
        }

        db.SaveChanges();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Warning: seeding failed, attempting EnsureCreated fallback: " + ex.Message);
        if (app.Environment.IsDevelopment())
        {
            db.Database.EnsureCreated();
            // retry seeding after EnsureCreated
            if (!db.Users.Any(u => u.Username == "user"))
            {
                db.Users.Add(new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = "user",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
                    Mode = "user",
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (!db.Users.Any(u => u.Username == "admin"))
            {
                db.Users.Add(new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"),
                    Mode = "user",
                    CreatedAt = DateTime.UtcNow
                });
            }

            db.SaveChanges();
        }
        else
        {
            throw;
        }
    }
}

// Dev-only automatic re-hash removed to avoid accidental password overwrite in non-dev environments.

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowGateway");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "IdentityApi" }));

app.Run();

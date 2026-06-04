using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecommendationsApi.Data;
using RecommendationsApi.Messaging;
using RecommendationsApi.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on port 5127
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5127);
});

builder.Services.AddHttpClient("LensApi", client =>
{
    var lensApiBaseUrl = builder.Configuration["LensApi:BaseUrl"] ?? "http://localhost:5126/";
    client.BaseAddress = new Uri(lensApiBaseUrl);
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:Configuration"] ?? "localhost:6379";
    options.InstanceName = "recommendationsapi:";
});
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddHostedService<RecommendationsCacheInvalidationWorker>();

// Add DbContext
var recommendationsConnection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=lens_bd;Username=postgres;Password=postgres";
builder.Services.AddDbContext<RecommendationsDbContext>(options =>
    options.UseNpgsql(recommendationsConnection));

// Add JWT Authentication
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

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGateway", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5000",
                "http://127.0.0.1:5000",
                "http://localhost:5126",
                "http://127.0.0.1:5126",
                "http://localhost:4200",
                "http://127.0.0.1:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddAuthorization();

var app = builder.Build();

// [STARTUP] Initialize database
Console.WriteLine("[STARTUP] Initializing RecommendationsApi database...");
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RecommendationsDbContext>();
    try
    {
        db.Database.Migrate();
        Console.WriteLine("[STARTUP] Database migrations applied");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Warning: database migration failed at startup: " + ex.Message);
        if (app.Environment.IsDevelopment())
        {
            db.Database.EnsureCreated();
            Console.WriteLine("[STARTUP] Database ensured created (development fallback)");
        }
        else
        {
            throw;
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Add global exception handling middleware
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseRouting();
app.UseCors("AllowGateway");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () => new { status = "Recommendations API is running on port 5127" });

app.Run();

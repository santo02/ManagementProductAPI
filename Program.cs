using ManagementProduct.Data;
using ManagementProduct.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// 1. DATABASE
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=productapi.db"));

// 2. MEMORY CACHE
builder.Services.AddMemoryCache();

// 3. SERVICES
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProductService>();

// 4. JWT AUTHENTICATION
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync(
                    """{"message":"Unauthorized. Token tidak valid atau sudah expired."}""");
            }
        };
    });

builder.Services.AddAuthorization();

// 5. RATE LIMITING
builder.Services.AddRateLimiter(options =>
{
    // Auth: max 3 request per 60 detik
    options.AddFixedWindowLimiter("auth", o =>
    {
        o.PermitLimit = 3;
        o.Window = TimeSpan.FromSeconds(60);
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 0;
    });

    // Write: max 1 request per 5 detik
    options.AddFixedWindowLimiter("write", o =>
    {
        o.PermitLimit = 1;
        o.Window = TimeSpan.FromSeconds(5);
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 0;
    });

    options.RejectionStatusCode = 429;
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync(
            """{"message":"Terlalu banyak request. Silahkan coba lagi nanti."}""",
            cancellationToken);
    };
});

// 6. CORS
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()));

// 7. CONTROLLERS
builder.Services.AddControllers();

var app = builder.Build();

// AUTO CREATE DATABASE
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
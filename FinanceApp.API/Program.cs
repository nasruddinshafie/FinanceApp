using FinanceApp.API.Data;
using FinanceApp.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Finance App API",
        Version = "v1",
        Description = "API for Family Finance Management System"
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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


// Database configuration - using SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Configuring database with connection string: {(string.IsNullOrEmpty(connectionString) ? "NOT SET" : "SET (length: " + connectionString.Length + ")")}");

builder.Services.AddDbContext<FinanceDbContext>(options =>
{
    options.UseSqlServer(
        connectionString,
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
    );
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    options.EnableDetailedErrors();
});

// JWT Authentication configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyHere_ChangeInProduction_AtLeast32Characters";
Console.WriteLine($"JWT Issuer: {jwtSettings["Issuer"] ?? "NOT SET"}");
Console.WriteLine($"JWT Audience: {jwtSettings["Audience"] ?? "NOT SET"}");
Console.WriteLine($"JWT Secret Key: {(string.IsNullOrEmpty(secretKey) ? "NOT SET" : "SET (length: " + secretKey.Length + ")")}");


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
        ValidIssuer = jwtSettings["Issuer"] ?? "FinanceAppAPI",
        ValidAudience = jwtSettings["Audience"] ?? "FinanceAppClient",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();


// Register application services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://finance-app-five-wine.vercel.app") // React dev server
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Add detailed error logging for debugging
var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Global exception handler middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Unhandled exception occurred. Path: {Path}, Method: {Method}",
            context.Request.Path, context.Request.Method);

        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var errorResponse = new
        {
            error = "An internal server error occurred",
            message = app.Environment.IsDevelopment() ? ex.Message : "Please contact support",
            path = context.Request.Path.ToString()
        };

        await context.Response.WriteAsJsonAsync(errorResponse);
    }
});

// Log startup information
logger.LogInformation("Finance App API starting...");
logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{

//}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Finance App API V1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at root
});

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

app.MapGet("/health/db", async (FinanceDbContext dbContext) =>
{
    try
    {
        var canConnect = await dbContext.Database.CanConnectAsync();
        return Results.Ok(new
        {
            status = canConnect ? "Connected" : "Cannot connect",
            timestamp = DateTime.UtcNow,
            database = dbContext.Database.GetDbConnection().Database
        });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database health check failed");
        return Results.Json(new
        {
            status = "Error",
            message = ex.Message,
            innerException = ex.InnerException?.Message,
            timestamp = DateTime.UtcNow
        }, statusCode: 500);
    }
});

logger.LogInformation("Application configured successfully. Starting...");

app.Run();

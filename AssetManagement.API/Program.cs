using AssetManagement.API.Data;
using AssetManagement.API.Endpoints;
using AssetManagement.API.Helpers;
using AssetManagement.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// JWT configuration
var jwtKey = builder.Configuration["Jwt:Key"] ?? "SecretKeyForDevelopmentPurposesOnlyDoNotUseInProduction12345";
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Dependency Injection
builder.Services.AddSingleton<JwtHelper>();
builder.Services.AddSingleton<AssetManagement.API.Services.DispatchReceiptStore>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAssetIdGeneratorService, AssetIdGeneratorService>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IBulkUploadService, BulkUploadService>();
builder.Services.AddHttpClient<IEmailService, EmailService>();
// Chatbot
builder.Services.AddScoped<ChatbotQueryService>();
builder.Services.AddHttpClient<ChatbotService>();

var app = builder.Build();

// Middleware
app.UseCors();
app.UseMiddleware<AssetManagement.API.Middleware.ExceptionMiddleware>();

// Swagger (enabled in all environments)
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapConfigEndpoints();
app.MapAssetEndpoints();
app.MapRequestEndpoints();
app.MapNotificationEndpoints();
app.MapDashboardEndpoints();
app.MapReportEndpoints();
app.MapBulkUploadEndpoints();
app.MapChatbotEndpoints();

// Render port binding
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://*:{port}");

app.Run();
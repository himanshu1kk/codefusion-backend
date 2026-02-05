using CFFFusions.Models;
using CFFFusions.Services;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// 🔑 RAILWAY PORT FIX
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "CFFFusions API", Version = "v1" })
);

// 🔑 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevFrontend", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// 🔑 MongoDB integration (NEW)
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));

builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("Smtp"));

// builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.Configure<BrevoSettings>(
    builder.Configuration.GetSection("BrevoSettings")
);

builder.Services.AddHttpClient();

// ✅ USE BREVO ONLY
builder.Services.AddScoped<IEmailService, BrevoEmailService>();

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IProblemMetaService, ProblemMetaService>();




builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IUserService, UserService>();


// Existing services
builder.Services.AddHttpClient<ICodeforcesClient, CodeforcesClient>();
builder.Services.AddHttpClient<IContestClient, ContestClient>();
builder.Services.AddHttpClient<IProblemClient, ProblemClient>();
builder.Services.AddScoped<ICfAnalyticsService, CfAnalyticsService>();
builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseRouting();
app.UseCors("DevFrontend");

// Health check
app.MapGet("/", () => "CFFFusions API is running 🚀");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CFFFusions API v1");
    c.RoutePrefix = "swagger";
});

app.MapControllers();
app.Run();

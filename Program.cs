using CFFFusions.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 🔑 RAILWAY PORT FIX
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "CFFFusions API", Version = "v1" })
);

// 🔑 CORS (OPEN FOR NOW)
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevFrontend", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddHttpClient<ICodeforcesClient, CodeforcesClient>();
builder.Services.AddHttpClient<IContestClient, ContestClient>();
builder.Services.AddHttpClient<IProblemClient, ProblemClient>();
builder.Services.AddScoped<ICfAnalyticsService, CfAnalyticsService>();
builder.Services.AddMemoryCache();

var app = builder.Build();

// ✅ IMPORTANT ORDER
app.UseRouting();
app.UseCors("DevFrontend");

// ✅ Health endpoint (Railway hits `/`)
app.MapGet("/", () => "CFFFusions API is running 🚀");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CFFFusions API v1");
    c.RoutePrefix = "swagger";
});

app.MapControllers();

app.Run();

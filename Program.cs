using CFFFusions.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "CFFFusions API", Version = "v1" })
);

// Allow your dev frontend origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevFrontend", policy =>
        policy
            .WithOrigins(
                "http://127.0.0.1:5501",
                "http://localhost:5501"   // add if you sometimes use localhost
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            // add .AllowCredentials() only if you send cookies/Authorization with credentials mode
    );
});

// builder.Services.AddTransient<CodeforcesThrottleHandler>();
builder.Services.AddHttpClient<ICodeforcesClient, CodeforcesClient>();
    // .AddHttpMessageHandler<CodeforcesThrottleHandler>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CFFFusions API v1");
    c.RoutePrefix = "swagger";
});

// IMPORTANT: CORS must be before MapControllers
app.UseCors("DevFrontend");

app.MapControllers();

app.Run();

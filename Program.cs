using CFFFusions.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "CFFFusions API", Version = "v1" })
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevFrontend", policy =>
        policy
            .WithOrigins(
                "http://127.0.0.1:5501",
                "http://localhost:5501",
                "http://127.0.0.1:5500",
                "http://localhost:5500"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});



builder.Services.AddHttpClient<ICodeforcesClient, CodeforcesClient>();
builder.Services.AddHttpClient<IContestClient, ContestClient>();
builder.Services.AddMemoryCache();



var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CFFFusions API v1");
    c.RoutePrefix = "swagger";
});


app.UseCors("DevFrontend");


app.MapControllers();


app.Run();

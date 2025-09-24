using Application;
using Infrastructure;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Http.Timeouts;
using MultiTenantApp.API;
using MultiTenantApp.API.Middleware;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddCors(options =>
{
    //options.AddPolicy("AllowFrontend", policy =>
    //{
    //    policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
    //          .AllowAnyHeader()
    //          .AllowAnyMethod()
    //          .AllowCredentials();
    //});
    options.AddPolicy("AllowAll",
            builder =>
            {
                builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
            });
});
builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddRequestTimeouts(options =>
{
    // Timeout por defecto de 30 segundos
    options.DefaultPolicy = new RequestTimeoutPolicy
    {
        Timeout = TimeSpan.FromSeconds(5)
    };

    //// Definir políticas con nombre
    //options.AddPolicy("ShortTimeout", new RequestTimeoutPolicy
    //{
    //    Timeout = TimeSpan.FromSeconds(5)
    //});

    //options.AddPolicy("LongTimeout", new RequestTimeoutPolicy
    //{
    //    Timeout = TimeSpan.FromMinutes(2)
    //});
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
var app = builder.Build();
app.UseRequestTimeouts();


app.MapDefaultEndpoints();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RequestContextLoggingMiddleware>();

app.UseHttpsRedirection();
// Use custom tenant middleware
app.UseTenantMiddleware();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
    app.CreateDbIfNotExists();
app.Run();

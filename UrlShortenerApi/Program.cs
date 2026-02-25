using UrlShortenerApi.Repositories.Interfaces;
using UrlShortenerApi.Repositories;
using UrlShortenerApi.Services.interfaces;
using UrlShortenerApi.Services;
using UrlShortenerApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/url-shortener-api-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog(); // Use Serilog for logging

// Add services to the container.

var configuration = builder.Configuration; // Assign the configuration from the builder

builder.Services.Configure<UrlShortenerServiceOptions>(configuration.GetSection("UrlShortenerServiceOptions"));

builder.Services.AddSingleton<IUrlRepository, FileUrlRepository>();
builder.Services.AddSingleton<IUrlShortenerService, UrlShortenerService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt => opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
{
    Title = "URL Shortener API",
    Description = "Simple RESTful API for shortening URLs",
    Version = "1.0.0",
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

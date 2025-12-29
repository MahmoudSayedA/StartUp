using Web;
using Infrastructure;
using Application;
using Serilog;
using Infrastructure.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplication();
builder.AddInfrastructure();
builder.AddWebServices();

// configure logging

builder.Host.UseSerilog((context, ConfigurationBinder) =>
{
    ConfigurationBinder
        .MinimumLevel.Information()
        .Enrich.With<UtcTimestampEnricher>()
        .WriteTo.Console(outputTemplate: "[{UtcTimestamp:HH:mm:ss} UTC] {Level:u3} {Message:lj}{NewLine}{Exception}")
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Month);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapSwagger();
    app.UseSwagger();
    app.UseSwaggerUI();
    // seed env-data
}

//app.UseHttpsRedirection();

app.UseExceptionHandler(option => { });

app.MapStaticAssets();

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.UseSerilogRequestLogging();

app.Run();


public partial class Program { }


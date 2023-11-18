using Assignment.DosProtection.DM.Interfaces;
using Assignment.DosProtection.DM.Models;
using Assignment.Utils;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilog();

        // Create the configuration object.
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Validate the configuration file.
        ValidateConfiguration(configuration);
        
        // Configure Serilog.
        var logger = new LoggerConfiguration()
            .MinimumLevel.Error()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(path: configuration[Constants.LOG_PATH])
            .Enrich.FromLogContext()
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<IConfiguration>(configuration);

        builder.Services.AddSerilog(logger);
        builder.Services.AddMemoryCache();
        builder.Services.AddTransient<IDosProtectionClient, DosProtectionClient>();
        builder.Services.AddSingleton<IDosProtectionService, DosProtectionService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor |
            ForwardedHeaders.XForwardedProto
        });
        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }

    /// <summary>
    /// Validates the parsable variables in configuration.
    /// </summary>
    /// <exception cref="FormatException"></exception>
    static void ValidateConfiguration(IConfiguration configuration)
    {
        if (!int.TryParse(configuration[Constants.MAX_REQUESTS_PER_FRAME], out _))
        {
            throw new FormatException("MAX_REQUESTS_PER_FRAME value is not configured properly. Shutting down.");
        }

        if (!int.TryParse(configuration[Constants.TIME_FRAME_THRESHOLD], out _))
        {
            throw new FormatException("TIME_FRAME_THRESHOLD value is not configured properly. Shutting down.");
        }
    }
}
using Assignment.DosProtection.DM;
using Assignment.DosProtection.DM.Interfaces;
using Assignment.DosProtection.DM.Models;
using Assignment.Utils;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;

internal class Program
{
    private static WebApplication app;
    private static IConfigurationRoot configuration;
    private static ILogger<Program> log;

    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilog();

        // Build configuration.
        BuildConfiguration();

        // Validate the configuration file.
        ValidateConfiguration(configuration);

        // Configure Serilog.
        Serilog.Core.Logger logger = ConfigureSerilog();

        // Create Logger instance for internal use
        log = app.Services.GetService<ILogger<Program>>();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Configure configuration and logging for dependency injection.
        builder.Services.AddSingleton<IConfiguration>(configuration);
        builder.Services.AddSerilog(logger);

        // Configure instances for dependency injection.
        builder.Services.AddMemoryCache();
        builder.Services.AddTransient<IDosProtectionClient, DosProtectionClient>();
        builder.Services.AddSingleton<IDosProtectionService, DosProtectionService>();
        builder.Services.AddSingleton<KeySignalEvent>();

        app = builder.Build();

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

        // Instantiate a Singleton instance of the KeySignalEvent class
        var processEvent = app.Services.GetRequiredService<KeySignalEvent>();

        // Start a separate task to listen for the exit event
        Task.Run(() =>
        {
            processEvent.HttpRequestReceived += HandleHttpRequest;
        });

        await app.RunAsync();
    }

    /// <summary>
    /// Handles the HTTP request event serving as a key press signal mock and gracefully shuts down the application.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void HandleHttpRequest(object? sender, KeySignalEventArgs e)
    {
        log.LogInformation($"[Program:HandleHttpRequest] Received key: {e.Key}");
        if (string.Equals(e.Key, configuration[Constants.EXIT_KEY], StringComparison.OrdinalIgnoreCase))
        {
            log.LogInformation("[Program:HandleHttpRequest] Received exit key.");
            log.LogInformation("[Program:HandleHttpRequest] Performing a controlled and graceful shutdown.");
            var hostApplicationLifetime = app.Services.GetService<IHostApplicationLifetime>();
            hostApplicationLifetime?.StopApplication();
        }
        else
        {
            log.LogInformation("[Program:HandleHttpRequest] The received key is not the exit key. Ignoring.");
        }
    }

    private static void BuildConfiguration()
    {
        configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    private static Serilog.Core.Logger ConfigureSerilog()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Error()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(path: configuration[Constants.LOG_PATH])
            .Enrich.FromLogContext()
            .CreateLogger();
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
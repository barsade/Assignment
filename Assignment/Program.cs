using Assignment.DosProtection.DM.Interfaces;
using Assignment.DosProtection.DM.Models;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Serilog.Events;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
/*builder.Host.UseSerilog()*/;

//var logger = new LoggerConfiguration()
//    .MinimumLevel.Information()
//    .WriteTo.File(path: @"C:\Users\barsa\OneDrive\שולחן העבודה\מטלה\root.txt")
//    .Enrich.FromLogContext()
//    .CreateLogger();

//builder.Logging.ClearProviders();
//builder.Logging.AddSerilog(logger);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSerilog(logger);
builder.Services.AddTransient<DosClient>();
builder.Services.AddSingleton<ConcurrentDictionary<string, DosClient>>();
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

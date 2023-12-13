using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using RaceRoute.Core;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRaceRouteDb(builder.Configuration);

var app = builder.Build();


app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            context.Response.ContentType = Text.Plain;

            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

            await context.Response.WriteAsync(exceptionHandlerPathFeature.Error.Message);
        });
    });

app.UseRouting();
#pragma warning disable ASP0014 // Suggest using top level route registrations
app.UseEndpoints(_ => {});
#pragma warning restore ASP0014 // Suggest using top level route registrations

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapControllers();
app.MapHealthChecks("/healthz");

var uiStaticMode = app.Configuration.GetSection("UiStaticMode").Value;
switch (uiStaticMode)
{
    case "staticFiles":
        app.UseStaticFiles();
        app.MapFallbackToFile("index.html");
        break;
    case "devServer":
        app.UseSpa(s =>
        {
            s.UseProxyToSpaDevelopmentServer(app.Configuration.GetSection("UiStaticDevServer").Value);
        });
        break;
    default:
        throw new ApplicationException($"Wrong configuration for ui static files: either 'staticFiles' or 'devServer' are valid values for 'UiStaticMode' configuration. Actual is {uiStaticMode}");
}

app.Services.ExecuteDbMigrations();

app.Run();

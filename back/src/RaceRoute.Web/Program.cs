using System.Text.Json.Serialization;
using RaceRoute.Core;
using RaceRoute.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(x => {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRaceRouteDb(builder.Configuration);

var app = builder.Build();

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
        app.UseMiddleware<ProxySpaDevServer>(app.Configuration.GetSection("UiStaticDevServer").Value);
        break;
    default:
        throw new ApplicationException($"Wrong configuration for ui static files: either 'staticFiles' or 'devServer' are valid values for 'UiStaticMode' configuration. Actual is {uiStaticMode}");
}

app.Services.ExecuteDbMigrations();

app.Run();

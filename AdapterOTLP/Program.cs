using Infra.Configuration.Outbound;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenTelemetryAdapter(builder.Configuration);

var app = builder.Build();

app.MapSwagger();
app.UseRouting();

var configOnlyJson = new ConfigurationBuilder()
    .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
    .Build();

foreach (var kvp in configOnlyJson.AsEnumerable())
{
    Console.WriteLine($"{kvp.Key} = {kvp.Value}");
}

Console.WriteLine("|||||||||||||||||||");

using var _activity = Activity.Current?.Source.StartActivity($"PROGRAM");
_activity?.SetTag("Program Activity", "Test");

app.Run();

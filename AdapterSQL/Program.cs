using Infra.OutBound;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSQLAdapter(builder.Configuration); ;

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

app.Run();

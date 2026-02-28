using Platform.Core.Configuration;
using Platform.Worker;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();
var correlationId = Guid.NewGuid().ToString("N");
using (LogContext.PushProperty("CorrelationId", correlationId))
{
    Log.Information("Procesando job...");
}

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.Services.AddSerilog((services, lc) => lc
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    builder.Services.AddHostedService<Worker>();

    builder.Services.AddOptions<DgiiOptions>()
    .Bind(builder.Configuration.GetSection("DGII"))
    .ValidateDataAnnotations()
    .Validate(o => !string.IsNullOrWhiteSpace(o.AuthBaseUrl), "DGII:AuthBaseUrl es requerido")
    .ValidateOnStart();

    builder.Services.Configure<DgiiOptions>(builder.Configuration.GetSection("DGII"));

    var keyVaultUri = builder.Configuration["KeyVault:VaultUri"];
    if (!string.IsNullOrWhiteSpace(keyVaultUri))
    {
        builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
    }

    var host = builder.Build();
    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Worker terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
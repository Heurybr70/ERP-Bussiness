using Serilog;
using Serilog.Events;
using Serilog.Context;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails(options =>
{
    // Ocultar detalles técnicos en prod
    options.IncludeExceptionDetails = (ctx, ex) =>
        builder.Environment.IsDevelopment();

    // Mapea excepciones conocidas a status
    options.MapToStatusCode<ArgumentException>(StatusCodes.Status400BadRequest);
    options.MapToStatusCode<UnauthorizedAccessException>(StatusCodes.Status401Unauthorized);
    options.MapToStatusCode<KeyNotFoundException>(StatusCodes.Status404NotFound);

    // Fallback
    options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);

    // Agrega correlationId al response ProblemDetails
    options.OnBeforeWriteDetails = (ctx, problem) =>
    {
        if (ctx.Items.TryGetValue(Platform.Core.Observability.CorrelationIdConstants.LogPropertyName, out var cid)
            && cid is string s && !string.IsNullOrWhiteSpace(s))
        {
            problem.Extensions["correlationId"] = s;
        }
    };
});

var app = builder.Build();

app.UseProblemDetails();

app.UseSerilogRequestLogging(options =>
{
    // Puedes enriquecer con info adicional
    options.EnrichDiagnosticContext = (diag, http) =>
    {
        if (http.Items.TryGetValue(Platform.Core.Observability.CorrelationIdConstants.LogPropertyName, out var cid))
            diag.Set("CorrelationId", cid);

        diag.Set("RequestPath", http.Request.Path.Value);
        diag.Set("UserAgent", http.Request.Headers.UserAgent.ToString());
    };
});

app.UseMiddleware<Platform.Core.Observability.CorrelationIdMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

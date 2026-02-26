using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Platform.Core.Observability;

public sealed class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        // 1) Si viene de fuera, úsalo. Si no, créalo.
        var correlationId = context.Request.Headers[CorrelationIdConstants.HeaderName].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(correlationId))
            correlationId = Activity.Current?.Id ?? Guid.NewGuid().ToString("N");

        // 2) Exponerlo en HttpContext para otros componentes
        context.Items[CorrelationIdConstants.LogPropertyName] = correlationId;

        // 3) Devolverlo en response header
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationIdConstants.HeaderName] = correlationId!;
            return Task.CompletedTask;
        });

        // 4) Enriquecer logs
        using (LogContext.PushProperty(CorrelationIdConstants.LogPropertyName, correlationId))
        {
            await _next(context);
        }
    }
}
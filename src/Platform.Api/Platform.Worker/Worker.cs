using Serilog.Context;

namespace Platform.Worker
{
    public class Worker(ILogger<Worker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var correlationId = Guid.NewGuid().ToString("N");
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                logger.LogInformation("Procesando job...");
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }

        }
    }
}

using InvoiceGenerator.Contracts.Services;
using InvoiceGenerator.Service.Services;
using InvoiceGenerator.Service.Settings;
using Microsoft.Extensions.Options;

namespace InvoiceGenerator.Service
{
    public class Worker(
        ILogger<Worker> logger,
        IServiceProvider serviceProvider,
        IOptions<AppSettings> appSettings,
        IXlApiService xlApiService) : BackgroundService
    {
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting worker service and logging into XL API.");
            xlApiService.Login();
            logger.LogInformation("Logged into XL API.");

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var settings = appSettings.Value;

            while (!stoppingToken.IsCancellationRequested)
            {
                var interval = TimeSpan.FromMinutes(settings.RunIntervalMinutes);

                if (DateTime.Now.Hour < settings.StartHour || DateTime.Now.Hour >= settings.EndHour)
                {
                    logger.LogInformation("Current time is outside of configured hours ({StartHour}:00 - {EndHour}:00). Waiting for the next interval.", settings.StartHour, settings.EndHour);
                    await Task.Delay(interval, stoppingToken);
                    continue;
                }

                logger.LogInformation("Starting invoice generation.");

                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var invoiceService = scope.ServiceProvider.GetRequiredService<GeneratingService>();
                    await invoiceService.GenerateInvoices();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while processing invoices.");
                }

                logger.LogInformation("Ended invoice generation.");

                await Task.Delay(interval, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                xlApiService.Logout();
                logger.LogInformation("Logged out from XL API.");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to logout from XL API during service shutdown.");
            }

            await base.StopAsync(cancellationToken);
        }
    }
}

using InvoiceGenerator.Contracts.Repositories;
using InvoiceGenerator.Contracts.Services;
using InvoiceGenerator.Contracts.Settings;
using InvoiceGenerator.Infrastructure.Data;
using InvoiceGenerator.Infrastructure.Repositories;
using InvoiceGenerator.Infrastructure.Services;
using InvoiceGenerator.Service;
using InvoiceGenerator.Service.Constants;
using InvoiceGenerator.Service.Logging;
using InvoiceGenerator.Service.Services;
using InvoiceGenerator.Service.Settings;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = ServiceConstants.ServiceName;
    })
    .UseSerilog((hostContext, _, loggerConfiguration) =>
    {
        loggerConfiguration.ConfigureServiceLogging(hostContext.Configuration, ServiceConstants.ServiceName);
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        // Configuration
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        services.Configure<XlApiSettings>(configuration.GetSection("XlApiSettings"));

        // Database context
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddSingleton<IDbExecutor>(sp => new DapperDbExecutor(connectionString));

        // Repositories
        services.AddScoped<IOrderRespository, OrderRespository>();
        services.AddScoped<IAttributeRepository, AttributeRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();

        // Services
        services.AddSingleton<IXlApiService, XlApiService>();
        services.AddScoped<GeneratingService>();

        // Background worker
        services.AddHostedService<Worker>();

        // Host options
        services.Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromSeconds(15));
    })
    .Build();

host.Run();
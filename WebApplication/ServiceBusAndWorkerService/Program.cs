using Microsoft.Extensions.Azure;
using ServiceBusAndWorkerService.MyWorker;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        // services.AddHostedService<Worker>();
        services.AddHostedService<SendMessageWorker>();
        services.AddHostedService<ReceiveMessageWorker>();
        services.AddAzureClients(builder =>
        {
            builder.AddServiceBusClient(configuration["AzureServiceBus1:ConnectionString"])
                .WithName(configuration["AzureServiceBus1:ServiceName"])
                .ConfigureOptions(options =>
                {
                    options.RetryOptions.Delay = TimeSpan.FromMilliseconds(50);
                    options.RetryOptions.MaxDelay = TimeSpan.FromSeconds(5);
                    options.RetryOptions.MaxRetries = 3;
                });
        });
    })
    .Build();

await host.RunAsync();
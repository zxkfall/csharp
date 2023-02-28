using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using QuartzAndEventWorkerService.Job;
using QuartzAndEventWorkerService.service;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // services.AddHostedService<Worker>();
        services.AddHostedService<ReceiveEventJobService>();
        services.AddHostedService<SendEventJobService>();
        services.AddSingleton<IJobFactory, MyJobFactory>();
        services.AddSingleton<SendEventJob>();
        services.AddSingleton<ReceiveEventJob>();
        services.AddEventStoreClient(settings =>
        {
            settings.ConnectivitySettings.Address =
                new Uri("esdb://127.0.0.1:2113?tls=false&keepAliveTimeout=10000&keepAliveInterval=10000");
            // settings.ConnectivitySettings = EventStoreClientConnectivitySettings.Default;
            //     EventStoreClientSettings
            //     .Create("esdb://127.0.0.1:2113?tls=false&keepAliveTimeout=10000&keepAliveInterval=10000");
        });
      
    })
    .Build();

await host.RunAsync();
using QuartzAndEventWorkerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // services.AddHostedService<Worker>();
        services.AddHostedService<ReceiveEventJobService>();
        services.AddHostedService<SendEventJobService>();
    })
    .Build();

await host.RunAsync();
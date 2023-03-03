using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Quartz;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // services.AddHostedService<Worker>();
        var configuration = hostContext.Configuration;
        services.Configure<QuartzOptions>(configuration.GetSection("Quartz"));
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.UseSimpleTypeLoader();
            q.UseInMemoryStore();
            q.UseDefaultThreadPool(options =>
            {
                options.MaxConcurrency = 10;
            });

            //SendJob
            var sendJobName = "send job";
            var jobGroup = "bus group";
            var sendJobKey = new JobKey(sendJobName, jobGroup);
            q.AddJob<SendMessageJob>(sendJobKey, j => j
                .WithDescription("send message job").StoreDurably()
            );
            q.AddTrigger(t => t
                .WithIdentity("Send Trigger")
                .ForJob(sendJobKey)
                .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(3)))
                .WithSimpleSchedule(x =>
                    x.WithInterval(
                            TimeSpan.FromSeconds(configuration.GetValue<int>("Send-Job-Interval:IntervalInSeconds")))
                        .RepeatForever())
                .WithDescription("send message job cron trigger")
            );

            //ReceiveJob
            var receiveJobName = "receive job";
            var receiveJobKey = new JobKey(receiveJobName, jobGroup);
            q.AddJob<ReceiveMessageJob>(receiveJobKey, j => j
                .WithDescription("receive message job").StoreDurably()
            );
            q.AddTrigger(t => t
                .WithIdentity("Receive Trigger")
                .ForJob(receiveJobKey)
                .StartNow()
                .WithCronSchedule(configuration["Receive-Job-CORN:CronExpression"])
                .WithDescription("receive message job cron trigger")
            );
            q.UseTimeZoneConverter();
            q.UseJobAutoInterrupt(options =>
            {
                options.DefaultMaxRunTime = TimeSpan.FromMinutes(5);
            });
        });
        services.AddTransient<SendMessageJob>();
        services.AddTransient<ReceiveMessageJob>();
        services.AddQuartzHostedService(q => 
            q.WaitForJobsToComplete = true);

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

public class SendMessageJob : IJob
{
    private readonly IAzureClientFactory<ServiceBusClient> _serviceBusClientFactory;
    private readonly IConfiguration _configuration;

    public SendMessageJob(IAzureClientFactory<ServiceBusClient> serviceBusClientFactory, IConfiguration configuration)
    {
        _serviceBusClientFactory = serviceBusClientFactory;
        _configuration = configuration;
    }

    public Task Execute(IJobExecutionContext context)
    {
        try
        {
            return _serviceBusClientFactory.CreateClient(_configuration["AzureServiceBus1:ServiceName"])
                .CreateSender(_configuration["AzureServiceBus1:TopicName"])
                .SendMessageAsync(new ServiceBusMessage("Hello World!")
                {
                    ContentType = "text/plain",
                    Subject = "Greetings",
                    MessageId = Guid.NewGuid().ToString(),
                    TimeToLive = TimeSpan.FromMinutes(2)
                });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return Task.CompletedTask;
    }
}

public class ReceiveMessageJob : IJob
{
    private readonly IAzureClientFactory<ServiceBusClient> _serviceBusClientFactory;
    private readonly IConfiguration _configuration;

    public ReceiveMessageJob(IAzureClientFactory<ServiceBusClient> serviceBusClientFactory, IConfiguration configuration)
    {
        _serviceBusClientFactory = serviceBusClientFactory;
        _configuration = configuration;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var processor = _serviceBusClientFactory.CreateClient(_configuration["AzureServiceBus1:ServiceName"])
            .CreateProcessor(_configuration["AzureServiceBus1:TopicName"], 
                _configuration["AzureServiceBus1:SubscriptionName"]);
        processor.ProcessMessageAsync += ProcessMessage;
        processor.ProcessErrorAsync += ProcessError;
        await processor.StartProcessingAsync();
    }
    
    private async Task ProcessMessage(ProcessMessageEventArgs args)
    {
        Console.WriteLine("received message:");
        Console.WriteLine(args.Message.Body.ToString());
        await args.CompleteMessageAsync(args.Message);
    }
    
    private Task ProcessError(ProcessErrorEventArgs args)
    {
        Console.WriteLine("received error:");
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
}
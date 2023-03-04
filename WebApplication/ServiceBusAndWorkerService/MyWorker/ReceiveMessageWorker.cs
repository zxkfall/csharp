using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;

namespace ServiceBusAndWorkerService.MyWorker;

public class ReceiveMessageWorker : BackgroundService
{
    private readonly IAzureClientFactory<ServiceBusClient> _serviceBusClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ReceiveMessageWorker> _logger;

    public ReceiveMessageWorker(IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        IConfiguration configuration, ILogger<ReceiveMessageWorker> logger)
    {
        _serviceBusClientFactory = serviceBusClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var processor = _serviceBusClientFactory.CreateClient(_configuration["AzureServiceBus1:ServiceName"])
            .CreateProcessor(_configuration["AzureServiceBus1:TopicName"],
                _configuration["AzureServiceBus1:SubscriptionName"]);
        processor.ProcessMessageAsync += ProcessMessageAsync;
        processor.ProcessErrorAsync += ProcessErrorAsync;
        return processor.StartProcessingAsync(stoppingToken);
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        _logger.LogWarning("Received message: {MessageBody}", args.Message.Body);
        await args.CompleteMessageAsync(args.Message);
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError("Error occurred: {ErrorMessage}", args.Exception.Message);
        return Task.CompletedTask;
    }
}
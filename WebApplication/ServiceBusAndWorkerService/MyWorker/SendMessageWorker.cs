using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;

namespace ServiceBusAndWorkerService.MyWorker;

public class SendMessageWorker : BackgroundService
{
    private readonly IAzureClientFactory<ServiceBusClient> _serviceBusClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SendMessageWorker> _logger;

    public SendMessageWorker(IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        IConfiguration configuration, ILogger<SendMessageWorker> logger)
    {
        _serviceBusClientFactory = serviceBusClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = _serviceBusClientFactory.CreateClient(_configuration["AzureServiceBus1:ServiceName"]);
        while (!stoppingToken.IsCancellationRequested)
        {
            var messageContent = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            await client.CreateSender(_configuration["AzureServiceBus1:TopicName"])
                .SendMessageAsync(new ServiceBusMessage(messageContent)
                {
                    ContentType = "text/plain",
                    Subject = "Greetings",
                    MessageId = Guid.NewGuid().ToString(),
                    TimeToLive = TimeSpan.FromMinutes(2)
                }, stoppingToken);
            _logger.LogWarning("Sending message: {MessageContent}", messageContent);
            await Task.Delay(5000, stoppingToken);
        }
    }
}
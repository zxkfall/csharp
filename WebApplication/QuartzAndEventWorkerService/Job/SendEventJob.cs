using System.Globalization;
using System.Text.Json;
using EventStore.Client;
using Quartz;
using QuartzAndEventWorkerService.Domain;

namespace QuartzAndEventWorkerService.Job;

// band run many jobs in same time
[DisallowConcurrentExecution]
[PersistJobDataAfterExecution]
public class SendEventJob : IJob
{
    private readonly EventStoreClient _eventStoreClient;
    public SendEventJob(EventStoreClient eventStoreClient)
    {
        _eventStoreClient = eventStoreClient;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var eventData = new EventData(
            Uuid.NewUuid(),
            nameof(UpdateAccountEvent),
            JsonSerializer.SerializeToUtf8Bytes(new UpdateAccountEvent
            {
                EntityId = Guid.NewGuid().ToString("N"),
                UserName = "test",
                Email = "147@gmail.com",
                Password = DateTime.Now.ToString(CultureInfo.InvariantCulture)
            })
        );
        var cancellationToken = new CancellationToken();
        var sendRes = await _eventStoreClient.AppendToStreamAsync(
            "some-stream",
            StreamState.Any,
            new[] { eventData },
            cancellationToken: cancellationToken
        );
        Console.WriteLine($"send: {sendRes.NextExpectedStreamRevision.ToString()}");
    }
}
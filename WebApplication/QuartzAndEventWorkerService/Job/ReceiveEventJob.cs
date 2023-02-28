using System.Text.Json;
using EventStore.Client;
using Quartz;
using QuartzAndEventWorkerService.Domain;

namespace QuartzAndEventWorkerService.Job;

[DisallowConcurrentExecution]
[PersistJobDataAfterExecution]
public class ReceiveEventJob : IJob
{
    private readonly EventStoreClient _eventStoreClient;

    public ReceiveEventJob(EventStoreClient eventStoreClient)
    {
        _eventStoreClient = eventStoreClient;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var result = _eventStoreClient.ReadStreamAsync(
            Direction.Forwards,
            "some-stream",
            StreamPosition.Start,
            cancellationToken: cancellationTokenSource.Token);

        var events = await result.ToListAsync(cancellationTokenSource.Token);
        Console.WriteLine("receive: ");
        foreach (var @event in events)
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            if (@event.Event.EventType == nameof(UpdateAccountEvent))
            {
                var updateAccountEvent = JsonSerializer.Deserialize<UpdateAccountEvent>(@event.Event.Data.ToArray(), jsonSerializerOptions);
                Console.WriteLine($"{updateAccountEvent.EntityId} {updateAccountEvent.UserName} {updateAccountEvent.Email} {updateAccountEvent.Password}");
            }
        }
    }
}
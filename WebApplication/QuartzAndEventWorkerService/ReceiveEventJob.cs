using System.Text.Json;
using EventStore.Client;
using Quartz;

namespace QuartzAndEventWorkerService;

public class ReceiveEventJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var client = ClientHelper.GetInstance().GetClient();
        var result = client.ReadStreamAsync(
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
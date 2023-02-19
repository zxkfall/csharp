// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using EventStore.Client;

Console.WriteLine("Hello, World!");


var eventData = new EventData(
    Uuid.NewUuid(),
    "TestEvent",
    JsonSerializer.SerializeToUtf8Bytes(new TestEvent
    {
        EntityId = Guid.NewGuid().ToString("N"),
        ImportantData = "I wrote my first event!"
    })
);

var client = CreateClient();
var cancellationToken = new CancellationToken();
var sendRes = await client.AppendToStreamAsync(
    "some-stream",
    StreamState.Any,
    new[] { eventData },
    cancellationToken: cancellationToken
);
Console.WriteLine($"send: {sendRes.NextExpectedStreamRevision.ToString()}");

var result = client.ReadStreamAsync(
    Direction.Forwards,
    "some-stream",
    StreamPosition.Start,
    cancellationToken: cancellationToken);

var events = await result.ToListAsync(cancellationToken);
foreach (var @event in events)
{
    var jsonSerializerOptions = new JsonSerializerOptions();
    if (@event.Event.EventType == typeof(TestEvent).ToString())
    {
        var testEvent = JsonSerializer.Deserialize<TestEvent>(@event.Event.Data.ToArray(), jsonSerializerOptions);
        Console.WriteLine($"{testEvent}");
    }
}

EventStoreClient CreateClient()
{
    var settings = EventStoreClientSettings
        .Create("esdb://127.0.0.1:2113?tls=false&keepAliveTimeout=10000&keepAliveInterval=10000");
    var eventStoreClient = new EventStoreClient(settings);
    return eventStoreClient;
}



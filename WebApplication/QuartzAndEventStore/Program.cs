// See https://aka.ms/new-console-template for more information

using System.Globalization;
using System.Text.Json;
using EventStore.Client;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace QuzrtzAndEventStore;
public class Program
{
    public static async Task Main(string[] args)
    {
        //1. create a scheduler factory
        var factory = new StdSchedulerFactory();
        //2. create a scheduler from factory
        var scheduler = await factory.GetScheduler();
        //3. start the scheduler
        await scheduler.Start();
        //4. create a job
        //job name and group name combination must be unique, we can use name and group name to identify a job
        var job1 = "job1";
        var group1 = "group1";
        var job = JobBuilder.Create<ReceiveEventJob>()
            .WithIdentity(job1, group1)
            .Build();
        var name = "job2";
        var job2 = JobBuilder.Create<SendEventJob>()
            .WithIdentity(name, group1)
            .Build();
        //5. create a trigger
        //trigger name and group name combination must be unique, we can use name and group name to identify a trigger
        var triggerName1 = "trigger1";
        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity(triggerName1, group1)
            .StartNow()
            .WithCronSchedule("0/2 * * * * ?")
            .Build();
        var triggerName2 = "trigger2";
        var trigger2 = TriggerBuilder.Create()
            .WithIdentity(triggerName2, group1)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(10)
                .RepeatForever())
            .Build();
        //6. schedule the job with trigger
        await scheduler.ScheduleJob(job, trigger);
        await scheduler.ScheduleJob(job2, trigger2);
        
        // some sleep to show what's happening
        await Task.Delay(TimeSpan.FromSeconds(60));
        await scheduler.PauseJob(new JobKey(job1, group1));
        await scheduler.PauseJobs(GroupMatcher<JobKey>.GroupEquals(group1));
    }
}

public class SendEventJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var eventData = new EventData(
            Uuid.NewUuid(),
            typeof(UpdateAccountEvent).ToString(),
            JsonSerializer.SerializeToUtf8Bytes(new UpdateAccountEvent
            {
                EntityId = Guid.NewGuid().ToString("N"),
                UserName = "test",
                Email = "147@gmail.com",
                Password = DateTime.Now.ToString(CultureInfo.InvariantCulture)
            })
        );
        var client = ClientHelper.GetInstance().GetClient();
        var cancellationToken = new CancellationToken();
        var sendRes = await client.AppendToStreamAsync(
            "some-stream",
            StreamState.Any,
            new[] { eventData },
            cancellationToken: cancellationToken
        );
        Console.WriteLine($"send: {sendRes.NextExpectedStreamRevision.ToString()}");
        // return Console.Out.WriteLineAsync($"Hello World! {DateTime.Now} {context.JobDetail.Key} {context.Trigger.Key}");
    }
}
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
            if (@event.Event.EventType == typeof(UpdateAccountEvent).ToString())
            {
                var updateAccountEvent = JsonSerializer.Deserialize<UpdateAccountEvent>(@event.Event.Data.ToArray(), jsonSerializerOptions);
                Console.WriteLine($"{updateAccountEvent.EntityId} {updateAccountEvent.UserName} {updateAccountEvent.Email} {updateAccountEvent.Password}");
            }
        }
        // return Console.Out.WriteLineAsync($"Hello World! {DateTime.Now} {context.JobDetail.Key} {context.Trigger.Key}");
    }
}

public class UpdateAccountEvent
{
    public string EntityId { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}


public class ClientHelper {
 
    //创建 SingleObject 的一个对象
    private static readonly ClientHelper Instance = new ClientHelper();
 
    //让构造函数为 private，这样该类就不会被实例化
    private ClientHelper(){}
 
    //获取唯一可用的对象
    public static ClientHelper GetInstance(){
        return Instance;
    }
 
    public EventStoreClient GetClient(){
        var settings = EventStoreClientSettings
            .Create("esdb://127.0.0.1:2113?tls=false&keepAliveTimeout=10000&keepAliveInterval=10000");
        var eventStoreClient = new EventStoreClient(settings);
        return eventStoreClient;
    }
}



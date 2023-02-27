using EventStore.Client;
using Quartz;
using Quartz.Impl;

namespace QuartzAndEventWorkerService;

public class ReceiveEventJobService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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
        //5. create a trigger
        //trigger name and group name combination must be unique, we can use name and group name to identify a trigger
        var triggerName1 = "trigger1";
        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity(triggerName1, group1)
            .StartNow()
            .WithCronSchedule("0/2 * * * * ?")
            .Build();
        //6. schedule the job with trigger
        await scheduler.ScheduleJob(job, trigger, stoppingToken);
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
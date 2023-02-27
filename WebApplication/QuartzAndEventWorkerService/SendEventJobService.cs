using Quartz;
using Quartz.Impl;

namespace QuartzAndEventWorkerService;

public class SendEventJobService : BackgroundService
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
        var job1 = "job2";
        var group1 = "group1";
        var job = JobBuilder.Create<SendEventJob>()
            .WithIdentity(job1, group1)
            .Build();
        //5. create a trigger
        //trigger name and group name combination must be unique, we can use name and group name to identify a trigger
        var triggerName2 = "trigger2";
        var trigger2 = TriggerBuilder.Create()
            .WithIdentity(triggerName2, group1)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(10)
                .RepeatForever())
            .Build();
        //6. schedule the job with trigger
        await scheduler.ScheduleJob(job, trigger2, stoppingToken);
    }
}
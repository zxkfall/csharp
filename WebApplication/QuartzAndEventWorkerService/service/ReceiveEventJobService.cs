using EventStore.Client;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using QuartzAndEventWorkerService.Job;

namespace QuartzAndEventWorkerService.service;

public class ReceiveEventJobService : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IJobFactory _myJobFactory;


    public ReceiveEventJobService(IConfiguration configuration, IJobFactory myJobFactory )
    {
        _configuration = configuration;
        _myJobFactory = myJobFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //1. create a scheduler factory
        var factory = new StdSchedulerFactory();
        //2. create a scheduler from factory
        var scheduler = await factory.GetScheduler(stoppingToken);
        scheduler.JobFactory = _myJobFactory;
        //3. start the scheduler
        await scheduler.Start(stoppingToken);
        //4. create a job
        //job name and group name combination must be unique, we can use name and group name to identify a job
        var jobName = "job1";
        var groupName = "group1";
        var job = JobBuilder.Create<ReceiveEventJob>()
            .WithIdentity(jobName, groupName)
            .Build();
        //5. create a trigger
        //trigger name and group name combination must be unique, we can use name and group name to identify a trigger
        var triggerName = "trigger1";
        var trigger = TriggerBuilder.Create()
            .WithIdentity(triggerName, groupName)
            .StartNow()
            .WithCronSchedule(_configuration["CORN-Scheduler:CronExpression"])
            .Build();
        //6. schedule the job with trigger
        await scheduler.ScheduleJob(job, trigger, stoppingToken);
    }
}
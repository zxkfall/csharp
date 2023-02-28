using Quartz;
using Quartz.Impl.Calendar;
using QuartzAndEventWorkerService.Job;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        // services.AddHostedService<Worker>();
        services.Configure<QuartzOptions>(configuration.GetSection("Quartz"));
        services.Configure<QuartzOptions>(options =>
        {
            options.Scheduling.IgnoreDuplicates = true; // default: false
            options.Scheduling.OverWriteExistingData = true; // default: true
        });
        services.AddQuartz(q =>
        {
            q.SchedulerId = "Scheduler-Core";
            // we take this from appsettings.json, just show it's possible
            // q.SchedulerName = "QuartzScheduler";
            // as of 3.3.2 this also injects scoped services (like EF DbContext) without problems
            q.UseMicrosoftDependencyInjectionJobFactory();
            // or for scoped service support like EF Core DbContext
            // q.UseMicrosoftDependencyInjectionScopedJobFactory();
            // these are the defaults
            q.UseSimpleTypeLoader();
            q.UseInMemoryStore();
            q.UseDefaultThreadPool(tp => { tp.MaxConcurrency = 10; });
            
            // job1
            var sendJobName = "send job";
            var jobGroup = "awesome group";
            var sendJobKey = new JobKey(sendJobName, jobGroup);
            q.AddJob<SendEventJob>(sendJobKey, j => j
                .WithDescription("my awesome job").StoreDurably()
            );
            // you can add calendars too (requires version 3.2)
            const string calendarName = "myHolidayCalendar";
            q.AddCalendar<HolidayCalendar>(
                name: calendarName,
                replace: true,
                updateTriggers: true,
                x => x.AddExcludedDate(new DateTime(2020, 5, 15))
            );
            q.AddTrigger(t => t
                .WithIdentity("Cron Trigger")
                .ForJob(sendJobKey)
                .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(3)))
                .WithCronSchedule("0/10 * * * * ?")
                .WithDescription("my awesome cron trigger")
                .ModifiedByCalendar(calendarName)
            );

            //job2
            var receiveJobName = "receive job";
            var receiveJobKey = new JobKey(receiveJobName, jobGroup);
            q.AddJob<ReceiveEventJob>(receiveJobKey, j => j
                .WithDescription("my awesome job").StoreDurably()
            );
            q.AddTrigger(t => t
                .WithIdentity("Simple Trigger")
                .ForJob(receiveJobKey)
                .StartNow()
                .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(2)).RepeatForever())
                .WithDescription("my awesome simple trigger")
            );

            // convert time zones using converter that can handle Windows/Linux differences
            q.UseTimeZoneConverter();

            // auto-interrupt long-running job
            q.UseJobAutoInterrupt(options =>
            {
                // this is the default
                options.DefaultMaxRunTime = TimeSpan.FromMinutes(5);
            });
        });
        // we can use options pattern to support hooking your own configuration
        // because we don't use service registration api, 
        // we need to manually ensure the job is present in DI
        services.AddTransient<SendEventJob>();
        services.AddTransient<ReceiveEventJob>();
        // Quartz.Extensions.Hosting allows you to fire background service that handles scheduler lifecycle
        services.AddQuartzHostedService(options =>
        {
            // when shutting down we want jobs to complete gracefully
            options.WaitForJobsToComplete = true;
        });
        
        services.AddEventStoreClient(settings =>
        {
            settings.ConnectivitySettings.Address = new Uri(configuration["EventStore:ConnectionString"]);
            settings.ConnectionName = configuration["EventStore:ConnectionName"];
        });
    })
    .Build();

await host.RunAsync();
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Quartz;
using Quartz.Impl.Calendar;
using WebUseQuartzEventEf.Job;

namespace WebUseQuartzEventEf;

public static class StartupExtension
{
    public static void AddMiddlewares(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEventStoreClient(settings =>
        {
            settings.ConnectivitySettings.Address = new Uri(configuration.GetValue<string>("EventStore:ConnectionString"));
            settings.ConnectionName = configuration["EventStore:ConnectionName"];
        });
    }

    public static void UseHealthCheck(this IApplicationBuilder builder)
    {
        var version = Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;
        var serverName = Environment.MachineName;

        builder.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                var sb = new StringBuilder();
                sb.AppendLine("Name = Account");
                sb.AppendLine($"Application Version = {version}");
                sb.AppendLine($"Status = {report.Status}");
                sb.AppendLine($"Entries = {report.Entries}");
                sb.AppendLine($"TotalDuration = {report.TotalDuration}");
                sb.AppendLine($"Server Name = {serverName}");
                sb.AppendLine();

                sb.AppendLine("Dependencies:");
                foreach (var (key, value) in report.Entries)
                {
                    sb.AppendLine($"{key} = {value.Status}");
                }

                await context.Response.WriteAsync(sb.ToString());
            }
        });
    }

    public static void AddQuartz(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<QuartzOptions>(configuration.GetSection("Quartz"));
        services.Configure<QuartzOptions>(options =>
        {
            options.Scheduling.IgnoreDuplicates = true; // default: false
            options.Scheduling.OverWriteExistingData = true; // default: true
        });
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.UseSimpleTypeLoader();
            q.UseInMemoryStore();
            q.UseDefaultThreadPool(tp => { tp.MaxConcurrency = 10; });

            var sendJobKey = new JobKey(configuration.GetValue<string>("SendJob:Name"),
                configuration.GetValue<string>("SendJob:Group"));
            q.AddJob<SendEventJob>(sendJobKey, j => j
                .WithDescription(configuration.GetValue<string>("SendJob:Description")).StoreDurably()
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
                .WithIdentity(configuration.GetValue<string>("SendJob:Trigger:Identity"))
                .ForJob(sendJobKey)
                .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(3)))
                .WithCronSchedule(configuration.GetValue<string>("SendJob:Trigger:Cron"))
                .WithDescription(configuration.GetValue<string>("SendJob:Trigger:Description"))
                .ModifiedByCalendar(calendarName)
            );

            var receiveJobKey = new JobKey(configuration.GetValue<string>("ReceiveJob:Name"),
                configuration.GetValue<string>("ReceiveJob:Group"));
            q.AddJob<ReceiveEventJob>(receiveJobKey, j => j
                .WithDescription(configuration.GetValue<string>("ReceiveJob:Description")).StoreDurably()
            );
            q.AddTrigger(t => t
                .WithIdentity(configuration.GetValue<string>("ReceiveJob:Trigger:Identity"))
                .ForJob(receiveJobKey)
                .StartNow()
                .WithCronSchedule(configuration.GetValue<string>("ReceiveJob:Trigger:Cron"))
                .WithDescription(configuration.GetValue<string>("ReceiveJob:Trigger:Description"))
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
    }
}
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace WebApplication;

public static class StartupServicesExtension
{
    public static void AddHealthCheckServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks();
        }

        public static void AddMiddlewares(this IServiceCollection services, IConfiguration configuration)
        {
            // services.AddSingleton(new ServiceBusClient(configuration["ServiceBusConnectionString"]));
            // services.AddSingleton(sp =>
            // {
            //     var client = sp.GetService<ServiceBusClient>();
            //     var processor = client.CreateProcessor(configuration["ServiceBusTopicName"], configuration["ServiceBusSubscriptionName"]);
            //     return processor;
            // });
        }

        public static void AddDomainServices(this IServiceCollection services)
        {
            // services.AddScoped<IntegrationTestService>();
            // services.AddScoped<IActionAccount, ActionAccount>();
            // services.AddScoped<IServiceBusHandler, AccountHandler>();
            // services.AddScoped<IActionAccountConfiguration, ActionAccountConfiguration>();
        }

        public static void AddHostedServices(this IServiceCollection services)
        {
            // services.AddAutoMapper(typeof(Program).Assembly);
            // services.AddHostedService<AccountBackgroundService>();
        }

        public static void ConfigureApplicationInsights(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            // serviceCollection.AddApplicationInsightsTelemetry(
            //     new ApplicationInsightsServiceOptions
            //     {
            //         ConnectionString = configuration["AzureAppInsightConnectionString"]
            //     });
            // serviceCollection.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) =>
            // {
            //     module.EnableSqlCommandTextInstrumentation =
            //         configuration.GetValue<bool>("EnableTrackSqlFullQueryTextInApplicationInsights");
            // });
        }

        public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder builder)
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

            return builder;
        }
}
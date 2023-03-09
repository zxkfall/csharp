using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Quartz;
using WebUseQuartzEventEf.Controllers;
using WebUseQuartzEventEf.Domain;

namespace WebUseQuartzEventEf.Test;

public class ControllerTestBase : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    protected readonly CustomWebApplicationFactory<Startup> Factory;
    protected readonly IServiceProvider ServiceProvider;
    protected readonly AccountContext AccountContext;

    protected ControllerTestBase(CustomWebApplicationFactory<Startup> factory)
    {
        Factory = factory;
        // can config client in controller constructor, because factory will get config
        //Controller=>Base
        var mock = new Mock<IAccountService>();
        mock.Setup(x => x.GetAccounts()).ReturnsAsync(new List<Account>());
        factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                ServiceCollectionServiceExtensions.AddScoped<IAccountService>(services, x=>mock.Object);
            });
        }).CreateClient();
        var scope = Factory.Services.CreateScope();
        ServiceProvider = scope.ServiceProvider;
        var scheduler = ServiceProvider.GetRequiredService<ISchedulerFactory>();
        // if not shutdown, will throw exception
        var iScheduler = scheduler.GetScheduler().Result;
        if (!iScheduler.IsShutdown)
        {
            iScheduler.Shutdown();
        }
        AccountContext = ServiceProvider.GetService<AccountContext>()!;
        AccountContext.Database.EnsureDeleted();
    }
}
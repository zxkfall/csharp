using EfExample.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TestExample.Controller;

public class ControllerTestBase : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    protected IServiceProvider ServiceProvider { get; }
    protected AccountContext AccountContext { get; }
    protected CustomWebApplicationFactory<Startup> Factory { get; }

    public ControllerTestBase(CustomWebApplicationFactory<Startup> factory)
    {
        Factory = factory;
        var testScope = Factory.Services.GetService<IServiceScopeFactory>().CreateScope();
        ServiceProvider = testScope.ServiceProvider;
        AccountContext = ServiceProvider.GetRequiredService<AccountContext>();
        AccountContext.Database.EnsureDeleted();
    }
}

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<AccountContext>));

            services.Remove(descriptor);

            services.AddDbContext<AccountContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });
            
            
            var inMemorySettings = new Dictionary<string, string> {
                {"Key1", "Value1"},
                {"Nested:Key1", "NestedValue1"},
                {"Nested:Key2", "NestedValue2"},
            };
            // {
            //     "Key1": "Value1",
            //     "Nested": {
            //         "Key1": "NestedValue1",
            //         "Key2": "NestedValue2"
            //     }
            // }

            var fakeConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            services.AddSingleton<IConfiguration>(fakeConfiguration);
        });
    }
}
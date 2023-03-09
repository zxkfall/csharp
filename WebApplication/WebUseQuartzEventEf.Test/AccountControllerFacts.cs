using System.Net.Http.Json;
using FluentAssertions;
using WebUseQuartzEventEf.Domain;

namespace WebUseQuartzEventEf.Test;

public class AccountControllerFacts : ControllerTestBase
{
    public AccountControllerFacts(CustomWebApplicationFactory<Startup> factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task Test1()
    {
        // var scope = _factory.Services.GetService<IServiceScopeFactory>().CreateScope();
        await AccountContext.Accounts.AddAsync(new Account{UserName = "test",Email = "456"});
        await AccountContext.SaveChangesAsync();
        var httpClient = Factory.CreateClient();
        var response = await httpClient.GetAsync("accounts");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<Account>>();
        result.Count.Should().Be(1);
    }
    
    [Fact]
    public async Task Test2()
    {
        // var scope = _factory.Services.GetService<IServiceScopeFactory>().CreateScope();
        await AccountContext.Accounts.AddAsync(new Account{UserName = "test",Email = "456"});
        await AccountContext.SaveChangesAsync();
        var httpClient = Factory.CreateClient();
        var response = await httpClient.GetAsync("accounts");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<Account>>();
        result.Count.Should().Be(1);
    }
}
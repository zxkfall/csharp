using EfExample.Domain;
using FluentAssertions;

namespace TestExample.Controller;

public class AccountControllerTest : ControllerTestBase
{
    public AccountControllerTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task ShouldReturnAllAccounts()
    {
        var account = new Account {UserName = "test"};
        await AccountContext.Accounts.AddAsync(account);
        await AccountContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();
        var response = await client.GetAsync("accounts");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        responseString.Should().Be("[{\"id\":1,\"userName\":\"test\"}]");
    }
    
    [Fact]
    public async Task ShouldReturnAllAccountsA()
    {
        var account = new Account {UserName = "test"};
        await AccountContext.Accounts.AddAsync(account);
        await AccountContext.SaveChangesAsync();
        
        var client = Factory.CreateClient();
        var response = await client.GetAsync("accounts");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        responseString.Should().Be("[{\"id\":1,\"userName\":\"test\"}]");
    }
}
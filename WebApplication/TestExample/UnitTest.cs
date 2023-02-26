using EfExample.Domain;
using EfExample.Service;
using FluentAssertions;

namespace TestExample;

public class UnitTest
{
    [Fact]
    public void Test1()
    {
        var newAccountContext = UnitTestHelper.NewAccountContext();
        new AccountService(newAccountContext).GetAllAccounts().Result.Should().BeEmpty();
    }
    
    [Fact]
    public async Task Test2()
    {
        var newAccountContext = UnitTestHelper.NewAccountContext();
        var account = new Account {UserName = "test"};
        await newAccountContext.AddAsync(account);
        await newAccountContext.SaveChangesAsync();
        var accounts = new AccountService(newAccountContext).GetAllAccounts().Result;
        accounts.Count.Should().Be(1);
        accounts[0].Should().BeEquivalentTo(new Account {Id = 1,UserName = "test"});
    }
}

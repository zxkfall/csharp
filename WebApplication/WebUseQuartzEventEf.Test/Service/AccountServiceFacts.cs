using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using WebUseQuartzEventEf.Domain;
using WebUseQuartzEventEf.Service;

namespace WebUseQuartzEventEf.Test;

public class AccountServiceFacts
{
    [Fact]
    public void ShouldReturnAccountWhenSaveAccount()
    {
        var accountContext = GetAccountContext();
        var logger = new Mock<ILogger<AccountService>>();
        logger.Setup(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            )
        );
        var accountService = new AccountService(accountContext, logger.Object);
        var result = accountService.SaveAccount("test", "lucy@gmail.com").Result;
        // not work
        // logger.Verify(x => 
        //     x.LogInformation("SaveAccount {AccountId} {AccountUserName}", 
        //         result.Id, result.UserName),
        //     Times.Once);

        logger.Verify(
            m => m.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        result.Should().NotBeNull();
    }

    public static AccountContext GetAccountContext()
    {
        var options = new DbContextOptionsBuilder<AccountContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new AccountContext(options);
    }
}
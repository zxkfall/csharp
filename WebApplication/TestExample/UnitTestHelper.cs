using EfExample.Domain;
using Microsoft.EntityFrameworkCore;

namespace TestExample;

public class UnitTestHelper
{
    public static AccountContext NewAccountContext()
    {
        var options = new DbContextOptionsBuilder<AccountContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AccountContext(options);
    }
}
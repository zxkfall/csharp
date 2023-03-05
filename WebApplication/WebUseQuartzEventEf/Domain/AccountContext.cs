using Microsoft.EntityFrameworkCore;

namespace WebUseQuartzEventEf.Domain;

public class AccountContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }

    public AccountContext(DbContextOptions<AccountContext> options) : base(options)
    {
        
    }
}
using Microsoft.EntityFrameworkCore;

namespace EfExample.Domain;

public class AccountContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }

    // public string DbPath => "Server=localhost;Database=test;Trusted_Connection=True;";

    // private readonly ILogger<AccountContext> _logger;

    public AccountContext(DbContextOptions<AccountContext> options) : base(options)
    {
    }

    //move it if use dependency injection
    // protected override void OnConfiguring(DbContextOptionsBuilder options)
    //     => options.UseSqlite($"Data Source={DbPath}");
}


public class Account
{
    public int Id { get; set; }
    public string UserName { get; set; }
}

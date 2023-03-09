using WebUseQuartzEventEf.Domain;

namespace WebUseQuartzEventEf.Service;

public class AccountService : IAccountService
{
    private readonly AccountContext _accountContext;
    private readonly ILogger<AccountService> _logger;

    public AccountService(AccountContext accountContext, ILogger<AccountService> logger)
    {
        _accountContext = accountContext;
        _logger = logger;
    }

    public async Task<Account> SaveAccount(string userName, string email)
    {
        var result = await _accountContext.AddAsync(new Account { UserName = userName, Email = email });
        await _accountContext.SaveChangesAsync();
        var account = result.Entity;
        _logger.LogInformation("SaveAccount {AccountId} {AccountUserName}", account.Id, account.UserName);
        return account;
    }

    public Task<List<Account>> GetAccounts()
    {
        return Task.FromResult(_accountContext.Accounts.ToList());
    }
}
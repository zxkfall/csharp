using EfExample.Domain;

namespace EfExample.Service;

public interface IAccountService
{
    Task<List<Account>> GetAllAccounts();
}

public class AccountService : IAccountService
{
    private readonly AccountContext _accountContext;

    public AccountService(AccountContext accountContext)
    {
        _accountContext = accountContext;
    }


    public Task<List<Account>> GetAllAccounts()
    {
        return Task.FromResult(_accountContext.Accounts.ToList());
    }
}
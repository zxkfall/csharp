using WebUseQuartzEventEf.Domain;

namespace WebUseQuartzEventEf.Service;

public interface IAccountService
{
    Task<Account> SaveAccount(string userName, string email);
    
    Task<List<Account>> GetAccounts();
}
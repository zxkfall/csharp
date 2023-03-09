using Microsoft.AspNetCore.Mvc;
using WebUseQuartzEventEf.Domain;

namespace WebUseQuartzEventEf.Controllers;

[ApiController]
[Route("accounts")]
public class AccountController : ControllerBase
{

    private readonly ILogger<AccountController> _logger;
    private readonly IAccountService _accountService;

    public AccountController(ILogger<AccountController> logger, IAccountService accountService)
    {
        _logger = logger;
        _accountService = accountService;
    }

    [HttpGet]
    public Task<List<Account>> Get()
    {
        return _accountService.GetAccounts();
    }

    [HttpPost("fromBody")]
    public async Task<Account> Post([FromBody]string userName)
    {
        return await _accountService.SaveAccount(userName);
    }

    [HttpPost("fromDto")]
    public async Task<Account> Post(SimpleAccountDto simpleAccountDto)
    {
        return await _accountService.SaveAccount(simpleAccountDto.UserName);
    }
}

public interface IAccountService
{
    Task<Account> SaveAccount(string userName);
    
    Task<List<Account>> GetAccounts();
}

public class AccountService : IAccountService
{
    private readonly AccountContext _accountContext;

    public AccountService(AccountContext accountContext)
    {
        _accountContext = accountContext;
    }

    public async Task<Account> SaveAccount(string userName)
    {
        var result = await _accountContext.AddAsync(new Account { UserName = userName });
        await _accountContext.SaveChangesAsync();
        return result.Entity;
    }

    public Task<List<Account>> GetAccounts()
    {
        return Task.FromResult(_accountContext.Accounts.ToList());
    }
}

public class SimpleAccountDto
{
    public string UserName { get; set; }
}
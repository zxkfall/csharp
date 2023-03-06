using Microsoft.AspNetCore.Mvc;
using WebUseQuartzEventEf.Domain;

namespace WebUseQuartzEventEf.Controllers;

[ApiController]
[Route("accounts")]
public class AccountController : ControllerBase
{

    private readonly ILogger<AccountController> _logger;
    private readonly AccountContext _accountContext;

    public AccountController(ILogger<AccountController> logger, AccountContext accountContext)
    {
        _logger = logger;
        _accountContext = accountContext;
    }

    [HttpGet]
    public IEnumerable<Account> Get()
    {
        try
        {
            return _accountContext.Accounts.ToList();
        }
        catch (Exception e)
        {
            _logger.LogError("{E} {EMessage}", e, e.Message);
        }

        return new List<Account>();
    }

    [HttpPost("fromBody")]
    public async Task<Account> Post([FromBody]string userName)
    {
        return await SaveAccount(userName);
    }

    [HttpPost("fromDto")]
    public async Task<Account> Post(SimpleAccountDto simpleAccountDto)
    {
        return await SaveAccount(simpleAccountDto.UserName);
    }

    private async Task<Account> SaveAccount(string userName)
    {
        var result = await _accountContext.AddAsync(new Account { UserName = userName });
        await _accountContext.SaveChangesAsync();
        return result.Entity;
    }
}

public class SimpleAccountDto
{
    public string UserName { get; set; }
}
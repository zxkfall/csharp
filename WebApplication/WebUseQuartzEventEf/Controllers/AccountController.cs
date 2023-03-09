using Microsoft.AspNetCore.Mvc;
using WebUseQuartzEventEf.Domain;
using WebUseQuartzEventEf.Service;

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
    public async Task<Account> Post([FromBody]string userName, string email)
    {
        return await _accountService.SaveAccount(userName, email);
    }

    [HttpPost("fromDto")]
    public async Task<Account> Post(SimpleAccountDto simpleAccountDto)
    {
        return await _accountService.SaveAccount(simpleAccountDto.UserName, simpleAccountDto.Email);
    }
}

public class SimpleAccountDto
{
    public string UserName { get; set; }
    public string Email { get; set; }
}
using Microsoft.AspNetCore.Mvc;

namespace FluentNhibernateAndMigration.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IRepository<Account> _accountRepository;

    public AccountController(ILogger<AccountController> logger, IRepository<Account> accountRepository)
    {
        _logger = logger;
        _accountRepository = accountRepository;
    }

    [HttpGet]
    public Task<IEnumerable<Account>> Get()
    {
        return Task.FromResult<IEnumerable<Account>>(_accountRepository.GetAll().ToList());
    }

    [HttpPost]
    public async Task<Account> Post(AccountDto accountDto)
    {
        var account = new Account { Email = accountDto.Email, UserName = accountDto.UserName };
        await _accountRepository.Save(account);
        return account;
    }
}

public class AccountDto
{
    public string UserName { get; set; }
    public string Email { get; set; }
}
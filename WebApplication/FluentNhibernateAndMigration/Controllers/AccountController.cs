using FluentNhibernateAndMigration.Domain;
using FluentNhibernateAndMigration.Repository;
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

    [HttpPut]
    [Route("{Id:int}")]
    public async Task<Account> Put(int Id, AccountDto accountDto)
    {
        var account = new Account { Id = Id, Email = accountDto.Email, UserName = accountDto.UserName };
        await _accountRepository.Update(account);
        return account;
    }
}

public record AccountDto(string UserName, string Email);
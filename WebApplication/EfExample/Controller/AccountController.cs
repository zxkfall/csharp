using EfExample.Domain;
using EfExample.Service;
using Microsoft.AspNetCore.Mvc;

namespace EfExample.Controller;

[ApiController]
[Route("accounts")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Account>>> Get()
    {
        return Ok(await _accountService.GetAllAccounts());
    }
}
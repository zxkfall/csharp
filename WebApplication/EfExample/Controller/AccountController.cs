using EfExample.Domain;
using EfExample.Service;
using Microsoft.AspNetCore.Mvc;

namespace EfExample.Controller;

[ApiController]
[Route("accounts")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IConfiguration _configuration;

    public AccountController(IAccountService accountService, IConfiguration configuration)
    {
        _accountService = accountService;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<ActionResult<List<Account>>> Get()
    {
        return Ok(await _accountService.GetAllAccounts());
    }
    
    [HttpGet("configs")]
    public async Task<ActionResult<List<Account>>> GetConfiguration()
    {
        var key2 = _configuration["Nested:Key1"];
        return Ok(_configuration["Key1"]+" "+key2);
    }
}
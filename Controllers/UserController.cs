using System.Security.Claims;
using System.Transactions;
using CityCard_API.Models.DB;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CityCard_API.Controllers;

[ApiController]
[Route("api/user")]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class UserController : ControllerBase
{
    const int  accountTypeId = 1;
    private readonly CityCardDBContext _dbContext;
    private readonly UserManager<CCUser> _userManager;
    public UserController(CityCardDBContext dBContext, UserManager<CCUser> _userManager){
        _dbContext = dBContext;
        _userManager = _userManager;
    }
    
    [HttpGet("new-account")]
    public async Task<IActionResult> NewAccount(){
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound("User not found.");

        var accountType = await _dbContext.AccountTypes.FindAsync(accountTypeId);
        if (accountType == null)
            return BadRequest("Invalid account type.");

        var newAccount = new Account
        {
            User = user,
            AccountType = accountType,
            Amount = 0f
        };

        _dbContext.Accounts.Add(newAccount);
        await _dbContext.SaveChangesAsync();
        return Ok(new { Message = "Account created successfully", AccountId = newAccount.Id });
    }

    [HttpGet("ammount")]
    public async Task<ActionResult<float>> GetAccountAmmount(){
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

        var account = await _dbContext.Accounts
                            .Where(a => a.User.Id == userId)
                            .FirstOrDefaultAsync();

        if (account == null)
            return NotFound("No account found.");

        return Ok(account.Amount);
    }

    [HttpGet("transactions")]
    public async Task<ActionResult<List<AccountTransaction>>> GetAccountTransactions()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

        var transactions = await _dbContext.Transactions
                            .Where(t => t.Account.User.Id == userId)
                            .ToListAsync();

        return Ok(transactions);
    }
}
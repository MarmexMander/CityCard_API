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
[Authorize(Policy = "UserPolicy")]
public class UserController : ControllerBase
{
    const int  accountTypeId = 1;
    private readonly CityCardDBContext _dbContext;
    private readonly UserManager<CCUser> _userManager;
    public UserController(CityCardDBContext dBContext, UserManager<CCUser> userManager){
        _dbContext = dBContext;
        _userManager = userManager;
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
    public async Task<ActionResult<float>> GetAccountAmmount(string accountId){
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

        var account = await _dbContext.Accounts.Include(a=>a.User).Where(a=>a.Id == Guid.Parse(accountId)).FirstAsync();

        if (account == null)
            return NotFound("No account found.");
        if (account.User.Id != userId)
            return Forbid("It is not yours account");

        return Ok(account.Amount);
    }

    [HttpGet("transactions")]
    public async Task<ActionResult<List<AccountTransaction>>> GetAccountTransactions(string accountId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();
        bool accountExists = await _dbContext.Accounts
            .AnyAsync(a => a.Id == Guid.Parse(accountId) && a.User.Id == userId);
        if(!accountExists)
            return BadRequest("Account not exists or it is not yours");
        var transactions = await _dbContext.Transactions
                            .Where(t => t.Account.Id == Guid.Parse(accountId))
                            .OrderBy(t=>t.Timestamp)
                            .ToListAsync();

        return Ok(transactions);
    }
}
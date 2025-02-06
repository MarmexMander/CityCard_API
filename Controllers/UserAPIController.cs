using System.Security.Claims;
using System.Transactions;
using CityCard_API.Models.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CityCard_API.Controllers;

[ApiController]
[Route("api/{action}")]
[Authorize]
class UserAPIController : ControllerBase
{
    const int  accountTypeId = 1;
    private readonly CityCardDBContext _dbContext;
    public UserAPIController(CityCardDBContext dBContext){
        _dbContext = dBContext;
    }
    
    [HttpGet]
    public async Task<IActionResult> NewAccount(){
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();
        var user = await _dbContext.CCUsers.Include(u => u.Accounts)
        .FirstOrDefaultAsync(u => u.Id == userId);

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

    [HttpGet]
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

    [HttpGet]
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
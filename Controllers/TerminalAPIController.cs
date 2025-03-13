using System.Security.Claims;
using CityCard_API.Models.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace CityCard_API.Controllers;

[ApiController]
[Route("api/terminal")]
[Authorize(Policy = "TerminalPolicy")]
public class TerminalAPIController : ControllerBase
{
    private readonly CityCardDBContext _dbContext;

    public TerminalAPIController(CityCardDBContext dBContext){
        _dbContext = dBContext;
    }
    
    /// <summary>
    /// Top up a user's account.
    /// </summary>
    /// <param name="account">The account ID.</param>
    /// <param name="amount">The amount to top up.</param>
    /// <returns>True if the top-up was successful.</returns>
    [HttpPost("top-up")]
    [SwaggerOperation(Summary = "Top up a user's account", Description = "Increases the account balance by the specified amount.")]
    public async Task<ActionResult<bool>> MakeTransactionTopUp([FromQuery]Guid account, [FromQuery]float amount){
        var accountEntity = await _dbContext.Accounts.FindAsync(account);
        if (accountEntity == null)
            return NotFound("Account not found.");

        if (amount <= 0)
            return BadRequest("Invalid amount.");
        var terminalId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (terminalId == null)
            return Unauthorized("Terminal authentication required.");

        var terminal = await _dbContext.Terminals
            .Include(t => t.LocationTransport)
            .FirstOrDefaultAsync(t => t.Id == Guid.Parse(terminalId));

        accountEntity.Amount += amount;

        var transaction = new AccountTransaction
        {
            Account = accountEntity,
            Amount = amount,
            Timestamp = DateTime.UtcNow,
            Metadata = new TransactionMetadata
            {
                PriceUsed = null, // No specific ticket price for top-ups
                Terminal = terminal // No specific terminal for top-ups
            }
        };

        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();
        return Ok(true);
    }

    /// <summary>
    /// Withdraw from an account (ticket purchase).
    /// </summary>
    /// <param name="account">The account ID.</param>
    /// <returns>True if the withdrawal was successful.</returns>
    [HttpPost("withdraw")]
    [SwaggerOperation(Summary = "Withdraw from an account (ticket purchase)", Description = "Charges the ticket price based on the account type and transport type.")]
    public async Task<ActionResult<bool>> MakeTransactionWithdraw([FromQuery]Guid account){
        var accountEntity = await _dbContext.Accounts
                .Include(a => a.User)
                .Include(a => a.AccountType)
                .FirstOrDefaultAsync(a => a.Id == account);

        if (accountEntity == null)
            return NotFound("Account not found.");

        var terminalId = User.Claims.FirstOrDefault(c => c.Type == "TerminalId")?.Value;
        if (terminalId == null)
            return Unauthorized("Terminal authentication required.");

        var terminal = await _dbContext.Terminals
            .Include(t => t.LocationTransport)
            .FirstOrDefaultAsync(t => t.Id == Guid.Parse(terminalId));

        if (terminal == null || terminal.LocationTransport == null)
            return BadRequest("Invalid terminal or transport type.");

        var price = await _dbContext.Prices.FirstOrDefaultAsync(p =>
            p.AccountType.Id == accountEntity.AccountType.Id &&
            p.TransportType.Id == terminal.LocationTransport.Type.Id);

        if (price == null)
            return BadRequest("No price found for this account type and transport.");

        if (accountEntity.Amount < price.Amount)
            return BadRequest("Insufficient balance.");

        accountEntity.Amount -= price.Amount;

        var transaction = new AccountTransaction
        {
            Account = accountEntity,
            Amount = -price.Amount,
            Timestamp = DateTime.UtcNow,
            Metadata = new TransactionMetadata
            {
                PriceUsed = price,
                Terminal = terminal
            }
        };

        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();
        return Ok(true);
    }
}
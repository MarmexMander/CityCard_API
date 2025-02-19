using System.ComponentModel.DataAnnotations;
using CityCard_API.Models.DB;
using CityCard_API.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CityCard_API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = "AdminPolicy")]
public class AdminAPIController : ControllerBase
{
    private readonly CityCardDBContext _dbContext;
    private readonly UserManager<CCUser> _userManager;
    public AdminAPIController(CityCardDBContext dBContext, UserManager<CCUser> userManager){
        _dbContext = dBContext;
        _userManager = userManager;
    }

    [HttpGet("token")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetToken(){
        var user = await _userManager.GetUserAsync(User);
        var adminToken = await _dbContext.AdminTokens.Where(at => at.AdminId == user.Id).FirstAsync();
        if(adminToken != null)
            return Conflict("You already have a token");
        var token = Tools.GenerateToken();
        adminToken = new(){
            AdminId = user.Id,
            TokenHash = Tools.ComputeHash(token),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow+new TimeSpan(30,0,0,0)
        };
        await _dbContext.AdminTokens.AddAsync(adminToken);
        return Ok(token);
    }

    [HttpPost("add-city")]
    public async Task<IActionResult> AddCity(string city, string region)
    {
        if (string.IsNullOrWhiteSpace(city))
            return BadRequest("City name cannot be empty");

        var newCity = new City { Region = region, CityName = city };
        _dbContext.Cities.Add(newCity);
        await _dbContext.SaveChangesAsync();
        return Ok(newCity);
    }

    [HttpPost("add-account-type")]
    public async Task<IActionResult> AddAccountType(string accountType)
    {
        if (string.IsNullOrWhiteSpace(accountType))
            return BadRequest("Account type name cannot be empty");

        var newAccountType = new AccountType { Name = accountType };
        _dbContext.AccountTypes.Add(newAccountType);
        await _dbContext.SaveChangesAsync();
        return Ok(newAccountType);
    }

    [HttpPost("add-transport-type")]
    public async Task<IActionResult> AddTransportType(string transportType)
    {
        if (string.IsNullOrWhiteSpace(transportType))
            return BadRequest("Transport type name cannot be empty");

        var newTransportType = new TransportType { Name = transportType };
        _dbContext.TransportTypes.Add(newTransportType);
        await _dbContext.SaveChangesAsync();
        return Ok(newTransportType);
    }

  [HttpPost("add-ticket-type")]
    public async Task<IActionResult> AddTicketType(TicketTypeDTO ticketDto)
    {
        var city = await _dbContext.Cities.FirstOrDefaultAsync(c => c.CityName == ticketDto.CityName && c.Region == ticketDto.CityRegion);
        if (city == null)
            return BadRequest("Invalid city");

        var transportType = (TransportType)Enum.Parse(typeof(TransportTypeEnum), ticketDto.TransportType, true);
        var accountType = (AccountType)Enum.Parse(typeof(AccountTypeEnum), ticketDto.AccountType, true);

        var newPrice = new Price
        {
            City = city,
            TransportType = transportType,
            AccountType = accountType,
            Amount = ticketDto.Amount
        };

        _dbContext.Prices.Add(newPrice);
        await _dbContext.SaveChangesAsync();

        return Ok(newPrice);
    }

    [HttpPost("add-transport")]
    public async Task<IActionResult> AddTransport(TransportDTO transportDto)
    {
        //TODO: get region in search scope
        var city = await _dbContext.Cities.FirstOrDefaultAsync(c => c.CityName == transportDto.City);
        if (city == null)
            return BadRequest("Invalid city");

        var transportType = (TransportType)Enum.Parse(typeof(TransportTypeEnum), transportDto.Type, true);

        var newTransport = new Transport
        {
            LicensePlate = transportDto.LicensePlate,
            Type = transportType,
            City = city,
            Terminals = new List<Terminal>()
        };

        _dbContext.Transports.Add(newTransport);
        await _dbContext.SaveChangesAsync();

        return Ok(newTransport);
    }

    [HttpPost("add-terminal")]
    public async Task<IActionResult> AddTerminal(TerminalDTO terminalDto)
    {
        var transport = await _dbContext.Transports.Include(t => t.Terminals)
            .FirstOrDefaultAsync(t => t.LicensePlate == terminalDto.LicensePlate);

        if (transport == null)
            return BadRequest("Transport not found");

        var newTerminal = new Terminal
        {
            Id = Guid.NewGuid(),
            LocationAddress = terminalDto.LocationAddress,
            LocationTransport = transport
        };

        transport.Terminals.Add(newTerminal);
        await _dbContext.SaveChangesAsync();

        return Ok(newTerminal);
    }

    [HttpPost("new-terminal-token")]
    public async Task<IActionResult> GenerateTerminalToken(Guid terminalId)
    {
        var terminal = await _dbContext.Terminals.FindAsync(terminalId);
        if (terminal == null)
            return NotFound("Terminal not found");

        var rawToken = Tools.GenerateToken();
        var tokenHash = Tools.ComputeHash(rawToken);

        var newToken = new TerminalToken
        {
            Terminal = terminal,
            TokenHash = tokenHash,
            ValidUntil = DateTime.UtcNow.AddMonths(6)
        };

        _dbContext.TerminalTokens.Add(newToken);
        await _dbContext.SaveChangesAsync();

        return Ok(new { token = rawToken, validUntil = newToken.ValidUntil });
    }
    private string ComputeHash(string token)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashed = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashed);
    }

    // DTOs for requests
    public class TransportDTO
    {
        [Required] 
        public string LicensePlate { get; set; }
        
        [Required] 
        public string Type { get; set; }
        
        [Required] 
        public string City { get; set; }
    }

    public class TerminalDTO
    {
        [Required] 
        public string LicensePlate { get; set; }
        
        [Required] 
        public string LocationAddress { get; set; }
    }



    public class TicketTypeDTO
    {
        public string AccountType { get; set; }
        public string TransportType { get; set; }
        public string CityName { get; set; }
        public string CityRegion { get; set; }
        public float Amount { get; set; }
    }
}

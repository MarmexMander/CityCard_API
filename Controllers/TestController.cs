using System.Security.Claims;
using System.Transactions;
using CityCard_API.Models.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CityCard_API.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Ok("API is working!");

    [HttpGet("user")]
    [Authorize]
    public IActionResult GetUser(){
        return Ok(User.Claims.Select(c => new { c.Type, c.Value }).ToList());
    }
}

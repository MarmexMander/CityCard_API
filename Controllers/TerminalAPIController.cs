using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/terminal/{action}")]
[Authorize(Policy = "TerminalPolicy")]
class TerminalAPIController : ControllerBase
{
    
}
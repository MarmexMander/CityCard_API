using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/admin/{action}")]
[Authorize(Roles = "Admin")]
class AdminAPIController : ControllerBase
{
    
}
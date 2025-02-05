using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/{action}")]
[Authorize]
class UserAPIController : ControllerBase
{
    
}
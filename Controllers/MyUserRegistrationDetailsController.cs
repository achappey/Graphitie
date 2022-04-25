using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Graphitie.Models; 
using Graphitie.Extensions; 
using Graphitie.Services;

namespace Graphitie.Controllers.Microsoft;

[ApiController]
[Route("[controller]")]
[Authorize]
public class MyUserRegistrationDetailsController : ControllerBase
{
    private readonly ILogger<MyUserRegistrationDetailsController> _logger;

    private readonly GraphitieService _graphitieService;

    public MyUserRegistrationDetailsController(ILogger<MyUserRegistrationDetailsController> logger, GraphitieService graphitieService)
    {
        _logger = logger;
        _graphitieService = graphitieService;
    }

    [HttpGet(Name = "GetMyUserRegistrationDetails")]
    public async Task<UserRegistrationDetails?> Get()
    {
        return await _graphitieService.GetUserRegistrationDetailsByUser(this.HttpContext.GetUserPrincipalName());
    }
}
